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
        private string type;
        public string Type { get => type; }


        public SPolicy(DiscountPolicy discountPolicy, bool active , string t)
        {
            policyID = discountPolicy.ID;
            this.active = active;
            policyRule = discountPolicy.ToString();
            type = t;
        }
        
        public SPolicy(int condId, string cond, bool active, string t)
        {
            policyID = condId;
            this.active = active;
            policyRule = cond;
            type = t;
        }
    }
}