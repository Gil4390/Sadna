using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class Order
    {
        private int userID;
        private Guid storeID;
        private LinkedList<String> listItems;
        private double price;

        public Order(int userID, Guid storeId,LinkedList<String> listItems,double price)
        {
            this.userID = userID;
            storeID = storeId;
            this.listItems = listItems;
            this.price = price;
        }

        public int getUserID()
        {
            return userID;
        }

        public Guid getStoreID()
        {
            return storeID;
        }
        public LinkedList<String> getListItems()
        {
            return listItems;
        }
        public double getPrice()
        {
            return price;
        }
    }
}