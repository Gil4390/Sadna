using System;

namespace SadnaExpress.DomainLayer.Store
{
    public class ItemForOrder
    {
        private Guid itemID;
        public Guid ItemID {get=>itemID;}
        private Guid storeID;
        public Guid StoreID {get=>storeID;}
        private string name;
        public string Name {get => name;}
        private string category;
        public string Category {get => category;}
        private double price;
        public double Price {get => price;}
        private int rating;
        public int Rating {get => rating;}

        public ItemForOrder(Item item, Guid storeID)
        {
            itemID = item.ItemID;
            name = item.Name;
            category = item.Category;
            price = item.Price;
            rating = item.Rating;
            this.storeID = storeID;
        }
        
    }
}