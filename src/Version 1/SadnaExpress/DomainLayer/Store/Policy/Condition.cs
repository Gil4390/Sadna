using System;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store.Policy;

public abstract class Condition
    {
        public abstract bool Evaluate(Store store, Dictionary<Item, int> basket);
    }

    public class MinValueCondition<T> : Condition
    {
        private T level;
        private double minPrice;

        // T can be store or category(string)
        public MinValueCondition(T level, double minPrice)
        {
            this.level = level;
            this.minPrice = minPrice;
        }

        public override bool Evaluate(Store store, Dictionary<Item, int> basket)
        {
            double sum = 0;
            switch (level)
            {
                case Store storeType:
                    foreach (Item item in basket.Keys)
                        sum += item.Price;
                    if (sum >= minPrice)
                        return true;
                    return false;
                case string stringType:
                    foreach (Item item in basket.Keys)
                        if (item.Category.Equals(level))
                            sum += item.Price;
                    if (sum >= minPrice)
                        return true;
                    return false;
                default:
                    throw new Exception("Category or Store are the one we can evaluate");
            }
        }

        public class MinQuantityCondition<T> : Condition
        {
            private T level;
            private int minQuantity;

            // T can be store or item or category(string)
            public MinQuantityCondition(T level, int minQuantity)
            {
                this.level = level;
                this.minQuantity = minQuantity;
            }

            public override bool Evaluate(Store store, Dictionary<Item, int> basket)
            {
                double quantity = 0;
                switch (level)
                {
                    case Store storeType:
                        foreach (Item item in basket.Keys)
                            quantity += basket[item];
                        if (quantity >= minQuantity)
                            return true;
                        return false;
                    case string stringType:
                        foreach (Item item in basket.Keys)
                            if (item.Category.Equals(level))
                                quantity += basket[item];
                        if (quantity >= minQuantity)
                            return true;
                        return false;
                    case Item itemType:
                        if (basket.ContainsKey(itemType))
                            return basket[itemType] >= minQuantity;
                        return false;
                    default:
                        throw new Exception("Category or Store or item are the one we can evaluate");
                }
            }
        }
    }