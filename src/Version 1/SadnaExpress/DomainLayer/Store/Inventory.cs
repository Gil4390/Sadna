using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class Inventory
    {
        private Item item;
        private double price;
        private int in_stock;
        private Policy policy;
        private DiscountPolicy discount;
        //private Store store;
        // maybe need to add discount


        public Inventory(Item item, int in_stock, double price, DiscountPolicy dPolicy)
        {
            this.item = item;
            this.in_stock = in_stock;
            this.price = price;
            this.policy = null;
            this.discount = dPolicy;
            
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

        public int GetId()
        {
            return item.GetId();
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

        
        public void setInStock(int newIn_stock)
        {
            this.in_stock = newIn_stock;
        }

        public void setPrice(double newPrice)
        {
            this.price = newPrice;
        }


        public void addInStock(int newInStock)
        {
            this.in_stock += newInStock;
        }

        public void removeInStock(int delInStock)
        {
            this.in_stock -= delInStock;
        }

    }
}