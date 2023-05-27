using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace SadnaExpress.DomainLayer.Store
{
    public class Order 
    {
        [Key]
        public Guid OrderID { get; set; }
        private List<ItemForOrder> listItems;

        [NotMapped]
        public List<ItemForOrder> ListItems { get => listItems; set => listItems = value; }

        [NotMapped]
        public string OrderIDsJson
        {
            get
            {
                List<Guid> orderIds = new List<Guid>();
                foreach (ItemForOrder item in ListItems)
                    orderIds.Add(item.ItemForOrderId);
                return JsonConvert.SerializeObject(orderIds);
            }
            set
            {
            }
        }

        public string ListItemsDB { get; set; }


        public Order()
        {
            
        }

        
        private DateTime orderTime;
        public DateTime OrderTime { get => orderTime; set => orderTime = value; }


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