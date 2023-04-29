namespace SadnaExpress.Services
{
    public interface ISupplierService
    {
        bool Connect();

        bool ValidateAddress(string address);

        bool ShipOrder(string orderDetails, string userDetails);

        void CancelOrder(string orderNum);

    }
}