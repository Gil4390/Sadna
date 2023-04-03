using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class Inventory
    {
        private Item item;
        private double price;
        private int in_stock;
        private Policy policy;
        private Store store;
        // maybe need to add discount

        //         username, description
        Dictionary<string, string> reviews;
        //         username, description
        Dictionary<string, int> reviewsRating;


        public Inventory(Item item, int in_stock, double price, Policy policy, Store store)
        {
            this.item = item;
            this.in_stock = in_stock;
            this.price = price;
            this.policy = policy;
            this.store = store;
            this.reviews = new Dictionary<string, string>();
            this.reviewsRating = new Dictionary<string, int>();
        }

        //getters

        public Item getItem()
        {
            return this.item;
        }
        public Policy getPolicy()
        {
            return this.policy;
        }

        public Store getStore()
        {
            return this.store;
        }
        public int getInStock()
        {
            return this.in_stock;
        }

        public double getPrice()
        {
            return this.price;
        }


        public string getName()
        {
            return item.getName();
        }

        public string getCategory()
        {
            return item.getCategory();
        }

        public Dictionary<string, string> getReviews()
        {
            return this.reviews;
        }

        public Dictionary<string, int> getReviewsRating()
        {
            return this.reviewsRating;
        }

        // setters 

        public void setItem(Item newItem)
        {
            this.item = newItem;
        }
        public void setPolicy(Policy newPolicy)
        {
            this.policy = newPolicy;
        }

        public void setStore(Store newStore)
        {
            this.store = newStore;
        }
        public void setInStock(int newIn_stock)
        {
            this.in_stock = newIn_stock;
        }

        public void setPrice(double newPrice)
        {
            this.price = newPrice;
        }


        public void addReview(string username, string descreption, int rating)
        {
            this.reviews.Add(username, descreption);
            this.reviewsRating.Add(username, rating);
        }

    }
}