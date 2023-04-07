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
        ResponseT<int> Enter(); //1.1
        Response Exit(int id); //1.2
        Response Register(int id, string email, string firstName, string lastName, string password); //1.3
        ResponseT<int> Login(int id, string email, string password); //1.4
        ResponseT<List<S_Store>> GetAllStoreInfo(int id); //2.1
        // 2.2
        ResponseT<List<S_Item>> GetItemsByName(int id, string itemName);
        ResponseT<List<S_Item>> GetItemsByCategory(int id, string category);
        ResponseT<List<S_Item>> GetItemsByKeysWord(int id, string keyWords);
        ResponseT<List<S_Item>> GetItemsByPrices(int id, int minPrice, int maxPrice);
        ResponseT<List<S_Item>> GetItemsByItemRating(int id, int rating);
        ResponseT<List<S_Item>> GetItemsByStoreRating(int id, int rating);
        Response AddItemToCart(int id, int itemID, int itemAmount); //2.3
        //2.4
        Response RemoveItemFromCart(int id, int itemID,  int itemAmount);
        Response EditItemFromCart(int id, int itemID,  int itemAmount);
        ResponseT<Dictionary<string,List<string>>> getDetailsOnCart();
        Response PurchaseCart(int id, string paymentDetails); //2.5
        ResponseT<int> Logout(int id); //3.1
        Response OpenNewStore(int id, string storeName); //3.2
        Response WriteReview(int id, int itemID, string review); //3.3
        Response RateItem(int id, int itemID, int score); //3.4 (not in this version)
        Response WriteMessageToStore(int id, Guid storeID, string message); //3.5  (not in this version)
        Response ComplainToAdmin(int id, string message); //3.6 (not in this version)
        Response GetPurchasesInfo(int id); //3.7 (not in this version)
        //3.8 and 3.9 (not in this version)
        //4.1
        Response AddItemToStore(int id, Guid storeID, string itemName, string itemCategory, float itemPrice);
        Response RemoveItemFromStore(int id, int itemID);
        Response EditItemCategory(string storeName, string itemName, string category);
        Response EditItemPrice(string storeName, string itemName, int price); 
        Response AppointStoreOwner(int id, Guid storeID, string userEmail); //4.4
        Response RemoveStoreOwner(int id, Guid storeID, int userID); //4.5 (not in this version)
        Response AppointStoreManager(int id, Guid storeID, string userEmail); //4.6
        // 4.7
        Response AddStoreManagerPermissions(int id, Guid storeID, int newUserID, string permission);
        Response RemoveStoreManagerPermissions(int id, Guid storeID, int newUserID, string permission);
        Response RemoveStoreManager(int id, Guid storeID, int userID); //4.8 (not in this version)
        Response CloseStore(int id, Guid storeID); //4.9
        Response ReopenStore(int id, Guid storeID); //4.10 (not in this version)
        ResponseT<List<S_Member>> GetEmployeeInfoInStore(int id, Guid storeID);  //4.11
        Response GetPurchasesInfo(int id, Guid storeID);//4.13 //6.4
        //???????
        bool CheckSupplierConnection();
        bool CheckPaymentConnection();
        void CleanUp();
        void SetPaymentService(IPaymentService paymentService);
        void SetSupplierService(ISupplierService supplierService);
        ResponseT<bool> PlacePayment(string transactionDetails);
        ResponseT<bool> PlaceSupply(string orderDetails, string userDetails);
        int GetMaximumWaitServiceTime();
        ResponseT<bool> InitializeTradingSystem(int id);
        Response DeleteStore(int id, Guid storeID);
        Response DeleteMember(int id, int userID);
    }
}
