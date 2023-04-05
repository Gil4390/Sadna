using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SadnaExpress.Services;

namespace SadnaExpressTests
{
    public class Mocks
    {
        public class Mock_SupplierService : ISupplierService
        {
            int shipmentNum = 0;
            bool isConnected = false;

            public Mock_SupplierService()
            {
                isConnected = true;
            }

            public virtual void CancelOrder(string orderNum)
            {

            }

            public virtual bool Connect()
            {
                return isConnected;
            }


            public virtual string ShipOrder(string address)
            {
                shipmentNum++;
                return "test " + shipmentNum;
            }


            public virtual bool ValidateAddress(string address)
            {
                return true;
            }
        }


        public class Mock_PaymentService : IPaymentService
        {
            bool isConnected = false;

            public Mock_PaymentService()
            {
                isConnected = true;
            }

            public virtual bool Connect()
            {
                return isConnected;
            }


            public virtual void Pay(double amount, string payment)
            {

            }

            public virtual bool ValidatePayment(string payment)
            {
                return true;
            }
        }
    }
}
