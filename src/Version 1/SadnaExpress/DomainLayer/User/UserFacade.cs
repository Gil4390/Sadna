using System;
using System.Collections.Generic;
using System.Threading;

namespace SadnaExpress.DomainLayer.User
{
    public class UserFacade : IUserFacade
    {
        Dictionary<int, Guest> guests;
        Dictionary<int, Member> members;
        PasswordHash _ph = new PasswordHash();

        public UserFacade()
        {
            guests = new Dictionary<int, Guest>();
            members = new Dictionary<int, Member>();
        }

        public void Enter(int id)
        {
            if (guests[id] != null)
                throw new SadnaException("guests[id] == null","UserFacade","Enter");
            Guest g = new Guest(id);
            lock (this)
            {
                guests.Add(id , g);
            }
            Logger.Info(g ,"Enter the system.");
        }

        public void Exit(int id)
        {
            if (guests[id] == null)
                throw new SadnaException("guests[id] != null","UserFacade","Exit");
            Guest g = guests[id];
            lock (this)
            {
                guests.Remove(id);
            }
            Logger.Info(g ,"exited from the system.");
        }

        public void Register(int id, string email, string firstName, string lastName, string password)
        {
            if (members[id] != null)
                throw new SadnaException("guests[id] == null","UserFacade","Enter");
            if (members[id].Email != email)
                throw new SadnaException("members[id].Email != email","UserFacade","Enter");
            string hashPassword = _ph.Hash(password);
            Member newMember = new Member(id, email, firstName, lastName, hashPassword);
            lock (this)
            {
                members.Add(id, newMember);
            }
            Logger.Info(newMember ,"registered with "+email+".");
        }

        public void Login(int id, string email, string password)
        {
            throw new NotImplementedException();
        }

        public void Logout(int id)
        {
            throw new NotImplementedException();
        }

        public void AddItemToBag(int id, string storeName, string itemName)
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

        public void AddItemCart(int id, string storeName, string itemName)
        {
            throw new NotImplementedException();
        }

        public void RemoveCart(int id, string storeName, string itemName)
        {
            throw new NotImplementedException();
        }

        public void EditItemCart(int id, string storeName, string itemName)
        {
            throw new NotImplementedException();
        }

        public void OpenStore(int id, string storeName)
        {
            throw new NotImplementedException();
        }

        public void AddReview(int id, string storeName, string itemName)
        {
            throw new NotImplementedException();
        }

        public void AddItemInventory(int id, string storeName, string itemName)
        {
            throw new NotImplementedException();
        }

        public void RemoveItemInventory(int id, string storeName, string itemName)
        {
            throw new NotImplementedException();
        }

        public void EditItemInventory(int id, string storeName, string itemName)
        {
            throw new NotImplementedException();
        }

        public void AddOwner(int id, string storeName, string email)
        {
            throw new NotImplementedException();
        }

        public void AddManager(int id, string storeName, string email)
        {
            throw new NotImplementedException();
        }

        public void AddPermissionsToManager(int id, string storeName, string email, string Permission)
        {
            throw new NotImplementedException();
        }

        public void CloseStore(int id, string storeName)
        {
            throw new NotImplementedException();
        }

        public void GetDetailsOnStore(int id, string storeName)
        {
            throw new NotImplementedException();
        }
    }
}