using System;
using System.Collections.Generic;
using System.Linq;

namespace SadnaExpress.DomainLayer.Store.Policy
{
    public abstract class Condition
    {
        public int ID;
        public abstract bool Evaluate(Store store, Dictionary<Item, int> basket);
        public abstract bool Equals(Condition cond);
    }
    
    public class ComplexCondition: Condition
    {
        public Condition cond1;
        public Condition cond2;
        public Operator _op;

        public ComplexCondition(Condition cond1, Condition cond2, Operator op)
        {
            this.cond1 = cond1;
            this.cond2 = cond2;
            _op = op;
        }
        public ComplexCondition(Condition cond1)
        {
            this.cond1 = cond1;
            this.cond2 = null;
            _op = null;
        }
        public override bool Evaluate(Store store, Dictionary<Item, int> basket)
        {
            if (typeof(ConditioningCondition) == cond1.GetType() || cond2 == null)
                return cond1.Evaluate(store, basket);
            return _op.Calculate(cond1, cond2 , store , basket);
        }

        public override bool Equals(Condition cond)
        {
            switch (cond)
            {
                case ComplexCondition cc1:
                    return cond1.Equals(cc1.cond1) && cond2.Equals(cc1.cond2) && _op.GetType() == cc1.GetType();
                default:
                    return false;
            }
        }
        
    }

    public class ConditioningCondition : Condition
    {
        public Condition cond;
        public ConditioningResult res;

        public ConditioningCondition(Condition cond1, ConditioningResult result)
        {
            this.cond = cond1;
            this.res = result;
        }
        public override bool Evaluate(Store store, Dictionary<Item, int> basket)
        {
            return res.Evaluate(basket) & cond.Evaluate(store, basket);
        }

        public override bool Equals(Condition eCondition)
        {
            switch (eCondition)
            {
                case ConditioningCondition cc2:
                    return cond.Equals(cc2.cond) && res.Equals(cc2.res);
                default:
                    return false;
            }
        }
    }
    
    public abstract class ConditioningResult
    {
        public Item item;

        public ConditioningResult(Item item)
        {
            this.item = item;
        }

        public abstract bool Evaluate(Dictionary<Item, int> basket);

        public abstract bool Equals(ConditioningResult cr);

    }
    
    public class ConditioningResultQuantity : ConditioningResult
    {
        public int quantity;
        public ConditioningResultQuantity(Item item , int q) : base(item)
        {
            this.quantity = q;
        }

        public override bool Evaluate(Dictionary<Item, int> basket)
        {
            int newQuantity = 0;
            foreach (Item i in basket.Keys)
            {
                newQuantity += basket[i];
            }
            return newQuantity == quantity;
        }

        public override bool Equals(ConditioningResult cr)
        {
            switch (cr)
            {
                case ConditioningResultQuantity cc1:
                    return cr.item.ItemID == item.ItemID && quantity == cc1.quantity;
                default:
                    return false;
            }
        }
    }

    public class ConditioningResultSum : ConditioningResult
    {
        public double sum;
        public ConditioningResultSum(Item item , int s) : base(item)
        {
            this.sum = s;
        }

        public override bool Evaluate(Dictionary<Item, int> basket)
        {
            double newSum = 0;
            foreach (Item i in basket.Keys)
            {
                newSum += basket[i];
            }
            return newSum == sum;
        }

        public override bool Equals(ConditioningResult cr)
        {
            switch (cr)
            {
                case ConditioningResultSum cc1:
                    return cr.item.ItemID == item.ItemID && sum == cc1.sum;
                default:
                    return false;
            }
        }
    }

    public class ValueCondition<T> : Condition
    {
        public T entity;
        public double minPrice;
        public string minmax;
        public string op;
        public int opCond;

        // T can be store or category(string)
        public ValueCondition(T entity, double minPrice, string minmax)
        {
            this.entity = entity;
            this.minPrice = minPrice;
            this.minmax = minmax;
            op = "";
            opCond = -1;
        }

        public override bool Evaluate(Store store, Dictionary<Item, int> basket)
        {
            double sum = 0;
            switch (entity)
            {
                case Store storeType:
                    foreach (Item item in basket.Keys)
                        sum += item.Price;
                    break;
                case string stringType:
                    foreach (Item item in basket.Keys)
                        if (item.Category.Equals(entity))
                            sum += item.Price;
                    break;
                case ShoppingBasket shopping:
                    foreach (Item item in basket.Keys)
                        sum += item.Price;
                    break;
                case Item item:
                    foreach (Item i in basket.Keys)
                        if(i.ItemID == item.ItemID)
                            sum += i.Price;
                    break;
                default:
                    throw new Exception("Category or Store are the one we can evaluate");
            }
            return Check(sum);
        }

        public override bool Equals(Condition cond)
        {
            switch (cond)
            {
                case ValueCondition<T> cc1:
                    switch (cc1.entity)
                    {
                        case Item i:
                            switch (entity)
                            {
                                case Item i2:
                                    return i.Equals(i2) && cc1.minPrice == minPrice && cc1.minmax == minmax;
                                default:
                                    return false;
                            }
                        case Store i:
                            switch (entity)
                            {
                                case Store i2:
                                    return i.Equals(i2) && cc1.minPrice == minPrice && cc1.minmax == minmax;
                                default:
                                    return false;
                            }
                        case string category:
                            switch (entity)
                            {
                                case string category2:
                                    return category == category2 && cc1.minPrice == minPrice && cc1.minmax == minmax;
                                default:
                                    return false;
                            }
                        default:
                            return false;
                    }
                default:
                    return false;
            }
        }

        private bool Check(double sum)
        {
            switch (minmax)
            {
                case "min":
                    if (sum >= minPrice)
                        return true;
                    switch (entity)
                    {
                        case string category:
                            throw new Exception($"The quantity {minPrice} is smaller then the allowed {sum} of {category}");
                        case Item item:
                            throw new Exception($"The quantity {minPrice} is smaller then the allowed {sum} of {item.Name}");
                    }
                    break;
                case "max":
                    if ( sum <= minPrice)
                        return true;
                    switch (entity)
                    {
                        case string category:
                            throw new Exception($"The quantity {minPrice} is bigger then the allowed {sum} of {category}");
                        case Item item:
                            throw new Exception($"The quantity {minPrice} is smaller then the allowed {sum} of {item.Name}");
                    }

                    break;

                default:
                    throw new Exception("Need to be one of this operators");
            }

            return false;
        }
    }
    
    public class QuantityCondition<T> : Condition
    {
        public T entity;
        public int Quantity;
        public string minmax;
        public string op;
        public int opCond;

        // T can be store or item or category(string)
        public QuantityCondition(T entity, int Quantity , string minmax)
        {
            this.entity = entity;
            this.Quantity = Quantity;
            this.minmax = minmax;
            op = "";
            opCond = -1;
        }

        public override bool Evaluate(Store store, Dictionary<Item, int> basket)
        {
            int quantity = 0;
            switch (entity)
            {
                case Store storeType:
                    foreach (Item item in basket.Keys)
                        quantity += basket[item];
                    break;
                case string stringType:
                    foreach (Item item in basket.Keys)
                        if (item.Category.Equals(entity))
                            quantity += basket[item];
                    break;
                case Item itemType:
                    if (basket.ContainsKey(itemType))
                        quantity = basket[itemType];
                    break;
                case ShoppingBasket shopping:
                    foreach (Item item in basket.Keys)
                        quantity += basket[item];
                    break;
                default:
                    throw new Exception("Category or Store or item are the one we can evaluate");
            }
            return Check(quantity);
        }

        public override bool Equals(Condition cond)
        {
            switch (cond)
            {
                case QuantityCondition<T> cc1:
                    switch (cc1.entity)
                    {
                        case Item i:
                            switch (entity)
                            {
                                case Item i2:
                                    return i.Equals(i2) && cc1.Quantity == Quantity && cc1.minmax == minmax;
                                default:
                                    return false;
                            }
                        case Store i:
                            switch (entity)
                            {
                                case Store i2:
                                    return i.Equals(i2) && cc1.Quantity == Quantity && cc1.minmax == minmax;
                                default:
                                    return false;
                            }
                        case string category:
                            switch (entity)
                            {
                                case string category2:
                                    return category == category2 && cc1.Quantity == Quantity && cc1.minmax == minmax;
                                default:
                                    return false;
                            }
                        default:
                            return false;
                    }
                default:
                    return false;
            }
        }

        private bool Check(int q)
        {
            switch (minmax)
            {
                case "min":
                    if (this.Quantity <= q)
                        return true;
                    throw new Exception($"The quantity {Quantity} is bigger then the allowed {q}");
                case "max":
                    if (this.Quantity >= q)
                        return true;
                    throw new Exception($"The quantity {Quantity} is smaller then the allowed {q}");
                default:
                    throw new Exception("Need to be one of this operators");
            }
        }
    }
    
    public class TimeCondition<T> : Condition
    {
        public T entity;
        public DateTime timing;
        public string beforeAfter;
        public string op;
        public int opCond;

        // T can be store or item or category(string)
        public TimeCondition(T entity, DateTime timing , string beforeAfter)
        {
            this.entity = entity;
            this.timing = timing;
            this.beforeAfter = beforeAfter;
            op = "";
            opCond = -1;
        }

        public override bool Evaluate(Store store, Dictionary<Item, int> basket)
        {
            int quantity = 0;
            switch (entity)
            {
                case string category:
                    foreach (Item item in basket.Keys)
                        if (item.Category == category)
                            return Check();
                    break;
                case Item itemType:
                    if (basket.ContainsKey(itemType))
                        return Check();
                    break;
                default:
                    throw new Exception("Category or item are the one we can evaluate");
            }

            return true;
        }

        public override bool Equals(Condition cond)
        {
            switch (cond)
            {
                case TimeCondition<T> cc1:
                    return cc1.entity.Equals(entity) && cc1.timing == timing && cc1.beforeAfter == beforeAfter;
                default:
                    return false;
            }
        }

        private bool Check()
        {
            switch (beforeAfter)
            {
                case "before":
                    return this.timing.Hour > DateTime.Now.Hour;
                case "after":
                    return this.timing.Hour <= DateTime.Now.Hour;
                default:
                    throw new Exception("Need to be one of this operators");
            }
        }
    }

    public class PurchaseCondition
    {
        private int condID;
        public int CondID
        {
            get => condID;
            set => condID = value;
        }

        private string entity;
        public string Entity
        {
            get => entity;
            set => entity = value;
        }

        private string entityID;
        public string EntityID
        {
            get => entityID;
            set => entityID = value;
        }
        
        private string entityName;
        public string EntityName
        {
            get => entityName;
            set => entityName = value;
        }

        private string type;
        public string Type
        {
            get => type;
            set => type = value;
        }

        private int value;
        public int Value
        {
            get => value;
            set => this.value = value;
        }

        private string op;
        public string Op
        {
            get => op;
            set => op = value;
        }

        private string entityRes;
        public string EntityRes
        {
            get => entityRes;
            set => entityRes = value;
        }

        private string entityIDRes;
        public string EntityIDRes
        {
            get => entityIDRes;
            set => entityIDRes = value;
        }
        private string entityNameRes;
        public string EntityNameRes
        {
            get => entityNameRes;
            set => entityNameRes = value;
        }

        private string typeRes;
        public string TypeRes
        {
            get => typeRes;
            set => typeRes = value;
        }

        private int valueRes;
        public int ValueRes
        {
            get => valueRes;
            set => valueRes = value;
        }

        private int opCond;
        public int OpCond
        {
            get => opCond;
            set => opCond = value;
        }

        public PurchaseCondition(int condId, string entity, string entityId, string entityName, string type, int value, string op, string entityRes, string entityIdRes, string entityNameRes, string typeRes, int valueRes, int opCond)
        {
            condID = condId;
            this.entity = entity;
            entityID = entityId;
            this.entityName = entityName;
            this.type = type;
            this.value = value;
            this.op = op;
            this.entityRes = entityRes;
            entityIDRes = entityIdRes;
            this.entityNameRes = entityNameRes;
            this.typeRes = typeRes;
            this.valueRes = valueRes;
            this.opCond = opCond;
        }
        
        public PurchaseCondition(int condId, string entity, string entityId,string entityName, string type, int value)
        {
            this.condID = condId;
            this.entity = entity;
            entityID = entityId;
            this.entityName = entityName;
            this.type = type;
            this.value = value;
            this.op = "";
            this.entityRes = "";
            entityIDRes = "";
            this.typeRes = "";
            this.valueRes = -1;
            this.opCond = -1;
        }

        
    }
}