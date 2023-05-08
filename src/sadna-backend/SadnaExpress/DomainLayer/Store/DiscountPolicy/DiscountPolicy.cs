using System;
using System.Collections.Generic;
using System.Linq;

namespace SadnaExpress.DomainLayer.Store.DiscountPolicy
{
    public abstract class DiscountPolicy
    {
        public int ID;
        public abstract Dictionary<Item, KeyValuePair<double, DateTime>> calculate(Store store,
            Dictionary<Item, int> basket);

        public abstract override string ToString();
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
            Dictionary<Item, KeyValuePair<double, DateTime>> output =
                             new Dictionary<Item, KeyValuePair<double, DateTime>>();
            if (startDate <= DateTime.Now && endDate >= DateTime.Now)
            {
                
                switch (level)
                {
                    case Store storeType:
                        if (!store.Equals(storeType))
                            return output;
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

            return output;
        }
        public override string ToString()
        {
            switch (level)
            {
                case Store storeType:
                    return $"(Store {storeType.StoreName} have {percent} from {startDate} until {endDate})";
                case Item itemType:
                    return $"(Item {itemType.Name} have {percent}  from {startDate} until {endDate})";
                case string categoryType:
                    return $"(Item {categoryType} have {percent}  from {startDate} until {endDate})";
                default:
                    return "";
            }
        }
    }

    public class ConditionalDiscount : DiscountPolicy
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
            return new Dictionary<Item, KeyValuePair<double, DateTime>>();
        }
        public override string ToString()
        {
            return $"(If {cond} \n=> {discountPolicy})";
        }
    }

    public abstract class LogicalPolicy : DiscountPolicy
    {
        public Condition cond1;
        public Condition Cond1 {get=>cond1;}

        public Condition cond2;
        public Condition Cond2 {get=>cond2;}
        public DiscountPolicy discountPolicy;
        public DiscountPolicy DiscountPolicy {get=>discountPolicy;}


        public LogicalPolicy(Condition cond1, Condition cond2, DiscountPolicy discountPolicy)
        {
            this.cond1 = cond1;
            this.cond2 = cond2;
            this.discountPolicy = discountPolicy;
        }
    }
    
    public class XorDiscount: LogicalPolicy
    {
        public XorDiscount(Condition cond1, Condition cond2, DiscountPolicy discountPolicy) : base(cond1, cond2, discountPolicy)
        {
        }
        public override Dictionary<Item, KeyValuePair<double, DateTime>> calculate(Store store,
            Dictionary<Item, int> basket)
        {
            bool oneOfThem = cond1.Evaluate(store, basket) || cond2.Evaluate(store, basket);
            bool bothOfThem = cond1.Evaluate(store, basket) && cond2.Evaluate(store, basket);
            if (oneOfThem && !bothOfThem)
                return discountPolicy.calculate(store, basket);
            return new Dictionary<Item, KeyValuePair<double, DateTime>>();
        }

        public override string ToString()
        {
            return $"(If {cond1}\nXor {cond2}\n=>{discountPolicy})";
        }
    }

    public class AndDiscount : LogicalPolicy
    {
        private Condition cond1;
        private Condition cond2;
        private DiscountPolicy discountPolicy;

        public AndDiscount(Condition cond1, Condition cond2, DiscountPolicy discountPolicy) : base(cond1, cond2, discountPolicy)
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
            return new Dictionary<Item, KeyValuePair<double, DateTime>>();
        }
        public override string ToString()
        {
            return $"(If {cond1}\nAnd {cond2}\n=>{discountPolicy})";
        }
    }

    public class OrDiscount : LogicalPolicy
    {
        private Condition cond1;
        private Condition cond2;
        private DiscountPolicy discountPolicy;

        public OrDiscount(Condition cond1, Condition cond2, DiscountPolicy discountPolicy):base(cond1, cond2, discountPolicy)
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
            return new Dictionary<Item, KeyValuePair<double, DateTime>>();
        }
        public override string ToString()
        {
            return $"(If {cond1}\nOr {cond2}\n=>{discountPolicy})";
        }
    }
    
    public class MaxDiscount : DiscountPolicy
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
            double sum1 = 0;
            double sum2 = 0;
            foreach (Item item in basket.Keys)
            {
                if (discountDict1 != null && discountDict1.ContainsKey(item))
                    sum1 += discountDict1[item].Key * basket[item];
                else
                    sum1 += item.Price * basket[item];
                if (discountDict2 != null && discountDict2.ContainsKey(item))
                    sum2 += discountDict2[item].Key * basket[item];
                else
                    sum2 += item.Price * basket[item];
            }
            if (sum1 > sum2)
                return discountDict2;
            return discountDict1;
        }
        public override string ToString()
        {
            return $"(Max between\n{discountPolicy1}\nto {discountPolicy2})";
        }
    }

    public class AddDiscount : DiscountPolicy
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
        public override string ToString()
        {
            return $"(Add\n{discountPolicy1}\n{discountPolicy2})";
        }
    }

    public class DiscountPolicyTree : DiscountPolicy
    {
        private List<DiscountPolicy> roots;
        public List<DiscountPolicy> Roots {get => roots;}

        public DiscountPolicyTree(DiscountPolicy root)
        {
            roots = new List<DiscountPolicy>{root};
        }

        public void AddPolicy(DiscountPolicy discountPolicy)
        {
            roots.Add(discountPolicy);
        }

        public override Dictionary<Item, KeyValuePair<double, DateTime>> calculate(Store store,
            Dictionary<Item, int> basket)
        {
            Dictionary<Item, KeyValuePair<double, DateTime>> output = new Dictionary<Item, KeyValuePair<double, DateTime>>();
            foreach (DiscountPolicy root in roots)
            {
                Dictionary<Item, KeyValuePair<double, DateTime>> rootDict = root.calculate(store, basket);
                foreach (Item item in basket.Keys)
                {
                    if (output.ContainsKey(item) && rootDict.ContainsKey(item))
                    {
                        if (output[item].Key > rootDict[item].Key)
                            output[item] = rootDict[item];
                    }
                    else if (rootDict.ContainsKey(item))
                        output.Add(item, rootDict[item]);
                }
            }
            return output;
        }

        public void RemovePolicy(DiscountPolicy discountPolicy)
        {
            if (!roots.Remove(discountPolicy))
                throw new Exception("policy not found");
        }
        
        public override string ToString()
        {
            string output = "";
            int i = 1;
            foreach (DiscountPolicy policy in roots)
            {
                output += $"{i}. {policy}";
                i++;
            }
            return output;
        }
    }
}