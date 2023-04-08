namespace SadnaExpress.DomainLayer.Store
{
    public class Policy
    {
        public virtual bool IsPurchasePolicyTakesPlace(int n)
        {
            return false;
        }

        public virtual bool HasDiscount()
        {
            return false;
        }

        public virtual newPair<int, int> GetDiscount()
        {
            return null;
        }
    }
}