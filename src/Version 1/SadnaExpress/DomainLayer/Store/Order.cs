using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class Order
    {
        private Guid userID;
        public Guid UserID {get=>userID;}
        private Guid storeID;
        public Guid StoreID {get=>storeID;}
        private List<Guid> listItems;
        public List<Guid> ListItem { get=>listItems; }
        private double price;
        public double Price { get=>price; }

        public Order(Guid userID, Guid storeId,List<Guid> listItems,double price)
        {
            this.userID = userID;
            storeID = storeId;
            this.listItems = listItems;
            this.price = price;
        }
    }
}