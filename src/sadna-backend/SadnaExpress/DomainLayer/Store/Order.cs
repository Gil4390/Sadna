using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace SadnaExpress.DomainLayer.Store
{
    public class Order
    {
        private List<ItemForOrder> listItems;
        public List<ItemForOrder> ListItems { get=>listItems; }
        private DateTime orderTime;
        public DateTime OrderTime {get=>orderTime;}

        public Order(List<ItemForOrder> listItems)
        {
            this.listItems = listItems;
            orderTime = DateTime.Now;
        }

        public double CalculatorAmount()
        {
            double sum = 0;
            foreach (ItemForOrder item in listItems)
                sum += item.Price;
            return sum;
        }
    }
}