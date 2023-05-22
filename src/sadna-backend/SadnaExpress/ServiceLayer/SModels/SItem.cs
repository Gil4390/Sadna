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
        private double priceDiscount;
        public double PriceDiscount { get => priceDiscount; set => priceDiscount = value; }
        private double offerPrice;
        public double OfferPrice { get => offerPrice; set => offerPrice = value; }
        private bool openBid; // to show the button
        public bool OpenBid { get => openBid; set => openBid = value; }
        private int rating;
        public int Rating { get => rating; set => rating = value; }
        private string storeId;
        public string StoreId { get => storeId; }
        private bool inStock;
        public bool InStock { get => inStock; }
        private int count;
        public int Count { get => count; set => count = value; }
        
        public SItem(Item item, double priceDiscount, KeyValuePair<double, bool> bid, Guid storeID, bool inStock,
            int count)
        {
            this.itemId = item.ItemID.ToString();
            this.name = item.Name;
            this.price = item.Price;
            this.rating = item.Rating;
            this.category = item.Category;
            if (bid.Value)
                offerPrice = bid.Key;
            else
                offerPrice = -1;
            openBid = !bid.Value;
            this.priceDiscount = priceDiscount;
            this.storeId = storeID.ToString();
            this.inStock = inStock;
            this.count = count;
        }

        public SItem(Item item, double priceDiscount, Guid storeID, bool inStock, int count)
        {
            this.itemId = item.ItemID.ToString();
            this.name = item.Name;
            this.price = item.Price;
            this.rating = item.Rating;
            this.category = item.Category;
            this.OfferPrice = -1;
            this.openBid = false;
            this.priceDiscount = priceDiscount;
            this.storeId = storeID.ToString();
            this.inStock = inStock;
            this.count = count;
        }
    }

    public class SStore
    {
        private Guid storeId;
        public Guid StoreId { get => storeId; }
        private string name;
        public string Name { get => name; set => name = value; }
        private bool isOpen;
        public bool IsOpen { get => isOpen; }

        public SStore(Store store)
        {
            this.storeId = store.StoreID;
            this.name = store.StoreName;
            this.isOpen = store.Active; 
        }
    }
}
