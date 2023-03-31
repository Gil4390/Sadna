using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SadnaExpress.Services
{
    public interface PaymentService
    {

        bool connect();
        bool ValidatePayment(string payment);
        void pay(double amount, string payment);
    }
}
