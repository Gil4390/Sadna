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

        public string StoreName
        {
            get => storeName;
            set => storeName = value;
        }

        public Inventory itemsInventory;
        private Guid storeID;

        public Guid StoreID
        {
            get => storeID;
        }

        private bool active;

        public bool Active
        {
            get => active;
            set => active = value;
        }

        private int storeRating;
        private DiscountPolicyTree discountPolicyTree;

        public DiscountPolicyTree DiscountPolicyTree
        {
            get => discountPolicyTree;
            set => discountPolicyTree = value;
        }

        private ComplexCondition purchasePolicy;

        public ComplexCondition PurchasePolicy
        {
            get => purchasePolicy;
            set => purchasePolicy = value;
        }

        public List<Condition> PurchasePolicyList { get; set; }

        public int StoreRating
        {
            get => storeRating;
            set => StoreRating = value;
        }

        public Store(string name)
        {
            storeName = name;
            itemsInventory = new Inventory();
            storeRating = 0;
            storeID = Guid.NewGuid();
            active = true;
            discountPolicyTree = null;
        }
        public bool Equals(Store store)
        {
            return store.storeName == storeName && store.itemsInventory.Equals(itemsInventory)
                                                && store.storeRating == storeRating && store.storeID == storeID
                                                && store.active == active;
        }

        public Item GetItemsByName(string itemName, int minPrice, int maxPrice, string category, int ratingItem)
        {
            return itemsInventory.GetItemByName(itemName, minPrice, maxPrice, category, ratingItem);
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

        public double PurchaseCart(Dictionary<Guid, int> items, ref List<ItemForOrder> itemForOrders, string email)
        {
            return itemsInventory.PurchaseCart(items, ref itemForOrders, storeID , storeName,email);
        }
        public bool CheckPurchasePolicy(Dictionary<Guid, int> items, ref List<ItemForOrder> itemForOrders)
        {
            Dictionary<Item, int> basket = new Dictionary<Item, int>();
            foreach (Guid itemID in items.Keys)
            {
                basket.Add(GetItemById(itemID),items[itemID]);
            }

            return PurchasePolicy.Evaluate(this, basket);
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
        public Condition AddCondition<T>(T entity, string type, double val, DateTime dt=default)
        {
            if (val < 0)
                throw new Exception("value must be positive");
            if (entity == null)
                throw new Exception("entity must be not null");
            switch (entity)
            {
                case Item item:
                    if (GetItemById(item.ItemID) == null)
                        throw new Exception("entity must be not null");
                    break;
            }
            switch (type)
            {
                case "min value":
                    ValueCondition<T> minValue = new ValueCondition<T>(entity, val, "min");
                    return minValue;
                case "max value":
                    ValueCondition<T> maxValue = new ValueCondition<T>(entity, val, "max");
                    return maxValue;
                case "min quantity":
                    QuantityCondition<T> minQuantity = new QuantityCondition<T>(entity, (int)val, "min");
                    return minQuantity;
                case "max quantity":
                    QuantityCondition<T> maxQuantity = new QuantityCondition<T>(entity, (int)val, "max");
                    return maxQuantity;
                case "before time":
                    TimeCondition<T> timeBefore = new TimeCondition<T>(entity, dt, "before");
                    return timeBefore;
                case "after time":
                    TimeCondition<T> timeAfter = new TimeCondition<T>(entity, dt, "after");
                    return timeAfter;
                default:
                    throw new Exception("the condition not fine");
            }
        }

        public ConditioningCondition AddConditioning(Condition cond ,Item item ,string type ,  double val)
        {
            switch (type)
            {
                case "sum":
                    ConditioningResultSum crs = new ConditioningResultSum(item,(int)val);
                    return new ConditioningCondition(cond,crs);
                case "quantity":
                    ConditioningResultQuantity crq = new ConditioningResultQuantity(item, (int)val);
                    return new ConditioningCondition(cond,crq);
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

        public void AddNewConditionToPurchasePolicy(Condition c)
        {
            PurchasePolicyList.Add(c);
        }

        public void AddSimplePurchaseCondition(Condition newPurchasePolicy1,Condition newPurchasePolicy2=null , Operator _op = null)
        {
            if (purchasePolicy == null)
                purchasePolicy = new ComplexCondition(newPurchasePolicy1);
            else if (_op != null)
                purchasePolicy = new ComplexCondition(purchasePolicy.cond1, newPurchasePolicy1, _op);
            else if (purchasePolicy.cond2 == null)
                purchasePolicy = new ComplexCondition(purchasePolicy.cond1, newPurchasePolicy1, new AndOperator());
            else 
                purchasePolicy = new ComplexCondition(newPurchasePolicy1, newPurchasePolicy2, _op);
        }

        public Condition BuildCondition(Condition newPurchasePolicy1, Condition newPurchasePolicy2 = null,
            Operator _op = null)
        {
            return new ComplexCondition(newPurchasePolicy1, newPurchasePolicy2, _op);
        }
        
        public Condition GetCondition(Condition cond , ComplexCondition tree=null)
        {
            if (cond == null || PurchasePolicy == null)
                return null;
            if (cond.GetType() == typeof(ComplexCondition))
            {
                if (tree == null)
                    tree = PurchasePolicy;
                Condition searchCond1 = GetCondition(cond , (ComplexCondition)tree.cond1);
                if (searchCond1 != null)
                    return searchCond1;
                Condition searchCond2 = GetCondition(cond , (ComplexCondition)tree.cond2);
                if (searchCond2 != null)
                    return searchCond2;
            }
            if (tree == null)
            {
                if (cond.Equals(PurchasePolicy.cond1) || cond.Equals(PurchasePolicy.cond2))
                    return cond;
            }
            else if (cond.Equals(tree))
                return cond;
            return null;
        }
        public void RemoveCondition(Condition cond , ComplexCondition tree=null)
        {
            if (cond == null || PurchasePolicy == null)
            {
                throw new Exception("Condition now exists");
            }
            if (cond.GetType() == typeof(ComplexCondition))
            {
                if (tree == null)
                    tree = PurchasePolicy;
                Condition searchCond1 = GetCondition(cond ,(ComplexCondition)tree.cond1);
                if (searchCond1 != null)
                {
                    tree.cond1 = null;
                    PurchasePolicyList.Remove(searchCond1);
                }
                Condition searchCond2 = GetCondition(cond ,(ComplexCondition)tree.cond2);
                if (searchCond2 != null)
                {
                    tree.cond2 = null;
                    PurchasePolicyList.Remove(searchCond2);
                }

            }
            else if (tree == null)
            {
                if (cond.Equals(PurchasePolicy.cond1))
                {
                    PurchasePolicyList.Remove(PurchasePolicy.cond1);
                    PurchasePolicy = null;
                }
                else if (cond.Equals(PurchasePolicy.cond2))
                {
                    PurchasePolicyList.Remove(PurchasePolicy.cond2);
                    PurchasePolicy.cond2 = null;
                }
            }
            else if (cond.Equals(tree))
            {
                PurchasePolicyList = new List<Condition>();
                PurchasePolicy = null;
            }
        }

        public bool EvaluatePurchasePolicy(Store store, Dictionary<Item, int> basket)
        {
            if (purchasePolicy != null)
                return purchasePolicy.Evaluate(store , basket);
            return true;
        }

        public Condition[] GetAllConditions()
        {
            return PurchasePolicyList.ToArray();
        }

        public bool ItemExist(Guid itemid)
        {
            return itemsInventory.ItemExist(itemid);
        }
    }
}
