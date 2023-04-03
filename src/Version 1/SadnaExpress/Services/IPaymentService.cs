namespace SadnaExpress.Services
{
    public interface IPaymentService
    {
        bool Connect();
        bool ValidatePayment(string payment);
        void Pay(double amount, string payment);
    }
}