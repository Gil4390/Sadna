using System;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store.Policy;

public abstract class DiscountPolicy
    {
        public abstract Dictionary<Item, KeyValuePair<double, DateTime>> calculate(Store store,
            Dictionary<Item, int> basket);
    }

    public class SimpleDiscount<T> : DiscountPolicy
    {
        private T level;
        private int percent;
        private readonly DateTime startDate;
        private readonly DateTime endDate;

        // T can be store or item or category(string)
        public SimpleDiscount(T level, int percent, DateTime startDate, DateTime endDate)
        {
            this.level = level;
            this.percent = percent;
            this.startDate = startDate;
            this.endDate = endDate;
        }

        public override Dictionary<Item, KeyValuePair<double, DateTime>> calculate(Store store,
            Dictionary<Item, int> basket)
        {
            if (startDate <= DateTime.Now && endDate >= DateTime.Now)
            {
                Dictionary<Item, KeyValuePair<double, DateTime>> output =
                    new Dictionary<Item, KeyValuePair<double, DateTime>>();
                switch (level)
                {
                    case Store storeType:
                        if (!store.Equals(storeType))
                            return null;
                        foreach (Item item in basket.Keys)
                            output.Add(item, new KeyValuePair<double, DateTime>((100 - percent) * item.Price / 100, endDate));
                        return output;
                    case string categoryType:
                        foreach (Item item in basket.Keys)
                            if (item.Category.Equals(categoryType))
                                output.Add(item,
                                    new KeyValuePair<double, DateTime>((100 - percent) * item.Price / 100, endDate));
                        return output;
                    case Item item:
                        return new Dictionary<Item, KeyValuePair<double, DateTime>>
                        {
                            { item, new KeyValuePair<double, DateTime>((100 - percent) * item.Price / 100, endDate) }
                        };
                    default:
                        throw new Exception("Category or Store or item are the one we can evaluate");
                }
            }

            return null;
        }
    }

    public class ConditionalDiscount<T> : DiscountPolicy
    {
        private Condition cond;
        private DiscountPolicy discountPolicy;

        public ConditionalDiscount(Condition cond, DiscountPolicy discountPolicy)
        {
            this.cond = cond;
            this.discountPolicy = discountPolicy;
        }

        public override Dictionary<Item, KeyValuePair<double, DateTime>> calculate(Store store,
            Dictionary<Item, int> basket)
        {
            if (cond.Evaluate(store, basket))
                return discountPolicy.calculate(store, basket);
            return null;
        }
    }

    public class XorDiscount<T> : DiscountPolicy
    {
        private DiscountPolicy discountPolicy1;
        private DiscountPolicy discountPolicy2;

        public XorDiscount(DiscountPolicy discountPolicy1, DiscountPolicy discountPolicy2)
        {
            this.discountPolicy1 = discountPolicy1;
            this.discountPolicy2 = discountPolicy2;
        }

        public override Dictionary<Item, KeyValuePair<double, DateTime>> calculate(Store store,
            Dictionary<Item, int> basket)
        {
            double discount1 = 0;
            double discount2 = 0;
            Dictionary<Item, KeyValuePair<double, DateTime>> discountDict1 = discountPolicy1.calculate(store, basket);
            Dictionary<Item, KeyValuePair<double, DateTime>> discountDict2 = discountPolicy2.calculate(store, basket);
            foreach (Item item in discountDict1.Keys)
                discount1 = item.Price - discountDict1[item].Key; //calculate how much discount (in NIS)
            foreach (Item item in discountDict2.Keys)
                discount2 = item.Price - discountDict2[item].Key; //calculate how much discount (in NIS)
            if (discount1 > discount2) //with who I pay less
                return discountDict1;
            return discountDict2;
        }
    }

    public class AndDiscount<T> : DiscountPolicy
    {
        private Condition cond1;
        private Condition cond2;
        private DiscountPolicy discountPolicy;

        public AndDiscount(Condition cond1, Condition cond2, DiscountPolicy discountPolicy)
        {
            this.cond1 = cond1;
            this.cond2 = cond2;
            this.discountPolicy = discountPolicy;
        }

        public override Dictionary<Item, KeyValuePair<double, DateTime>> calculate(Store store,
            Dictionary<Item, int> basket)
        {
            if (cond1.Evaluate(store, basket) && (cond2.Evaluate(store, basket)))
                return discountPolicy.calculate(store, basket);
            return null;
        }
    }

    public class OrDiscount<T> : DiscountPolicy
    {
        private Condition cond1;
        private Condition cond2;
        private DiscountPolicy discountPolicy;

        public OrDiscount(Condition cond1, Condition cond2, DiscountPolicy discountPolicy)
        {
            this.cond1 = cond1;
            this.cond2 = cond2;
            this.discountPolicy = discountPolicy;
        }

        public override Dictionary<Item, KeyValuePair<double, DateTime>> calculate(Store store,
            Dictionary<Item, int> basket)
        {
            if (cond1.Evaluate(store, basket) || (cond2.Evaluate(store, basket)))
                return discountPolicy.calculate(store, basket);
            return null;
        }
    }

    public class MaxDiscount<T> : DiscountPolicy
    {
        private DiscountPolicy discountPolicy1;
        private DiscountPolicy discountPolicy2;

        public MaxDiscount(DiscountPolicy discountPolicy1, DiscountPolicy discountPolicy2)
        {
            this.discountPolicy1 = discountPolicy1;
            this.discountPolicy2 = discountPolicy2;
        }

        public override Dictionary<Item, KeyValuePair<double, DateTime>> calculate(Store store,
            Dictionary<Item, int> basket)
        {
            Dictionary<Item, KeyValuePair<double, DateTime>> output =
                new Dictionary<Item, KeyValuePair<double, DateTime>>();
            Dictionary<Item, KeyValuePair<double, DateTime>> discountDict1 = discountPolicy1.calculate(store, basket);
            Dictionary<Item, KeyValuePair<double, DateTime>> discountDict2 = discountPolicy2.calculate(store, basket);
            foreach (Item item in discountDict1.Keys)
            {
                if (discountDict2.ContainsKey(item) && discountDict1[item].Key > discountDict2[item].Key) //the price of discount 2 is lower
                    output.Add(item, discountDict2[item]);
                else
                    output.Add(item, discountDict1[item]);
            }
            foreach (Item item in discountDict2.Keys)
                if (!output.ContainsKey(item))
                    output.Add(item, discountDict2[item]);
            return output;
        }
    }

    public class AddDiscount<T> : DiscountPolicy
    {
        private DiscountPolicy discountPolicy1;
        private DiscountPolicy discountPolicy2;

        public AddDiscount(DiscountPolicy discountPolicy1, DiscountPolicy discountPolicy2)
        {
            this.discountPolicy1 = discountPolicy1;
            this.discountPolicy2 = discountPolicy2;
        }

        public override Dictionary<Item, KeyValuePair<double, DateTime>> calculate(Store store,
            Dictionary<Item, int> basket)
        {
            Dictionary<Item, KeyValuePair<double, DateTime>> output =
                new Dictionary<Item, KeyValuePair<double, DateTime>>();
            Dictionary<Item, KeyValuePair<double, DateTime>> discountDict1 = discountPolicy1.calculate(store, basket);
            Dictionary<Item, KeyValuePair<double, DateTime>> discountDict2 = discountPolicy2.calculate(store, basket);
            foreach (Item item in discountDict1.Keys)
            {
                if (discountDict2.ContainsKey(item)) //double discount
                {
                    DateTime earlyDate = discountDict2[item].Value;
                    if (earlyDate > discountDict1[item].Value)
                        earlyDate = discountDict1[item].Value;
                    double percent1 = -(discountDict1[item].Key * 100 / item.Price - 100); // get percent
                    double percent2 = -(discountDict2[item].Key * 100 / item.Price - 100);
                    double newPrice = (100 - (percent1 + percent2)) * item.Price / 100; 
                    output.Add(item, new KeyValuePair<double, DateTime>(newPrice, earlyDate));
                }
                else
                    output.Add(item, discountDict1[item]);
            }

            foreach (Item item in discountDict2.Keys)
                if (!output.ContainsKey(item))
                    output.Add(item, discountDict2[item]);
            return output;
        }
    }
    class DiscountPolicyTree : DiscountPolicy
    {
        private DiscountPolicy root;

        public DiscountPolicyTree(DiscountPolicy root)
        {   
            this.root = root;
        }

        public override Dictionary<Item, KeyValuePair<double, DateTime>> calculate(Store store,
            Dictionary<Item, int> basket)
        {
            return root.calculate(store, basket);
        }
    }