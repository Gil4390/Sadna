
using SadnaExpress.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpressTests
{
    public class Mocks
    {
        public class Mock_SupplierService : SupplierService
        {
            int shipmentNum = 0;
            bool isConnected = false;

            public Mock_SupplierService()
            {
                isConnected = true;
            }
            public void cancelOrder(string orderNum)
            {

            }

            public bool connect()
            {
                return isConnected;
            }

            public string shipOrder(string address)
            {
                shipmentNum++;
                return "test " + shipmentNum;
            }

            public bool validateAddress(string address)
            {
                return true;
            }
        }


        public class Mock_PaymentService : PaymentService
        {   
            bool isConnected = false;

            public Mock_PaymentService()
            {
                isConnected = true;
            }
            public bool connect()
            {
                return isConnected;
            }

            public void pay(double amount, string payment)
            {
                
            }

            public bool ValidatePayment(string payment)
            {
                return true;
            }
        }
    }
}
