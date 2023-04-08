namespace SadnaExpress.DomainLayer.Store
{
    public class DiscountPolicy : Policy
    {
        

        public class ItemDiscount : DiscountPolicy
        {
            //private int itemId;
            private int stockD;
            private int discount;

            public ItemDiscount(int stockD, int discount)
            {
                this.stockD = stockD;
                this.discount = discount;
            }
            public override bool HasDiscount()
            {
                return true;
            }

            public override ourNewPair<int, int> GetDiscount()
            {
                return new ourNewPair<int, int>(stockD, discount);
            }
        }

        public class NoDiscount : DiscountPolicy
        {
            public override bool HasDiscount()
            {
                return false;
            }
        }
    }
}