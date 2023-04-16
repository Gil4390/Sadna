using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class Order
    {
        private List<ItemForOrder> listItems;
        public List<ItemForOrder> ListItems { get=>listItems; }
        private DateTime date;
        public DateTime Date;

        public Order(List<ItemForOrder> listItems)
        {
            this.listItems = listItems;
            date = DateTime.Now;
        }
    }
}