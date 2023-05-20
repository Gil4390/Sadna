using System;
using System.Collections.Generic;
using System.Linq;

namespace SadnaExpress.DomainLayer.Store.Policy
{
    public abstract class Condition
    {
        public int ID;
        public string message=null;
        public abstract bool Evaluate(Store store, Dictionary<Item, int> basket);
        public abstract bool Equals(Condition cond);
        public abstract override string ToString();
        
    }
    
    #region quantity
    public class QuantityCondition<T> : Condition
    {
        public T entity;
        public int Quantity;
        public string minmax;

        // T can be store or item or category(string)
        public QuantityCondition(T entity, int Quantity , string minmax)
        {
            this.entity = entity;
            this.Quantity = Quantity;
            this.minmax = minmax;
        }

        public override bool Evaluate(Store store, Dictionary<Item, int> basket)
        {
            int quantity = 0;
            String type = "";
            switch (entity)
            {
                case Store storeType:
                    type = $"store {store.StoreName}";
                    foreach (Item item in basket.Keys)
                        quantity += basket[item];
                    break;
                case string stringType:
                    type = $"category {stringType}";
                    foreach (Item item in basket.Keys)
                        if (item.Category.Equals(entity))
                            quantity += basket[item];
                    break;
                case Item itemType:
                    type = $"item {itemType.Name}";
                    if (basket.ContainsKey(itemType))
                        quantity = basket[itemType];
                    break;
                case ShoppingCart shopping:
                    type = $"shopping cart";
                    foreach (Item item in basket.Keys)
                        quantity += basket[item];
                    break;
                default:
                    throw new Exception("Category or Store or item are the one we can evaluate");
            }
            return Check(quantity, type);
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

        private bool Check(int q, string type)
        {
            switch (minmax)
            {
                case "min":
                    if (Quantity <= q)
                        return true;
                    message = $"The quantity of {type} is {q} while the minimum quantity is {Quantity}";
                    return false;
                case "max":
                    if (Quantity >= q)
                        return true;
                    message = $"The quantity of {type} is {q} while the maximum quantity is {Quantity}";
                    return false;
                default:
                    throw new Exception("Need to be one of this operators");
            }
        }

        public override string ToString()
        {
            string mn = "maximum";
            if (minmax.Equals("min"))
                mn = "minimum";
            switch (entity)
            {
                case Store storeType:
                    return $"({mn} quantity of {storeType.StoreName} is {Quantity})";
                case Item itemType:
                    return $"({mn} quantity of {itemType.Name} is {Quantity})";
                case string categoryType:
                    return $"({mn} quantity of {categoryType} is {Quantity})";
                default:
                    return "";
            }
        }
    }
    #endregion
    
    #region value
    public class ValueCondition<T> : Condition
    {
        public T entity;
        public double minPrice;
        public string minmax;

        // T can be store or category(string)
        public ValueCondition(T entity, double minPrice, string minmax)
        {
            this.entity = entity;
            this.minPrice = minPrice;
            this.minmax = minmax;
        }

        public override bool Evaluate(Store store, Dictionary<Item, int> basket)
        {
            double sum = 0;
            string type = "";
            switch (entity)
            {
                case Store storeType:
                    type = $"store {storeType.StoreName}";
                    foreach (Item item in basket.Keys)
                        sum += item.Price*basket[item];
                    break;
                case string stringType:
                    type = $"category {stringType}";
                    foreach (Item item in basket.Keys)
                        if (item.Category.Equals(entity))
                            sum += item.Price*basket[item];
                    break;
                case ShoppingBasket shopping:
                    foreach (Item item in basket.Keys)
                        sum += item.Price;
                    break;
                case Item item:
                    type = $"item {item.Name}";
                    foreach (Item i in basket.Keys)
                        if(i.ItemID == item.ItemID)
                            sum += i.Price*basket[i];
                    break;
                default:
                    throw new Exception("Category or Store are the one we can evaluate");
            }
            return Check(sum, type);
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

        private bool Check(double sum, string type)
        {
            switch (minmax)
            {
                case "min":
                    if (sum >= minPrice)
                        return true;
                    message = $"The price of {type} is {sum} while the minimum price is {minPrice}";
                    return false;
                case "max":
                    if (sum <= minPrice)
                        return true;
                    message = $"The price of {type} is {sum} while the maximum is price {minPrice}";
                    return false;
                default:
                    throw new Exception("Need to be one of this operators");
            }
        }
        
        public override string ToString()
        {
            string mn = "maximum";
            if (minmax.Equals("min"))
                mn = "minimum";
            switch (entity)
            {
                case Store storeType:
                    return $"({mn} purchase for {storeType.StoreName} is {minPrice}$)";
                case Item itemType:
                    return $"({mn} purchase for  {itemType.Name} is {minPrice}$)";
                case string categoryType:
                    return $"({mn} purchase for {categoryType} is {minPrice}$)";
                default:
                    return "";
            }
        }
    }
    #endregion
    
    #region time
    public class TimeCondition<T> : Condition
    {
        public T entity;
        public DateTime timing;
        public string beforeAfter;
        
        public TimeCondition(T entity, DateTime timing , string beforeAfter)
        {
            this.entity = entity;
            this.timing = timing;
            this.beforeAfter = beforeAfter;
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
                    return timing.Hour > DateTime.Now.Hour;
                case "after":
                    return timing.Hour <= DateTime.Now.Hour;
                default:
                    throw new Exception("Need to be one of this operators");
            }
        }
        public override string ToString()
        {
            string mn = "after";
            if (beforeAfter.Equals("before"))
                mn = "before";
            switch (entity)
            {
                case Store storeType:
                    return $"(Purchase for {storeType.StoreName} is {mn} {timing}$)";
                case Item itemType:
                    return $"(Purchase for {itemType.Name} is {mn} {timing}$)";
                case string categoryType:
                    return $"(Purchase for {categoryType} is {mn} {timing}$)";
                default:
                    return "";
            }
        }
    }
    #endregion
    
    #region condition conditional
    public class ConditioningCondition : ComplexCondition
    {

        public ConditioningCondition(Condition cond1, Condition result) : base(cond1, result)
        {
        }
        public override bool Evaluate(Store store, Dictionary<Item, int> basket)
        {
            if (cond1.Evaluate(store, basket))
            {
                bool output = cond2.Evaluate(store, basket);
                message = cond2.message;
                return output;
            }

            return true;
        }

        public override bool Equals(Condition eCondition)
        {
            switch (eCondition)
            {
                case ConditioningCondition cc2:
                    return cond1.Equals(cc2.cond1) && cond2.Equals(cc2.cond2);
                default:
                    return false;
            }
        }
        public override string ToString()
        {
            return $"(if {cond1}\nthan {cond2})";
        }
    }
    #endregion
    
    #region or condition
    public class OrCondition: ComplexCondition
    {
        public OrCondition(Condition cond1, Condition cond2) : base(cond1, cond2)
        {
        }

        public override bool Evaluate(Store store, Dictionary<Item, int> basket)
        {
            bool output;
            output = cond1.Evaluate(store, basket) || cond2.Evaluate(store, basket);
            if (cond1.message != null && cond2.message != null)
                message = "("+ cond1.message + "and" + cond2.message + ")";
            else if (cond1.message != null)
                message = "("+ cond1.message + ")";
            else if (cond2.message != null)
                message = "("+ cond2.message + ")";
            return output;
        }
        public override string ToString()
        {
            return $"({cond1} \nor {cond2})";
        }
    }
    #endregion

    #region and condition

    public class AndCondition : ComplexCondition
    {

        public AndCondition(Condition cond1, Condition cond2) : base(cond1, cond2)
        {
        }

        public override bool Evaluate(Store store, Dictionary<Item, int> basket)
        {
            bool output;
            output = cond1.Evaluate(store, basket) && cond2.Evaluate(store, basket);
            if (cond1.message != null && cond2.message != null)
                message = "(" + cond1.message + "and" + cond2.message + ")";
            return output;
        }

        public override string ToString()
        {
            return $"({cond1} \nand {cond2})";
        }
    }

    #endregion
    
    #region complex
    public class ComplexCondition: Condition
    {
        public Condition cond1;
        public Condition cond2;

        public ComplexCondition(Condition cond1, Condition cond2)
        {
            this.cond1 = cond1;
            this.cond2 = cond2;
        }
        public ComplexCondition(Condition cond1)
        {
            this.cond1 = cond1;
            this.cond2 = null;
        }
        public override bool Evaluate(Store store, Dictionary<Item, int> basket)
        {
            if (cond2 == null)
            {
                bool output = cond1.Evaluate(store, basket);
                message = cond1.message;
                return output;
            }
            return true;
        }

        public override bool Equals(Condition cond)
        {
            switch (cond)
            {
                case ComplexCondition cc1:
                    return cond1.Equals(cc1.cond1) && cond2.Equals(cc1.cond2);
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            return cond1.ToString();
        }
    }
    #endregion

}