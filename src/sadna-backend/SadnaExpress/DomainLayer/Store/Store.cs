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
        private ComplexCondition purchasePolicy;
        public ComplexCondition PurchasePolicy { get => purchasePolicy; set => purchasePolicy = value; }
        public int purchasePolicyCounter;
        public int PurchasePolicyCounter { get => purchasePolicyCounter; set => purchasePolicyCounter = value; }
        public int discountPolicyCounter;
        public int DiscountPolicyCounter { get => discountPolicyCounter; set => discountPolicyCounter = value; }

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
            purchasePolicy = null;
            PurchasePolicyList = new List<Condition>();
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

        public double PurchaseCart(Dictionary<Guid, int> items, ref List<ItemForOrder> itemForOrders, string email)
        {
            Dictionary<Item, int> itemsBeforeDiscount = new Dictionary<Item, int>();
            foreach (Guid itemID in items.Keys)
            {
                itemsBeforeDiscount[GetItemById(itemID)] = items[itemID];
            }

            Dictionary<Item, KeyValuePair<double, DateTime>> itemAfterDiscount =
                new Dictionary<Item, KeyValuePair<double, DateTime>>();
            if (discountPolicyTree != null)
                itemAfterDiscount = discountPolicyTree.calculate(this, itemsBeforeDiscount);
            return itemsInventory.PurchaseCart(items, itemAfterDiscount, ref itemForOrders, storeID , storeName,email);
        }
        public bool CheckPurchasePolicy(Dictionary<Guid, int> items)
        {
            Dictionary<Item, int> basket = new Dictionary<Item, int>();
            foreach (Guid itemID in items.Keys)
            {
                basket.Add(GetItemById(itemID),items[itemID]);
            }
            if (PurchasePolicy == null)
                return true;
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
            return null;
        }

        private Condition checkNotInList(Condition c , DateTime dt=default)
        {
            foreach (Condition cond in PurchasePolicyList)
            {
                if (cond.Equals(c))
                    return null;
            }

            AddSimplePurchaseCondition(c);
            if (dt != default)
                PurchasePolicyList.Add(c);
            return c;
        }
        
        public Condition AddCondition<T>(T entity, string type, double val, DateTime dt=default , string op=default , int opCond=default , bool p=default)
        {
            if (val < 0)
                throw new Exception("value must be positive");
            if (entity == null)
                throw new Exception("entity must be not null");
            Condition cond;
            switch (type)
            {
                case "min value":
                    ValueCondition<T> minValue = new ValueCondition<T>(entity, val, "min");
                    minValue.ID = purchasePolicyCounter++;
                    if (op == "AND" | op =="OR")
                    {
                        minValue.op = op;
                        minValue.opCond = opCond;
                    }
                    cond = checkNotInList(minValue,dt);
                    break;
                case "max value":
                    ValueCondition<T> maxValue = new ValueCondition<T>(entity, val, "max");
                    maxValue.ID = purchasePolicyCounter++;
                    if (op == "AND" | op =="OR")
                    {
                        maxValue.op = op;
                        maxValue.opCond = opCond;
                    }
                    cond = checkNotInList(maxValue,dt);
                    break;
                case "min quantity":
                    QuantityCondition<T> minQuantity = new QuantityCondition<T>(entity, (int)val, "min");
                    minQuantity.ID = purchasePolicyCounter++;
                    if (op == "AND" | op =="OR")
                    {
                        minQuantity.op = op;
                        minQuantity.opCond = opCond;
                    }
                    cond = checkNotInList(minQuantity,dt);
                    break;
                case "max quantity":
                    QuantityCondition<T> maxQuantity = new QuantityCondition<T>(entity, (int)val, "max");
                    maxQuantity.ID = purchasePolicyCounter++;
                    if (op == "AND" | op =="OR")
                    {
                        maxQuantity.op = op;
                        maxQuantity.opCond = opCond;
                    }
                    cond = checkNotInList(maxQuantity,dt);
                    break;
                case "before time":
                    TimeCondition<T> timeBefore = new TimeCondition<T>(entity, dt, "before");
                    timeBefore.ID = purchasePolicyCounter++;
                    if (op == "AND" | op =="OR")
                    {
                        timeBefore.op = op;
                        timeBefore.opCond = opCond;
                    }
                    cond = checkNotInList(timeBefore,dt);
                    break;
                case "after time":
                    TimeCondition<T> timeAfter = new TimeCondition<T>(entity, dt, "after");
                    timeAfter.ID = purchasePolicyCounter++;
                    if (op == "AND" | op =="OR")
                    {
                        timeAfter.op = op;
                        timeAfter.opCond = opCond;
                    }
                    cond = checkNotInList(timeAfter,dt);
                    break;
                default:
                    throw new Exception("the condition not fine");
            }
            if (dt == default)
                condDiscountPolicies.Add(cond, false);
            return cond;
        }

        public ConditioningCondition AddConditioning(Condition cond ,Item item ,string type ,  double val)
        {
            ConditioningCondition cc;
            switch (type)
            {
                case "sum" or "min value" or "max value":
                    ConditioningResultSum crs = new ConditioningResultSum(item,(int)val);
                     cc = new ConditioningCondition(cond,crs);
                    PurchasePolicyList.Remove(cond);
                    cc.ID = purchasePolicyCounter++;;
                    PurchasePolicyList.Add(cc);
                    AddSimplePurchaseCondition(cc);
                    return new ConditioningCondition(cond,crs);
                case "quantity"or "min quantity" or "max quantity":
                    ConditioningResultQuantity crq = new ConditioningResultQuantity(item, (int)val);
                    cc = new ConditioningCondition(cond,crq);
                    PurchasePolicyList.Remove(cond);
                    cc.ID = purchasePolicyCounter++;;
                    PurchasePolicyList.Add(cc);
                    AddSimplePurchaseCondition(cc);
                    return new ConditioningCondition(cond,crq);
                default:
                    throw new Exception("the condition not fine");
            }
            
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
        
        public void AddSimplePurchaseCondition(Condition newPurchasePolicy1,Condition newPurchasePolicy2=null , Operator _op = null)
        {
            if (purchasePolicy == null)
                purchasePolicy = new ComplexCondition(newPurchasePolicy1);
            else if (_op != null)
                purchasePolicy = new ComplexCondition(purchasePolicy.cond1, newPurchasePolicy1, _op);
            else if (purchasePolicy.cond2 == null)
                purchasePolicy = new ComplexCondition(purchasePolicy.cond1, newPurchasePolicy1, new AndOperator());
            else if (purchasePolicy.cond1 != null && purchasePolicy.cond2 != null)
            {
                ComplexCondition cr = new ComplexCondition(purchasePolicy.cond1, purchasePolicy.cond2,purchasePolicy._op );
                purchasePolicy = new ComplexCondition(cr, newPurchasePolicy1, new AndOperator());
            }
            else if (newPurchasePolicy1 != null && newPurchasePolicy2 != null)
            {
                ComplexCondition cr = new ComplexCondition(newPurchasePolicy1, newPurchasePolicy2, _op );
                purchasePolicy = new ComplexCondition(cr, purchasePolicy, new AndOperator());
            }
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

        public void RemoveCondition(Condition cond, ComplexCondition tree = null)
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

        public void RemoveConditionFromList(Condition cond , ComplexCondition tree=null)
        {
            Condition condToRemove = null;
            foreach (Condition condInList in PurchasePolicyList)
            {
                if (condInList.Equals(cond))
                    condToRemove = condInList;
            }

            if (condToRemove != null)
                PurchasePolicyList.Remove(condToRemove);
        }

        public bool EvaluatePurchasePolicy(Store store, Dictionary<Item, int> basket)
        {
            if (purchasePolicy != null)
                return purchasePolicy.Evaluate(store , basket);
            return true;
        }

        private void ifQuantityCond(Condition cond , List<PurchaseCondition> conds )
        {
            if(typeof(QuantityCondition<Item>) == cond.GetType())
            {
                PurchaseCondition newPurCond = new PurchaseCondition(((QuantityCondition<Item>)cond).ID,
                    ((QuantityCondition<Item>)cond).entity.ToString().Split('.')[3],
                    ((QuantityCondition<Item>)cond).entity.ItemID.ToString(),
                    ((QuantityCondition<Item>)cond).entity.Name,
                    ((QuantityCondition<Item>)cond).minmax + " quantity", ((QuantityCondition<Item>)cond).Quantity
                );
                if (((QuantityCondition<Item>)cond).op != "")
                {
                    newPurCond.Op = ((QuantityCondition<Item>)cond).op;
                    newPurCond.OpCond = ((QuantityCondition<Item>)cond).opCond;
                }
                conds.Add(newPurCond);
            }
            if(typeof(QuantityCondition<string>) == cond.GetType())
            {
                PurchaseCondition newPurCond = new PurchaseCondition(((QuantityCondition<string>)cond).ID,
                    ((QuantityCondition<string>)cond).entity,
                    "",
                    ((QuantityCondition<string>)cond).entity,
                    ((QuantityCondition<string>)cond).minmax + " quantity", ((QuantityCondition<string>)cond).Quantity
                );
                if (((QuantityCondition<string>)cond).op != "")
                {
                    newPurCond.Op = ((QuantityCondition<string>)cond).op;
                    newPurCond.OpCond = ((QuantityCondition<string>)cond).opCond;
                }
                
                if (newPurCond.Entity != "Item" && newPurCond.Entity != "Store")
                    newPurCond.Entity = "Category";
                conds.Add(newPurCond);
            }
        }
        private void ifValueCond(Condition cond , List<PurchaseCondition> conds )
        {
            if(typeof(ValueCondition<Item>) == cond.GetType())
            {
                PurchaseCondition newPurCond = new PurchaseCondition(((ValueCondition<Item>)cond).ID,
                    ((ValueCondition<Item>)cond).entity.ToString().Split('.')[3],
                    ((ValueCondition<Item>)cond).entity.ItemID.ToString(), ((ValueCondition<Item>)cond).entity.Name,
                    ((ValueCondition<Item>)cond).minmax + " sum", (int)((ValueCondition<Item>)cond).minPrice
                );
                if (((ValueCondition<Item>)cond).op != "")
                {
                    newPurCond.Op = ((ValueCondition<Item>)cond).op;
                    newPurCond.OpCond = ((ValueCondition<Item>)cond).opCond;
                }
                conds.Add(newPurCond);
            }
            if(typeof(ValueCondition<string>) == cond.GetType())
            {
                PurchaseCondition newPurCond = new PurchaseCondition(((ValueCondition<string>)cond).ID,
                    ((ValueCondition<string>)cond).entity,
                    "", ((ValueCondition<string>)cond).entity,
                    ((ValueCondition<string>)cond).minmax + " sum", (int)((ValueCondition<string>)cond).minPrice
                );
                if (((ValueCondition<string>)cond).op != "")
                {
                    newPurCond.Op = ((ValueCondition<string>)cond).op;
                    newPurCond.OpCond = ((ValueCondition<string>)cond).opCond;
                }

                if (newPurCond.Entity != "Item" && newPurCond.Entity != "Store")
                    newPurCond.Entity = "Category";
                conds.Add(newPurCond);
            }
        }
        

        public PurchaseCondition[] GetAllConditions()
        {
            List<PurchaseCondition> conds = new List<PurchaseCondition>();
            foreach (Condition cond in PurchasePolicyList)
            {
                ifQuantityCond(cond,conds);
                ifValueCond(cond,conds);
                if(typeof(ConditioningCondition) == cond.GetType())
                {
                    if (((ConditioningCondition)cond).res.GetType() == typeof(ConditioningResultSum))
                    {
                        if (((ConditioningCondition)cond).cond.GetType() == typeof(ValueCondition<Item>))
                        {
                            conds.Add(new PurchaseCondition(((ValueCondition<Item>)((ConditioningCondition)cond).cond).ID,
                                ((ValueCondition<Item>)((ConditioningCondition)cond).cond).entity.ToString().Split('.')[3],
                                ((ValueCondition<Item>)((ConditioningCondition)cond).cond).entity.ItemID.ToString(),((ValueCondition<Item>)((ConditioningCondition)cond).cond).entity.Name,
                                ((ValueCondition<Item>)((ConditioningCondition)cond).cond).minmax + " sum",(int)((ValueCondition<Item>)((ConditioningCondition)cond).cond).minPrice,"Conditioning","Item",
                                ((ConditioningCondition)cond).res.item.ItemID.ToString(),((ConditioningCondition)cond).res.item.Name,"sum",(int)((ConditioningResultSum)((ConditioningCondition)cond).res).sum,-1)
                            );
                        }
                        if (((ConditioningCondition)cond).GetType() == typeof(QuantityCondition<Item>))
                        {
                            conds.Add(new PurchaseCondition(((QuantityCondition<Item>)((ConditioningCondition)cond).cond).ID,
                                ((QuantityCondition<Item>)((ConditioningCondition)cond).cond).entity.ToString().Split('.')[3],
                                ((QuantityCondition<Item>)((ConditioningCondition)cond).cond).entity.ItemID.ToString(),((QuantityCondition<Item>)((ConditioningCondition)cond).cond).entity.Name,
                                ((QuantityCondition<Item>)((ConditioningCondition)cond).cond).minmax + " sum",((QuantityCondition<Item>)((ConditioningCondition)cond).cond).Quantity,"Conditioning","Item",
                                ((ConditioningCondition)cond).res.item.ItemID.ToString(),((ConditioningCondition)cond).res.item.Name,"sum",(int)((ConditioningResultSum)((ConditioningCondition)cond).res).sum,-1)
                            );
                        }
                    }

                    if (((ConditioningCondition)cond).res.GetType() == typeof(ConditioningResultQuantity))
                    {
                        if (((ConditioningCondition)cond).cond.GetType() == typeof(ValueCondition<Item>))
                        {
                            conds.Add(new PurchaseCondition(((ValueCondition<Item>)((ConditioningCondition)cond).cond).ID,
                                ((ValueCondition<Item>)((ConditioningCondition)cond).cond).entity.ToString().Split('.')[3],
                                ((ValueCondition<Item>)((ConditioningCondition)cond).cond).entity.ItemID.ToString(),
                                ((ValueCondition<Item>)((ConditioningCondition)cond).cond).entity.Name,
                                ((ValueCondition<Item>)((ConditioningCondition)cond).cond).minmax + " sum",
                                (int)((ValueCondition<Item>)((ConditioningCondition)cond).cond).minPrice, "Conditioning", "Item",
                                ((ConditioningCondition)cond).res.item.ItemID.ToString(),
                                ((ConditioningCondition)cond).res.item.Name, "quantity",
                                (int)((ConditioningResultQuantity)((ConditioningCondition)cond).res).quantity, -1)
                            );
                        }
                        if (((ConditioningCondition)cond).cond.GetType() == typeof(QuantityCondition<Item>))
                        {
                            conds.Add(new PurchaseCondition(((QuantityCondition<Item>)((ConditioningCondition)cond).cond).ID,
                                ((QuantityCondition<Item>)((ConditioningCondition)cond).cond).entity.ToString().Split('.')[3],
                                ((QuantityCondition<Item>)((ConditioningCondition)cond).cond).entity.ItemID.ToString(),
                                ((QuantityCondition<Item>)((ConditioningCondition)cond).cond).entity.Name,
                                ((QuantityCondition<Item>)((ConditioningCondition)cond).cond).minmax + " sum",
                                ((QuantityCondition<Item>)((ConditioningCondition)cond).cond).Quantity, "Conditioning", "Item",
                                ((ConditioningCondition)cond).res.item.ItemID.ToString(),
                                ((ConditioningCondition)cond).res.item.Name, "quantity",
                                (int)((ConditioningResultQuantity)((ConditioningCondition)cond).res).quantity, -1)
                            );
                        }
                    }
                }
            }

            return conds.ToArray();
        }

        public bool ItemExist(Guid itemid)
        {
            return itemsInventory.ItemExist(itemid);
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
    }
}
