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
        string address = ApplicationOptions.SupplierServiceURL;

        public SupplierService(string adrs = null)
        {
            client = new HttpClient();
            address = adrs;
            if (adrs == null)
                address = ApplicationOptions.SupplierServiceURL;

        }

        public bool Cancel_Supply(string transaction_id)
        {
            var postContent = new Dictionary<string, string>
            {
                {"action_type","cancel_supply"},
                {"transaction_id",transaction_id}
            };
            try
            {
                return (int)Send(postContent)==1;
            }
            catch (Exception e)
            {
                throw new Exception("failed to access the supply service");
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
                throw new Exception("failed to access the supply service");
            }
        }

        public string Handshake()
        {
            try
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
            catch (Exception e)
            {
                throw new Exception("failed to access the supply service");
            }
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
            try
            {
                return int.Parse((string)Send(postContent));
            }
            catch (Exception e)
            {
                throw new Exception("failed to access the supply service");
            }
        }


    }
}
