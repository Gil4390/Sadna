using System;
using System.Collections.Generic;

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


    public void addItemCart(string storeName, string itemName)
    {
        throw new NotImplementedException();
    }

    public void addItemInventory(string storeName, string itemName)
    {
        throw new NotImplementedException();
    }

    public void addItemToBag(string storeName, string itemName)
    {
        throw new NotImplementedException();
    }

    public void addManager(string storeName, string email)
    {
        throw new NotImplementedException();
    }

    public void addOwner(string storeName, string email)
    {
        throw new NotImplementedException();
    }

    public void addPermissionsToManager(string storeName, string email, string Permission)
    {
        throw new NotImplementedException();
    }

    public void addReview(string storeName, string itemName)
    {
        throw new NotImplementedException();
    }

    public void closeStore(string storeName)
    {
        throw new NotImplementedException();
    }

    public void editItemCart(string storeName, string itemName)
    {
        throw new NotImplementedException();
    }

    public void editItemInventory(string storeName, string itemName)
    {
        throw new NotImplementedException();
    }

    public void enter(int id)
    {
        Guest guest = new Guest(id);
        guests.Add(id , guest);
    }

    public void exit(int id)
    {
        if (guests[id] != null)
            throw new Exception();
        guests.Remove(id);
    }

    public Dictionary<string, List<string>> getDetailsOnCart()
    {
        throw new NotImplementedException();
    }

    public void getDetailsOnStore(string storeName)
    {
        throw new NotImplementedException();
    }

    public void login(string email, string password)
    {
        throw new NotImplementedException();
    }

    public void logout()
    {
        throw new NotImplementedException();
    }

    public void openStore(string storeName)
    {
        throw new NotImplementedException();
    }

    public void purchaseCart()
    {
        throw new NotImplementedException();
    }

    public void register(int id, string email, string firstName, string lastName, string password)
    {
        if (members[id] != null && members[id].Email != email)
            throw new Exception();
        string hashPassword = _ph.Hash(password);
        Member newMember = new Member(id, email, firstName, lastName, hashPassword);
        members.Add(id, newMember);
    }

    public void removeCart(string storeName, string itemName)
    {
        throw new NotImplementedException();
    }

    public void removeItemInventory(string storeName, string itemName)
    {
        throw new NotImplementedException();
    }
}
}