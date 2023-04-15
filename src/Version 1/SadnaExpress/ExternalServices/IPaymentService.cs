namespace SadnaExpress.Services
{
    public interface IPaymentService
    {
        bool Connect();
        bool Pay(double amount, string transactionDetails);
    }
}