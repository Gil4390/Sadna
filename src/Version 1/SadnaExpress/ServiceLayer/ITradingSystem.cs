using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;
using SadnaExpress.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.ServiceLayer
{
    public interface ITradingSystem
    {
        ResponseT<int> Enter();

        ResponseT<int> Login(int id, string email, string password);

        ResponseT<int> Logout(int id);

        bool CheckSupplierConnection();

        bool CheckPaymentConnection();

        Response Register(int id, string email, string firstName, string lastName, string password);

        void CleanUp();

        void SetPaymentService(IPaymentService paymentService);

        void SetSupplierService(ISupplierService supplierService);

        ResponseT<bool> PlacePayment(string transactionDetails);

        ResponseT<bool> PlaceSupply(string orderDetails, string userDetails);

        int GetMaximumWaitServiceTime();

        ResponseT<bool> InitializeTradingSystem(int id);

        ResponseT<List<S_Store>> GetAllStoreInfo(int id);

        Response AddItemToCart(int id, int itemID, int itemAmount);

        Response PurchaseCart(int id, string paymentDetails);

        Response CreateStore(int id, string storeName);

        Response WriteReview(int id, int itemID, string review);

        Response RateItem(int id, int itemID, int score);

        Response WriteMessageToStore(int id, int storeID, string message);

        Response ComplainToAdmin(int id, string message);

        Response GetPurchasesInfo(int id);

        Response AddItemToStore(int id, int storeID, string itemName, string itemCategory, float itemPrice);

        Response RemoveItemFromStore(int id, int storeID, int itemID);

        Response AppointStoreOwner(int id, int storeID, int newUserID);

        Response AppointStoreManager(int id, int storeID, int newUserID);

        Response RemoveStoreOwner(int id, int storeID, int userID);

        Response RemovetStoreManager(int id, int storeID, int userID);

        Response CloseStore(int id, int storeID);

        Response ReopenStore(int id, int storeID);

        ResponseT<List<S_Member>> GetEmployeeInfoInStore(int id, int storeID);

        Response GetPurchasesInfo(int id, int storeID);

        Response DeleteStore(int id, int storeID);

        Response DeleteMember(int id, int userID);




    }
        
}
