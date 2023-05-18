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
        string address = "https://php-server-try.000webhostapp.com/";

        public PaymentService()
        {
            client = new HttpClient();
        }

        public bool Cancel_Pay(double amount, int transaction_id)
        {
            var postContent = new Dictionary<string, string>
            {
                {"action_type","cancel_pay"},
                {"transaction_id",transaction_id.ToString()}
            };
            return (int)Send(postContent)==1;
        }

        public object Send(Dictionary<string, string> content)
        {
            
            // Convert the dictionary to form URL-encoded content
            var formData = new FormUrlEncodedContent(content);

            // Send the HTTP POST request
            var responseTask = client.PostAsync(address, formData);

            // Wait for the response
            var response = responseTask.Result;

            // Read the response content as a string
            var responseContentTask = response.Content.ReadAsStringAsync();
            return responseContentTask.Id;
            
        }

        public string Handshake()
        {
            var postContent = new Dictionary<string, string>
            {
                {"action_type","handshake"},
            };
            return (string)Send(postContent);
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
            };
            return (int)Send(postContent);
        }
    }
}
