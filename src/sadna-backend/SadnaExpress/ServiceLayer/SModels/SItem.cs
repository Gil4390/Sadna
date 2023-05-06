using SadnaExpress.DomainLayer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.ServiceLayer.Obj
{
    public class SItem
    {
        private string itemId;
        public string ItemId { get => itemId; }
        private string name;
        public string Name { get => name; set => name = value; }
        private string category;
        public string Category { get => category; set => category = value; }
        private double price;
        public double Price { get => price; set => price = value; }
        private int rating;
        public int Rating { get => rating; set => rating = value; }
        private string storeId;
        public string StoreId { get => storeId; }
        private bool inStock;
        public bool InStock { get => inStock; }
        private int count;
        public int Count { get => count; set => count = value; }

        public SItem(Item item, Guid storeID,bool inStock, int count)
        {
            this.itemId = item.ItemID.ToString(); this.name = item.Name;
            this.price=item.Price; this.rating = item.Rating;
            this.category = item.Category;
            this.storeId = storeID.ToString();
            this.inStock = inStock;
            this.count = count;
        }
    }
}
