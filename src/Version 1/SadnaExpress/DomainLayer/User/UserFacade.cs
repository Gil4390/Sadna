using SadnaExpress.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;

namespace SadnaExpress.DomainLayer.User
{
    public class UserFacade : IUserFacade
    {
        //Dictionary<int, Guest> guests;
        private ConcurrentDictionary<int, User> current_Users;
        private ConcurrentDictionary<int, Member> members;
        private int USER_ID = 0;
        private PasswordHash _ph = new PasswordHash();

        public UserFacade()
        {
            //guests = new Dictionary<int, Guest>();
            current_Users = new ConcurrentDictionary<int, User>();
            members = new ConcurrentDictionary<int, Member>();
        }

        public UserFacade(ConcurrentDictionary<int, User> current_Users, ConcurrentDictionary<int, Member> members, int uSER_ID, PasswordHash ph)
        {
            this.current_Users = current_Users;
            this.members = members;
            USER_ID = uSER_ID;
            _ph = ph;
        }

        public int Enter()
        {
            lock (this)
            {
                User user = new User(USER_ID);
                current_Users.TryAdd(USER_ID, user);
                USER_ID++;
                Logger.Instance.Info(user ,"Enter the system.");
                return user.UserId;
            }
        }

        public void Exit(int id)
        {
            lock (this)
            {
                User user;
                Member member;
                if (current_Users.TryRemove(id, out user))
                    Logger.Instance.Info(user, "exited from the system.");
                else if (members.TryRemove(id, out member))
                    Logger.Instance.Info(member, "exited from the system.");
                else
                {
                    throw new Exception("Error with exiting system with this id- " + id);
                }
            }
        }

        public void Register(int id, string email, string firstName, string lastName, string password)
        {
            lock (this)
            {
                if (members.ContainsKey(id))
                    throw new Exception("user with this id already logged in");
                foreach (Member m in members.Values)
                {
                    if (m.Email == email)
                        throw new Exception("email already exists");
                }
                string hashPassword = _ph.Hash(password);
                Member newMember = new Member(id, email, firstName, lastName, hashPassword);
                members.TryAdd(id, newMember);
                Logger.Instance.Info(newMember ,"registered with "+email+".");
            }

        }

        public int Login(int id, string email, string password)
        {
            lock (this)
            {
                foreach (Member member in members.Values)
                {
                    if (member.Email.Equals(email))
                    {
                        if (!_ph.Rehash(password, member.Password))
                        {
                            throw new Exception("wrong password for email");
                        }
                        else
                        {
                            //correct email & password:
                            member.LoggedIn = true;
                            User user;
                            current_Users.TryRemove(id, out user);
                            Logger.Instance.Info(member, "logged in");

                            return member.UserId;
                        }
                    }
                }

                //eamil not found
                throw new Exception("email doesn't exist");
            }
        }

        public int Logout(int id)
        {
            lock (this)
            {
                if (!members.ContainsKey(id))
                    throw new Exception("member with id dosen't exist");

                Member member = members[id];
                member.LoggedIn = false;
                Logger.Instance.Info(member, "logged out");
                return Enter(); //member logs out and a regular user enters the system instead
            }
        }

        public void AddItemToBag(int id, int storeID, string itemName)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, List<string>> getDetailsOnCart()
        {
            throw new NotImplementedException();
        }

        public void PurchaseCart(int id)
        {
            throw new NotImplementedException();
        }

        public void AddItemCart(int id, int storeID, string itemName)
        {
            throw new NotImplementedException();
        }

        public void RemoveCart(int id, int storeID, string itemName)
        {
            throw new NotImplementedException();
        }

        public void EditItemCart(int id, int storeID, string itemName)
        {
            throw new NotImplementedException();
        }

        public void OpenStore(int id, int storeID)
        {
            isLogin(id);
            PromotedMember founder = members[id].openStore(storeID);
            members[id] = founder;
        }

        public void AddReview(int id, int storeID, string itemName)
        {
            throw new NotImplementedException();
        }

        public void AddItemInventory(int id, int storeID, string itemName)
        {
            throw new NotImplementedException();
        }

        public void RemoveItemInventory(int id, int storeID, string itemName)
        {
            throw new NotImplementedException();
        }

        public void EditItemInventory(int id, int storeID, string itemName)
        {
            throw new NotImplementedException();
        }

        public void AddOwner(int id, int storeID, string email)
        {
            lock (this)
            {
                isLogin(id);
                Member newOwner = null;
                int newOwnerID = -1;
                foreach (Member member in members.Values)
                    if (member.Email == email)
                    {
                        newOwner = member;
                        newOwnerID = member.UserId;
                    }

                if (newOwner == null)
                    throw new Exception($"There isn't a member with {email}");
                PromotedMember owner = members[id].addNewOwner(storeID, newOwner);
                members[newOwnerID] = owner;
            }
        }

        public void AddManager(int id, int storeID, string email)
        {
            throw new NotImplementedException();
        }

        public void AddPermissionsToManager(int id, int storeID, string email, string Permission)
        {
            throw new NotImplementedException();
        }

        public void CloseStore(int id, int storeID)
        {
            throw new NotImplementedException();
        }

        public void GetDetailsOnStore(int id, int storeID)
        {
            throw new NotImplementedException();
        }

        public void CleanUp()
        {
            current_Users.Clear();
            members.Clear();
        }

        public bool InitializeTradingSystem(int id)
        {
            //functions steps:
            //1. check that this is id member
            //2. check that member is log in
            //3. check that member is system manager
            //4. check that there is connection to payment and supply services

            if(!members.ContainsKey(id))
                throw new Exception("member with id dosen't exist");

            if (!members[id].LoggedIn)
                throw new Exception("To Initialize the trading system member must be logged in");

            //impl of 3- throw error if not

            return true;

        }

        public bool hasPermissions(int id, int storeId, LinkedList<string> permissions)
        {
            if (members.ContainsKey(id))
                if (members[id].hasPermissions(storeId, permissions))
                    return true;
            return false;
        }
        private void isLogin(int id)
        {
            if (members.ContainsKey(id))
            {
                if (members[id].LoggedIn)
                    return;
                throw new Exception("member need to login");
            }
            throw new Exception("User need to register first");
        }

        public ConcurrentDictionary<int, User> GetCurrent_Users()
        {
            return current_Users;
        }
        public ConcurrentDictionary<int, Member> GetMembers()
        {
            return members;
        }
    }
}