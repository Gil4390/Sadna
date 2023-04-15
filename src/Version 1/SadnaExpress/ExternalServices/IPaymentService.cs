namespace SadnaExpress.Services
{
    public interface IPaymentService
    {
        bool Connect();
        bool ValidatePayment(string transactionDetails);
        void Pay(double amount, string payment);
    }
}