using SadnaExpress.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.ServiceLayer
{
    public class ProxyBridge : ITradingSystem
    {
        private ITradingSystem _realBridge;

        public void SetBridge(ITradingSystem Implemantation)
        {
            _realBridge = Implemantation;
        }

        public bool CheckPaymentConnection()
        {
            return _realBridge.CheckPaymentConnection();
        }

        public bool CheckSupplierConnection()
        {
            return _realBridge.CheckSupplierConnection();
        }

        public void CleanUp()
        {
            _realBridge.CleanUp();
        }

        public ResponseT<int> Enter()
        {
            return _realBridge.Enter();
        }

        public Response Register(int id, string email, string firstName, string lastName, string password)
        {
            return _realBridge.Register(id, email, firstName, lastName, password);
        }

        public void SetPaymentService(IPaymentService paymentService)
        {
            _realBridge.SetPaymentService(paymentService);
        }

        public void SetSupplierService(ISupplierService supplierService)
        {
            _realBridge.SetSupplierService(supplierService);
        }

        public ResponseT<int> Login(int id, string email, string password)
        {
            return _realBridge.Login(id, email, password);
        }

        public ResponseT<bool> PlacePayment(string transactionDetails)
        {
            return _realBridge.PlacePayment(transactionDetails);
        }

        public ResponseT<bool> PlaceSupply(string orderDetails, string userDetails)
        {
            return _realBridge.PlaceSupply(orderDetails, userDetails);
        }

        public int GetMaximumWaitServiceTime()
        {
            return _realBridge.GetMaximumWaitServiceTime();
        }

        public ResponseT<bool> InitializeTradingSystem(int id)
        {
            return _realBridge.InitializeTradingSystem(id);
        }

        public ResponseT<int> Logout(int id)
        {
            return _realBridge.Logout(id);
        }
    }
}
