using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using SadnaExpress.DomainLayer.Store.DiscountPolicy;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpress.DomainLayer.Store
{
    public class Store
    {
        private string storeName;
        public string StoreName {get => storeName; set => storeName = value;}
        public Inventory itemsInventory;
        private Guid storeID;
        public Guid StoreID {get=>storeID;}

        private bool active;
        public bool Active { get => active; set => active = value; }

        private int storeRating;
        private DiscountPolicyTree discountPolicyTree;
        public DiscountPolicyTree DiscountPolicyTree { get => discountPolicyTree; set => discountPolicyTree = value;}
        public int StoreRating {  get => storeRating ; set => StoreRating = value; }

        public Store(string name) {
            storeName = name;
            itemsInventory = new Inventory();
            storeRating = 0;
            storeID = Guid.NewGuid();
            active = true;
            discountPolicyTree = null;
        }

        public Item GetItemsByName(string itemName, int minPrice, int maxPrice, string category, int ratingItem)
        {
            return itemsInventory.GetItemByName(itemName, minPrice, maxPrice,category, ratingItem);
        }
        public List<Item> GetItemsByCategory(string category, int minPrice, int maxPrice, int ratingItem)
        {
            return itemsInventory.GetItemsByCategory(category, minPrice, maxPrice, ratingItem);
        }
        public List<Item> GetItemsByKeysWord(string keyWords, int minPrice, int maxPrice, int ratingItem, string category)
        {
            return itemsInventory.GetItemsByKeysWord(keyWords, minPrice, maxPrice, ratingItem, category);
        }
        public Guid AddItem(string name, string category, double price, int quantity)
        {
            return itemsInventory.AddItem(name, category, price, quantity);
        }
        public void RemoveItem(Guid itemID)
        {
            itemsInventory.RemoveItem(itemID);
        }
        public void EditItemPrice(Guid itemID, int price)
        {
            itemsInventory.EditItemPrice(itemID, price);
        }
        public void EditItemName(Guid itemID, string name)
        {
            itemsInventory.EditItemName(itemID, name);
        }
        public void EditItemCategory(Guid itemID, string category)
        {
            itemsInventory.EditItemCategory(itemID, category);
        }

        public void EditItemQuantity(Guid itemID, int quantity)
        {
            itemsInventory.EditItemQuantity(itemID, quantity);
        }

        public void AddItemToCart(Guid itemID, int quantity)
        {
            itemsInventory.AddItemToCart(itemID, quantity);
        }

        public double PurchaseCart(Dictionary<Guid, int> items, ref List<ItemForOrder> itemForOrders)
        {
            return itemsInventory.PurchaseCart(items, ref itemForOrders, storeID);
        }

        public Item GetItemById(Guid itemID)
        {
            return itemsInventory.GetItemById(itemID);
        }
        public int GetItemByQuantity(Guid itemID)
        {
            return itemsInventory.GetItemByQuantity(itemID);
        }

        public DiscountPolicy.DiscountPolicy CreateSimplePolicy<T>(T level, int percent, DateTime startDate, DateTime endDate)
        {
            SimpleDiscount<T> simpleDiscount = new SimpleDiscount<T>(level, percent, startDate, endDate);
            return simpleDiscount;
        }
        public Condition AddCondition<T>(T level, string type, int val)
        {
            switch (type)
            {
               case "min value":
                   MinValueCondition<T> minValue = new MinValueCondition<T>(level, val);
                   return minValue;
               case "min quantity":
                   MinQuantityCondition<T> minQuantity = new MinQuantityCondition<T>(level, val);
                   return minQuantity;
               default:
                   throw new Exception("the condition not fine");
            }
            
        }
        public DiscountPolicy.DiscountPolicy CreateComplexPolicy(string op, params object[] policys)
        {
            switch (op)
            {
                case "xor":
                    XorDiscount xor = new XorDiscount((Condition)policys[0], (Condition)policys[1], (DiscountPolicy.DiscountPolicy)policys[2]);
                    return xor;
                case "and":
                    AndDiscount and = new AndDiscount((Condition)policys[0], (Condition)policys[1], (DiscountPolicy.DiscountPolicy)policys[2]);
                    return and;
                case "or":
                    OrDiscount or = new OrDiscount((Condition)policys[0], (Condition)policys[1], (DiscountPolicy.DiscountPolicy)policys[2]);
                    return or;
                case "if":
                    ConditionalDiscount ifCond = new ConditionalDiscount((Condition)policys[0],(DiscountPolicy.DiscountPolicy)policys[1]);
                    return ifCond;
                case "max":
                    MaxDiscount max = new MaxDiscount((DiscountPolicy.DiscountPolicy)policys[0], (DiscountPolicy.DiscountPolicy)policys[1]);
                    return max;
                case "add":
                    AddDiscount add = new AddDiscount((DiscountPolicy.DiscountPolicy)policys[0], (DiscountPolicy.DiscountPolicy)policys[1]);
                    return add;
                default:
                    throw new Exception("the op not exist");
            }
        }

        public DiscountPolicyTree AddPolicy(DiscountPolicy.DiscountPolicy discountPolicy)
        {
            if (discountPolicyTree == null)
                discountPolicyTree = new DiscountPolicyTree(discountPolicy);
            else
                discountPolicyTree.AddPolicy(discountPolicy);
            return discountPolicyTree;
        }

        public void RemovePolicy(DiscountPolicy.DiscountPolicy discountPolicy)
        {
            discountPolicyTree.RemovePolicy(discountPolicy);
        }
    }
}
