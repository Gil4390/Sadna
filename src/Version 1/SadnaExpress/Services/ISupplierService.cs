namespace SadnaExpress.Services
{
    public interface ISupplierService
    {
        bool Connect();

        bool ValidateAddress(string address);

        string ShipOrder(string address);

        void CancelOrder(string orderNum);

    }
}