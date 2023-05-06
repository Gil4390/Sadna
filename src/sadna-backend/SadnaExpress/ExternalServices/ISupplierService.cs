namespace SadnaExpress.Services
{
    public interface ISupplierService
    {
        bool Connect();

        bool ValidateAddress(string address);

        bool ShipOrder(string orderDetails, string userDetails);

        bool CancelOrder(string orderNum);

    }
}