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
    public class SupplierService : ISupplierService
    {
        private HttpClient client;
        string address = "https://php-server-try.000webhostapp.com/";

        public SupplierService()
        {
            client = new HttpClient();
        }

        public bool Cancel_Supply(string transaction_id)
        {
            var postContent = new Dictionary<string, string>
            {
                {"action_type","cancel_supply"},
                {"transaction_id",transaction_id}
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

        public bool Handshake()
        {
            var postContent = new Dictionary<string, string>
            {
                {"action_type","handshake"},
            };
            return (string)Send(postContent)=="OK";
        }

        public int Supply(SSupplyDetails userDetails)
        {
            var postContent = new Dictionary<string, string>
            { 
                {"action_type","supply"},
                {"name",userDetails.Name},
                {"address",userDetails.Address },
                {"city",userDetails.City},
                {"country",userDetails.Country},
                {"zip",userDetails.Zip}
            };
            return (int)Send(postContent);
        }


    }
}
