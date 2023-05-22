using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace SadnaExpress.DomainLayer.Store
{
    public class Order // todo in database 
    {
        [Key]
        public Guid OrderID { get; set; }
        private List<ItemForOrder> listItems;

        public List<ItemForOrder> ListItems { get => listItems; set => listItems = value; }

        public Order()
        {
            OrderID = Guid.NewGuid();
        }

        
        private DateTime orderTime;
        public DateTime OrderTime {get=>orderTime;}


        public Order(List<ItemForOrder> listItems)
        {
            this.listItems = listItems;

            OrderID = Guid.NewGuid();

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