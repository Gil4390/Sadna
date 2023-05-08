using SadnaExpress.DomainLayer.Store.Policy;

namespace SadnaExpress.ServiceLayer.SModels
{
    public class SPolicy
    {
        private int policyID;
        public int PolicyID{get=>policyID;}
        private string policyRule;
        public string PolicyRule { get => policyRule; }
        private bool active;
        public bool Active { get => active; }
        

        public SPolicy(DiscountPolicy discountPolicy, bool active)
        {
            policyID = discountPolicy.ID;
            this.active = active;
            policyRule = discountPolicy.ToString();
        }
        
        public SPolicy(int condId, string cond, bool active)
        {
            policyID = condId;
            this.active = active;
            policyRule = cond;
        }
    }
}