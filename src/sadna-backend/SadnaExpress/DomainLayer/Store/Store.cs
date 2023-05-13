using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SadnaExpress.DomainLayer.Store.Policy;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpress.DomainLayer.Store
{
    public class Store
    {
        private string storeName;
        public string StoreName { get => storeName; set => storeName = value; }
        public Inventory itemsInventory;
        private Guid storeID;
        public Guid StoreID { get => storeID; }
        private bool active;
        public bool Active { get => active; set => active = value; }
        private int storeRating;
        private DiscountPolicyTree discountPolicyTree;
        public DiscountPolicyTree DiscountPolicyTree { get => discountPolicyTree; set => discountPolicyTree = value; }
        private Dictionary<DiscountPolicy, bool> allDiscountPolicies;
        public Dictionary<DiscountPolicy, bool> AllDiscountPolicies {get => allDiscountPolicies;set => allDiscountPolicies = value;}
        private Dictionary<Condition, bool> condDiscountPolicies;
        public Dictionary<Condition, bool> CondDiscountPolicies {get => condDiscountPolicies;set => condDiscountPolicies = value;}
        private int purchasePolicyCounter;
        public int PurchasePolicyCounter { get => purchasePolicyCounter; set => purchasePolicyCounter = value; }
        private int discountPolicyCounter;
        public int DiscountPolicyCounter { get => discountPolicyCounter; set => discountPolicyCounter = value; }
        private List<Condition> purchasePolicyList;
        public List<Condition> PurchasePolicyList { get => purchasePolicyList; set => purchasePolicyList = value;}

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
            purchasePolicyList = new List<Condition>();
            allDiscountPolicies = new Dictionary<DiscountPolicy, bool>();
            condDiscountPolicies = new Dictionary<Condition, bool>();
            purchasePolicyCounter = 0;
            discountPolicyCounter = 0;

        }
        
        public bool Equals(Store store)
        {
            return store.storeName == storeName && store.itemsInventory.Equals(itemsInventory)
                                                && store.storeRating == storeRating && store.storeID == storeID
                                                && store.active == active;
        }
        #region items
        public Item GetItemById(Guid itemID)
        {
            //return error
            return itemsInventory.GetItemById(itemID);
        }
        
        public bool ItemExist(Guid itemid)
        {
            //not return error
            return itemsInventory.ItemExist(itemid);
        }
        public int GetItemByQuantity(Guid itemID)
        {
            return itemsInventory.GetItemByQuantity(itemID);
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
        public void EditItemPrice(Guid itemID, double price)
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
        public double GetItemAfterDiscount(Item item)
        {
            Dictionary<Item, int> itemsBeforeDiscount = new Dictionary<Item, int>{{item, 1}};
            Dictionary<Item, KeyValuePair<double, DateTime>> itemAfterDiscount =
                new Dictionary<Item, KeyValuePair<double, DateTime>>();
            if (discountPolicyTree != null)
                itemAfterDiscount = discountPolicyTree.calculate(this, itemsBeforeDiscount);
            if (itemAfterDiscount.Count == 0)
                return -1;
            return itemAfterDiscount[item].Key;
        }
        #endregion
        
        #region purchase + before
        public Dictionary<Item, double> GetCartItems(Dictionary<Guid, int> items)
        {
            Dictionary<Item, double> basketItems = new Dictionary<Item, double>();
            Dictionary<Item, int> itemsBeforeDiscount = new Dictionary<Item, int>();
            foreach (Guid itemID in items.Keys)
            {
                itemsBeforeDiscount.Add(GetItemById(itemID), items[itemID]);
            }
            Dictionary<Item, KeyValuePair<double, DateTime>> itemAfterDiscount =
                new Dictionary<Item, KeyValuePair<double, DateTime>>();
            if (discountPolicyTree != null)
                itemAfterDiscount = discountPolicyTree.calculate(this, itemsBeforeDiscount);
            foreach (Item item in itemsBeforeDiscount.Keys)
            {
                if (itemAfterDiscount.Keys.Contains(item))
                    basketItems.Add(item, itemAfterDiscount[item].Key);
                else
                    basketItems.Add(item, -1);
            }
            return basketItems;
        }
        
        // purchase the check of the policy done earlier(when we click on check)
        public double PurchaseCart(Dictionary<Guid, int> items, ref List<ItemForOrder> itemForOrders, string email)
        {
            Dictionary<Item, int> itemsBeforeDiscount = new Dictionary<Item, int>();
            foreach (Guid itemID in items.Keys)
                itemsBeforeDiscount[GetItemById(itemID)] = items[itemID];
            // calculate the cart after the discount
            Dictionary<Item, KeyValuePair<double, DateTime>> itemAfterDiscount =
                new Dictionary<Item, KeyValuePair<double, DateTime>>();
            if (discountPolicyTree != null)
                itemAfterDiscount = discountPolicyTree.calculate(this, itemsBeforeDiscount);
            // remove the item from the store inventory
            return itemsInventory.PurchaseCart(items, itemAfterDiscount, ref itemForOrders, storeID , storeName,email);
        }
        #endregion

        #region Discount Policy
        //  create policy in not active state, add policy make it active
        private DiscountPolicy HelperInCreateSimplePolicy(DiscountPolicy discountPolicy)
        {
            if (allDiscountPolicies.ContainsKey(discountPolicy))
                throw new Exception("the policy already exist");
            discountPolicy.ID = discountPolicyCounter;
            discountPolicyCounter++;
            allDiscountPolicies.Add(discountPolicy, false);
            return discountPolicy;
        }
        
        public DiscountPolicy CreateSimplePolicy<T>(T level, int percent, DateTime startDate, DateTime endDate)
        {
            if (percent < 0)
                throw new Exception("Invalid percent amount");
            if (level.GetType() == typeof(string))
            {
                switch (level.ToString())
                {
                    case string s when s.StartsWith("Item"):
                        Item item = itemsInventory.GetItemByName(s.Substring(4));
                        return HelperInCreateSimplePolicy(new SimpleDiscount<Item>(item, percent, startDate, endDate));
                    case string s when s.StartsWith("Store"):
                        return HelperInCreateSimplePolicy(new SimpleDiscount<Store>(this, percent, startDate, endDate));
                    case string s when s.StartsWith("Category"):
                        return HelperInCreateSimplePolicy(new SimpleDiscount<string>(s.Substring(8), percent, startDate, endDate));
                }
            }

            throw new Exception("Entity not valid");
        }
        
        private DiscountPolicy HelperInCreateComplexPolicy(DiscountPolicy discountPolicy)
        {
            if (allDiscountPolicies.ContainsKey(discountPolicy))
                throw new Exception("the policy already exist");
            discountPolicy.ID = discountPolicyCounter;
            discountPolicyCounter++;
            allDiscountPolicies.Add(discountPolicy, false);
            return discountPolicy;
        }
 
        public DiscountPolicy CreateComplexPolicy(string op, params int[] policys)
        {
            switch (op)
            {
                case "xor":
                    XorDiscount xor = new XorDiscount(GetCondByID(policys[0]), GetCondByID(policys[1]), GetPolicyByID(policys[2]));
                    allDiscountPolicies.Remove(GetPolicyByID(policys[2]));
                    return HelperInCreateComplexPolicy(xor);
                case "and":
                    AndDiscount and = new AndDiscount(GetCondByID(policys[0]), GetCondByID(policys[1]), GetPolicyByID(policys[2]));
                    allDiscountPolicies.Remove(GetPolicyByID(policys[2]));
                    return HelperInCreateComplexPolicy(and);
                case "or":
                    OrDiscount or = new OrDiscount(GetCondByID(policys[0]), GetCondByID(policys[1]), GetPolicyByID(policys[2]));
                    allDiscountPolicies.Remove(GetPolicyByID(policys[2]));
                    return HelperInCreateComplexPolicy(or);
                case "if":
                    ConditionalDiscount ifCond = new ConditionalDiscount(GetCondByID(policys[0]),GetPolicyByID(policys[1]));
                    allDiscountPolicies.Remove(GetPolicyByID(policys[1]));
                    return HelperInCreateComplexPolicy(ifCond);
                case "max":
                    MaxDiscount max = new MaxDiscount(GetPolicyByID(policys[0]), GetPolicyByID(policys[1]));
                    allDiscountPolicies.Remove(GetPolicyByID(policys[0]));
                    allDiscountPolicies.Remove(GetPolicyByID(policys[1]));
                    return HelperInCreateComplexPolicy(max);
                case "add":
                    AddDiscount add = new AddDiscount(GetPolicyByID(policys[0]), GetPolicyByID(policys[1]));
                    allDiscountPolicies.Remove(GetPolicyByID(policys[0]));
                    allDiscountPolicies.Remove(GetPolicyByID(policys[1]));
                    return HelperInCreateComplexPolicy(add);
                default:
                    throw new Exception("the op not exist");
            }
        }
        
        public DiscountPolicyTree AddPolicy(int ID)
        {
            if (discountPolicyTree == null)
                discountPolicyTree = new DiscountPolicyTree(GetPolicyByID(ID));
            else
                discountPolicyTree.AddPolicy(GetPolicyByID(ID));
            allDiscountPolicies[GetPolicyByID(ID)] = true;
            return discountPolicyTree;
        }

        public void RemovePolicy(int ID)
        {
            Condition toRemoveCond = null;
            foreach (Condition condition in condDiscountPolicies.Keys)
            {
                if (condition.ID == ID)
                    toRemoveCond = condition;
            }

            if (toRemoveCond != null)
            {
                condDiscountPolicies.Remove(toRemoveCond);
                return;
            }
            
            DiscountPolicy toRemove = null;
            foreach (DiscountPolicy discountPolicy in allDiscountPolicies.Keys)
            {
                if (discountPolicy.ID == ID)
                    toRemove = discountPolicy;
            }

            if (toRemove != null)
            {
                discountPolicyTree.RemovePolicy(GetPolicyByID(ID));
                allDiscountPolicies.Remove(toRemove);
            }
            else
                throw new Exception("Policy/Condition not found");
        }
        
        private DiscountPolicy GetPolicyByID(int ID)
        {
            foreach (DiscountPolicy discountPolicy in allDiscountPolicies.Keys)
                if (discountPolicy.ID == ID)
                    return discountPolicy;
            throw new Exception($"The policy with {ID} not exist");
        }
        
        private Condition GetCondByID(int ID)
        {
            foreach (Condition condPolicy in condDiscountPolicies.Keys)
                if (condPolicy.ID == ID)
                    return condPolicy;
            throw new Exception($"The condition with {ID} not exist");
        }
        #endregion

        #region both policies

        public Condition AddCondition(string entityStr, string entityName, string type, double val, DateTime dt=default, string op=default , int opCond=default)
        {
            switch (entityStr)
            {
                case "Item":
                    Item entityI = itemsInventory.GetItemByName(entityName);
                    return AddConditionHelper(entityI, type, val, dt, op, opCond);
                case "Store":
                    return AddConditionHelper(this, type, val, dt, op, opCond);
                case "Category":
                    return AddConditionHelper(entityStr, type, val, dt, op, opCond);
                default:
                    throw new Exception("the entity not exist");
            }
        }
        
        public Condition AddConditionHelper<T>(T entity, string type, double val, DateTime dt=default, string op=default , int opCond=default)
        {
            if (val < 0)
                throw new Exception("value must be positive");
            if (entity == null)
                throw new Exception("entity must be not null");
            Condition cond;
            switch (type)
            {
                case "min value":
                    cond = new ValueCondition<T>(entity, val, "min");
                    break;
                case "max value":
                    cond = new ValueCondition<T>(entity, val, "max");
                    break;
                case "min quantity":
                    cond = new QuantityCondition<T>(entity, (int)val, "min");
                    break;
                case "max quantity":
                    cond = new QuantityCondition<T>(entity, (int)val, "max");  
                    break;
                case "before time":
                    cond = new TimeCondition<T>(entity, dt, "before");
                    break;
                case "after time":
                    cond = new TimeCondition<T>(entity, dt, "after");
                    break;
                default:
                    throw new Exception($"the condition type {type} not exist");
            }
            return checkNotInList(cond,dt, op, opCond);
        }
        
        private Condition checkNotInList(Condition c , DateTime dt=default, string op=default , int opCond=default)
        {
            if (dt != default)
            {
                if (PurchasePolicyList.Contains(c))
                    throw new Exception("the condition already exist");
                 c = AddSimplePurchaseCondition(c, GetPurchaseCond(opCond), op);
            }
            else
                if (!condDiscountPolicies.ContainsKey(c))
                    condDiscountPolicies.Add(c, false);
            c.ID = PurchasePolicyCounter;
            PurchasePolicyCounter++;
            return c;
        }
        #endregion


        #region purchase policy
        public Condition AddSimplePurchaseCondition(Condition newPurchasePolicy1,Condition newPurchasePolicy2=null , string op = null)
        {
            Condition purchasePolicy = newPurchasePolicy1;
            switch (op)
            {
                case "and":
                    purchasePolicy = new AndCondition(newPurchasePolicy1, newPurchasePolicy2);
                    PurchasePolicyList.Remove(newPurchasePolicy2);
                    break;
                case "or":
                    purchasePolicy = new OrCondition(newPurchasePolicy1, newPurchasePolicy2);
                    PurchasePolicyList.Remove(newPurchasePolicy2);
                    break;
                case "if":
                    purchasePolicy = new ConditioningCondition(newPurchasePolicy1, newPurchasePolicy2);
                    PurchasePolicyList.Remove(newPurchasePolicy2);
                    break;
            }
            PurchasePolicyList.Add(purchasePolicy);
            return purchasePolicy;
        }

        public Condition GetPurchaseCond(int condID)
        {
            foreach (Condition cond in purchasePolicyList)
            {
                if (cond.ID == condID)
                    return cond;
            }
            return null;
        }
        
        public void RemoveCondition(int condID)
        {
            Condition cond = GetPurchaseCond(condID);
            if (cond == null)
            {
                throw new Exception($"Condition {condID} not found");
            }
            purchasePolicyList.Remove(cond);
        }

        public bool EvaluatePurchasePolicy(Store store, Dictionary<Item, int> basket)
        {
            bool output = true;
            foreach (Condition purchasePolicy in PurchasePolicyList)
            {
                output = output && purchasePolicy.Evaluate(store, basket);
                if (!output)
                    throw new Exception(purchasePolicy.message);
            }
            return true;
        }
        
        public List<Condition> GetAllConditions()
        {
            return PurchasePolicyList;
        }

        public bool CheckPurchasePolicy(Dictionary<Guid, int> items)
        {
            Dictionary<Item, int> basket = new Dictionary<Item, int>();
            foreach (Guid itemID in items.Keys)
                basket.Add(GetItemById(itemID),items[itemID]);
            return EvaluatePurchasePolicy(this, basket);
        }
        #endregion
    }
}
