using SadnaExpress.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.ExternalServices
{
    public class PaymentService : IPaymentService
    {
        bool isConnected = false;

        public PaymentService()
        {
            isConnected = true;
        }

        public bool cancel(double amount, string transactionDetails)
        {
            return true;
        }

        public bool Connect()
        {
            return isConnected;
        }

        public bool Pay(double amount, string transactionDetails)
        {
            return true;
        }
    }
}
