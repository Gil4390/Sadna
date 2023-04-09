using SadnaExpress.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using SadnaExpress.DomainLayer.Store;

namespace SadnaExpress.DomainLayer.User
{
    public class UserFacade : IUserFacade
    {
        private ConcurrentDictionary<int, User> current_Users;
        private ConcurrentDictionary<int, Member> members;
        private ConcurrentDictionary<int, LinkedList<Order>> userOrders;
        private int USER_ID = 0;
        private PasswordHash _ph = new PasswordHash();
        

        public UserFacade()
        {
            //guests = new Dictionary<int, Guest>();
            current_Users = new ConcurrentDictionary<int, User>();
            members = new ConcurrentDictionary<int, Member>();
            userOrders = new ConcurrentDictionary<int, LinkedList<Order>>();
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
                bool oneSystemManager = false;
                foreach (Member member in members.Values)
                    if (member.Email.Contains("BGU"))
                        oneSystemManager = true;
                
                if (email.Contains("BGU") || !oneSystemManager)
                {
                    Member newMember = new Member(id, email, firstName, lastName, hashPassword);
                    members.TryAdd(id, newMember);
                    Logger.Instance.Info(newMember,"registered with "+email+".");
                }
                else
                {
                    PromotedMember newMember2 = new PromotedMember(id, email, firstName, lastName, hashPassword);
                    newMember2.createSystemManager();
                    members.TryAdd(id, newMember2);
                    Logger.Instance.Info(newMember2 ,"registered with "+email+".");
                }
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

        public void AddItemToCart(int id,Guid storeID, int itemID,  int itemAmount)
        {
            throw new NotImplementedException();
        }
        public void RemoveItemFromCart(int id,Guid storeID, int itemID,  int itemAmount)
        {
            throw new NotImplementedException();
        }

        public void EditItemFromCart(int id, Guid storeID, int itemID, int itemAmount)
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

        public void EditItemCart(int id, Guid storeID, string itemName)
        {
            throw new NotImplementedException();
        }

        public void OpenNewStore(int id, Guid storeID)
        {
            isLogin(id);
            PromotedMember founder = members[id].openNewStore(storeID);
            members[id] = founder;
        }

        public void AddReview(int id, Guid storeID, string itemName)
        {
            throw new NotImplementedException();
        }

        public void AddItemInventory(int id, Guid storeID, string itemName)
        {
            throw new NotImplementedException();
        }

        public void RemoveItemInventory(int id, Guid storeID, string itemName)
        {
            throw new NotImplementedException();
        }

        public void EditItemInventory(int id, Guid storeID, string itemName)
        {
            throw new NotImplementedException();
        }

        public void AppointStoreOwner(int id, Guid storeID, string email)
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
            PromotedMember owner = members[id].AppointStoreOwner(storeID, newOwner);
            members[newOwnerID] = owner;
        }

        public void AppointStoreManager(int id, Guid storeID, string email)
        {
            isLogin(id);
            Member newManager = null;
            int newManagerID = -1;
            foreach (Member member in members.Values)
                if (member.Email == email)
                {
                    newManager = member;
                    newManagerID = member.UserId;
                }

            if (newManager == null)
                throw new Exception($"There isn't a member with {email}");
            PromotedMember manager = members[id].AppointStoreManager(storeID, newManager);
            members[newManagerID] = manager;
        }

        public void AddStoreManagerPermissions(int id, Guid storeID, string email, string permission)
        {
            isLogin(id);
            Member manager = null;
            foreach (Member member in members.Values)
                if (member.Email == email)
                    manager = member;
            if (manager == null)
                throw new Exception($"There isn't a member with {email}");
            members[id].AddStoreManagerPermissions(storeID, manager, permission);
        }
        public void RemoveStoreManagerPermissions(int id, Guid storeID, string email, string permission)
        {
            isLogin(id);
            Member manager = null;
            foreach (Member member in members.Values)
                if (member.Email == email)
                    manager = member;

            if (manager == null)
                throw new Exception($"There isn't a member with {email}");
            members[id].RemoveStoreManagerPermissions(storeID, manager,permission);
        }
        public List<PromotedMember> GetEmployeeInfoInStore(int id, Guid storeID)
        {
            isLogin(id);
            List<PromotedMember> employees = members[id].GetEmployeeInfoInStore(storeID);
            return employees;
        }

        public void CloseStore(int id, Guid storeID)
        {
            throw new NotImplementedException();
        }

        public void GetDetailsOnStore(int id, Guid storeID)
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

        public bool hasPermissions(int id, Guid storeId, List<string> permissions)
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
        public ShoppingCart ShowShoppingCart(int id)
        {
            isLogin(id);
            if (current_Users.ContainsKey(id))
                return current_Users[id].ShoppingCart;
            
            return members[id].ShoppingCart;
        }
    }
}