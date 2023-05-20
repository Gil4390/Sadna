using System.Web.UI;

namespace SadnaExpress.ServiceLayer.SModels
{
    public class SPaymentDetails
    {
        private string cardNumber;
        
        public string CardNumber{get=>cardNumber;}
        
        private string month;
        
        public string Month{get=>month;}
        
        private string year;
        
        public string Year{get=>year;}
        
        private string holder;
        
        public string Holder{get=>holder;}
        
        private string cvv;
        
        public string Cvv{get=>cvv;}
        
        private string id;
        
        public string Id{get=>id;}

        public SPaymentDetails(string cardNumber, string month, string year, string holder, string cvv, string id)
        {
            this.cardNumber = cardNumber;
            this.month = month;
            this.year = year;
            this.holder = holder;
            this.cvv = cvv;
            this.id = id;
        }

        public bool ValidationSettings()
        {
            return cardNumber != "-" && month != "-" && year != "-" && holder != "-" &&
                   cvv != "-" && id != "-";
        }
        
    }
    
}