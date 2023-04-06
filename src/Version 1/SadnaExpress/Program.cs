using System;
using System.IO;
using System.Threading;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using SadnaExpress.Services;

namespace SadnaExpress
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Server Started!");
            Console.WriteLine("In order to login enter: Admin Admin");
            Thread serverThread = new Thread(delegate()
            {
                Server myserver = new Server("127.0.0.1", 10011,new Mock_SupplierService(), new Mock_PaymentService());
            });
            serverThread.Start();
        }

        private class Mock_SupplierService : ISupplierService
        {
            int shipmentNum = 0;
            bool isConnected = false;

            public bool Connect()
            {
                throw new NotImplementedException();
            }

            public bool ValidateAddress(string address)
            {
                throw new NotImplementedException();
            }

            public string ShipOrder(string address)
            {
                throw new NotImplementedException();
            }

            public void CancelOrder(string orderNum)
            {
                throw new NotImplementedException();
            }

            public bool ShipOrder(string orderDetails, string userDetails)
            {
                return true;
            }
        }

        private class Mock_PaymentService : IPaymentService
        {
            public bool Connect()
            {
                throw new NotImplementedException();
            }

            public bool ValidatePayment(string payment)
            {
                throw new NotImplementedException();
            }

            public void Pay(double amount, string payment)
            {
                throw new NotImplementedException();
            }
        }
    }
}