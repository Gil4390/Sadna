namespace SadnaExpress.DomainLayer.Store
{
    public class PurchasePolicy : Policy
    {
        public class MinPricePurchasePolicy : PurchasePolicy
        {
            private int minPrice;

            public MinPricePurchasePolicy(int min)
            {
                this.minPrice = min;
            }

            public override bool IsPurchasePolicyTakesPlace(int price)
            {
                return price >= this.minPrice;
            }
        }

        public class MinStockPurchasePolicy : PurchasePolicy
        {
            private int minStock;

            public MinStockPurchasePolicy(int min)
            {
                this.minStock = min;
            }

            public override bool IsPurchasePolicyTakesPlace(int stock)
            {
                return stock >= this.minStock;
            }
        }

        public class NoPurchasePolicy : PurchasePolicy
        {
            public override bool IsPurchasePolicyTakesPlace(int n)
            {
                return true;
            }
        }

    }
}