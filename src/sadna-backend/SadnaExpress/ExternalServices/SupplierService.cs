using SadnaExpress.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.ExternalServices
{
    public class SupplierService : ISupplierService
    {
        bool isConnected = false;

        public SupplierService()
        {
            isConnected = true;
        }

        public bool CancelOrder(string orderNum)
        {
            return true;
        }

        public bool Connect()
        {
            return isConnected;
        }

        public bool ShipOrder(string orderDetails, string userDetails)
        {
            return true;
        }

        public bool ValidateAddress(string address)
        {
            return true;
        }
    }
}
