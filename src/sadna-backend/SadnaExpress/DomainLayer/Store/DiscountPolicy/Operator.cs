using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store.DiscountPolicy
{

    public abstract class Operator
    {
        public abstract bool Calculate(Condition cond1, Condition cond2, Store store, Dictionary<Item, int> basket);
    }

    public class OrOperator : Operator
    {
        public override bool Calculate(Condition cond1, Condition cond2, Store store, Dictionary<Item, int> basket)
        {
            return cond1.Evaluate(store, basket) | (cond2 != null && cond2.Evaluate(store, basket));
        }
    }

    public class AndOperator : Operator
    {
        public override bool Calculate(Condition cond1, Condition cond2, Store store, Dictionary<Item, int> basket)
        {
            return cond1.Evaluate(store, basket) & (cond2 != null && cond2.Evaluate(store, basket));
        }
    }
}