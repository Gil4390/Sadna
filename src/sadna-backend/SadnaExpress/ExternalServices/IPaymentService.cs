using System.Collections.Generic;
using SadnaExpress.ServiceLayer.SModels;

namespace SadnaExpress.Services
{
    public interface IPaymentService
    {
        object Send(Dictionary<string, string> content);
        string Handshake();
        int Pay(double amount, SPaymentDetails transactionDetails);
        bool Cancel_Pay(double amount, int transaction_id);
    }
}