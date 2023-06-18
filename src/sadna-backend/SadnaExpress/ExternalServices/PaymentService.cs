using SadnaExpress.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SadnaExpress.ServiceLayer.SModels;

namespace SadnaExpress.ExternalServices
{
    public class PaymentService : IPaymentService
    {
        private HttpClient client;
        private string address = null;

        public PaymentService(string adrs=null)
        {
            client = new HttpClient();
            address = adrs;
            if (adrs == null)
                address= ApplicationOptions.PaymentServiceURL;
        }

        public bool Cancel_Pay(double amount, int transaction_id)
        {
            var postContent = new Dictionary<string, string>
            {
                {"action_type","cancel_pay"},
                {"transaction_id",transaction_id.ToString()}
            };
            try
            {
                int result;
                if (int.TryParse((string)Send(postContent), out result))
                    return result==1;
                else
                    return false;
            }
            catch (Exception e)
            {
                throw new Exception("failed to access the payment service");
            }
        }

        public object Send(Dictionary<string, string> content)
        {
            try
            {
                var formData = new FormUrlEncodedContent(content);
                var responseTask = client.PostAsync(address, formData);
                var response = responseTask.Result;
                var responseContentTask = response.Content.ReadAsStringAsync();
                return responseContentTask.Result;
            }
            catch (Exception e)
            {
                throw new Exception("failed to access the payment service");
            }

        }

        public string Handshake()
        {
            var postContent = new Dictionary<string, string>
            {
                 {"action_type","handshake"},
            };
            var formData = new FormUrlEncodedContent(postContent);
            var responseTask = client.PostAsync(address, formData);
            var response = responseTask.Result;
            var responseContentTask = response.Content.ReadAsStringAsync();
            return responseContentTask.Result;  
        }

        public int Pay(double amount, SPaymentDetails transactionDetails)
        {
            var postContent = new Dictionary<string, string>
            {
                { "action_type", "pay" },
                { "card_number", transactionDetails.CardNumber},
                { "month", transactionDetails.Month},
                { "year", transactionDetails.Year },
                { "holder", transactionDetails.Holder },
                { "ccv", transactionDetails.Cvv },
                { "id", transactionDetails.Id },
            };
            try
            {
                int result;
                if (int.TryParse((string)Send(postContent), out result))
                    return result;
                else
                    return -1;
            }
            catch (Exception e)
            {
                throw new Exception("failed to access the payment service");
            }
            
        }
    }
}
