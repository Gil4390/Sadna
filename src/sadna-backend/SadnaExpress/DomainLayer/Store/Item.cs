using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SadnaExpress.DomainLayer.Store
{
    public class Item
    {
        private Guid itemID;
        [Key]
        public Guid ItemID { get => itemID; set => itemID = value; }

        private string name;
        public string Name {get => name; set => name = value;}
        private string category;
        public string Category {get => category; set => category = value;}
        private double price;
        public double Price {get => price; set => price = value;}
        private int rating;
        public int Rating {get => rating; set => rating = value;}
        
        private int quantity;
        public int Quantity {get => quantity; set => quantity = value;}
        
        public Guid InventoryID { get; set; } // added for dbcontext

        public Item(string name, string category, double price)
        {
            this.name = name;
            this.category = category;
            this.price = price;
            rating = 0;
            itemID = Guid.NewGuid(); 
        }
        public bool Equals(Item item)
        {
            return item.name == name && item.itemID == itemID && item.category == category && item.price == price &&
                   item.rating == rating;
        }
        

        public Item()
        {

        }

    }
}