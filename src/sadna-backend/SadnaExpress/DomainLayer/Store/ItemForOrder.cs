using System;
using System.ComponentModel.DataAnnotations;

namespace SadnaExpress.DomainLayer.Store
{
    public class ItemForOrder // todo in database 
    {
        [Key]
        public Guid ItemForOrderId { get; set; }

        private Guid itemID;
        public Guid ItemID { get => itemID; set => itemID = value; }

        private Guid storeID;
        public Guid StoreID {get=>storeID; set => storeID = value; }

        private string name;
        public string Name {get => name; set => name = value; }

        private string category;
        public string Category {get => category; set => category = value; }

        private double price;
        public double Price {get => price; set => price = value; }

        private int rating;
        public int Rating {get => rating; set => rating = value; }
        
        private string userEmail;

        public string UserEmail
        {
            get => userEmail;
            set => userEmail = value;
        }

        private string storeName;

        public string StoreName { get => storeName; set => storeName = value; }

        public ItemForOrder(Item item, Guid storeID,  string userEmail, string storeName)
        {
            ItemForOrderId = Guid.NewGuid();
            itemID = item.ItemID;
            name = item.Name;
            category = item.Category;
            price = item.Price;
            rating = item.Rating;
            this.userEmail = userEmail;
            this.storeName = storeName;
            this.storeID = storeID;
        }
        
        public ItemForOrder(Item item, double discountPrice, Guid storeID, string userEmail, string storeName)
        {
            itemID = item.ItemID;
            name = item.Name;
            category = item.Category;
            price = discountPrice;
            rating = item.Rating;
            this.userEmail = userEmail;
            this.storeName = storeName;
            this.storeID = storeID;
        }

        public ItemForOrder()
        {

        }
    }
}