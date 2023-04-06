using SadnaExpress.ServiceLayer.ServiceObjects;
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

        public ResponseT<List<S_Store>> GetAllStoreInfo(int id)
        {
            return _realBridge.GetAllStoreInfo(id);
        }

        public Response AddItemToCart(int id, int itemID, int itemAmount)
        {
            return _realBridge.AddItemToCart(id, itemID, itemAmount);
        }

        public Response PurchaseCart(int id, string paymentDetails)
        {
            return _realBridge.PurchaseCart(id, paymentDetails);
        }

        public Response CreateStore(int id, string storeName)
        {
            return _realBridge.CreateStore(id, storeName);
        }

        public Response WriteReview(int id, int itemID, string review)
        {
            return _realBridge.WriteReview(id, itemID, review);
        }

        public Response RateItem(int id, int itemID, int score)
        {
            return _realBridge.RateItem(id, itemID, score);
        }

        public Response WriteMessageToStore(int id, int storeID, string message)
        {
            return _realBridge.WriteMessageToStore(id, storeID, message);
        }

        public Response ComplainToAdmin(int id, string message)
        {
            return _realBridge.ComplainToAdmin(id, message);
        }

        public Response GetPurchasesInfo(int id)
        {
            return _realBridge.GetPurchasesInfo(id);
        }

        public Response AddItemToStore(int id, int storeID, string itemName, string itemCategory, float itemPrice)
        {
            return _realBridge.AddItemToStore(id, storeID, itemName, itemCategory, itemPrice);
        }

        public Response RemoveItemFromStore(int id, int storeID, int itemID)
        {
            return _realBridge.RemoveItemFromStore(id, storeID, itemID);
        }

        public Response AppointStoreOwner(int id, int storeID, int newUserID)
        {
            return _realBridge.AppointStoreOwner(id, storeID, newUserID);
        }

        public Response AppointStoreManager(int id, int storeID, int newUserID)
        {
            return _realBridge.AppointStoreManager(id, storeID, newUserID);
        }

        public Response RemoveStoreOwner(int id, int storeID, int userID)
        {
            return _realBridge.RemoveStoreOwner(id, storeID, userID);
        }

        public Response RemovetStoreManager(int id, int storeID, int userID)
        {
            return _realBridge.RemovetStoreManager(id, storeID, userID);
        }

        public Response CloseStore(int id, int storeID)
        {
            return _realBridge.CloseStore(id, storeID);
        }

        public Response ReopenStore(int id, int storeID)
        {
            return _realBridge.ReopenStore(id, storeID);
        }

        public ResponseT<List<S_Member>> GetEmployeeInfoInStore(int id, int storeID)
        {
            return _realBridge.GetEmployeeInfoInStore(id, storeID);
        }

        public Response GetPurchasesInfo(int id, int storeID)
        {
            return _realBridge.GetPurchasesInfo(id, storeID);
        }

        public Response DeleteStore(int id, int storeID)
        {
            return _realBridge.DeleteStore(id, storeID);
        }

        public Response DeleteMember(int id, int userID)
        {
            return _realBridge.DeleteMember(id, userID);
        }
    }
}
