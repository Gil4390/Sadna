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

        public Response Exit(int id)
        {
            throw new NotImplementedException();
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

        public ResponseT<Guid> OpenNewStore(int id, string storeName)
        {
            return _realBridge.OpenNewStore(id, storeName);
        }

        public ResponseT<List<S_Store>> GetAllStoreInfo(int id)
        {
            return _realBridge.GetAllStoreInfo(id);
        }

        public ResponseT<List<S_Item>> GetItemsByName(int id, string itemName)
        {
            return _realBridge.GetItemsByName(id, itemName);
        }

        public ResponseT<List<S_Item>> GetItemsByCategory(int id, string category)
        {
            return _realBridge.GetItemsByCategory(id, category);
        }

        public ResponseT<List<S_Item>> GetItemsByKeysWord(int id, string keyWords)
        {
            return _realBridge.GetItemsByKeysWord(id, keyWords);
        }

        public ResponseT<List<S_Item>> GetItemsByPrices(int id, int minPrice, int maxPrice)
        {
            return _realBridge.GetItemsByPrices(id, minPrice, maxPrice);
        }

        public ResponseT<List<S_Item>> GetItemsByItemRating(int id, int rating)
        {
            return _realBridge.GetItemsByItemRating(id, rating);
        }

        public ResponseT<List<S_Item>> GetItemsByStoreRating(int id, int rating)
        {
            return _realBridge.GetItemsByStoreRating(id, rating);
        }

        //public Response AddItemToCart(int id, int itemID, int itemAmount)
        //{
        //    return _realBridge.AddItemToCart(id, itemID, itemAmount);
        //}

        //public Response RemoveItemFromCart(int id, int itemID, int itemAmount)
        //{
        //    return _realBridge.RemoveItemFromCart(id, itemID, itemAmount);
        //}

        public Response EditItemFromCart(int id, int itemID, int itemAmount)
        {
            return _realBridge.EditItemFromCart(id, itemID, itemAmount);
        }

        public ResponseT<Dictionary<string, List<string>>> getDetailsOnCart()
        {
            return _realBridge.getDetailsOnCart();
        }

        public Response PurchaseCart(int id, string paymentDetails)
        {
            return _realBridge.PurchaseCart(id, paymentDetails);
        }

        public Response WriteReview(int id, int itemID, string review)
        {
            return _realBridge.WriteReview(id, itemID, review);
        }

        public Response RateItem(int id, int itemID, int score)
        {
            return _realBridge.RateItem(id, itemID, score);
        }

        public Response WriteMessageToStore(int id, Guid storeID, string message)
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

        //public Response AddItemToStore(int id, Guid storeID, string itemName, string itemCategory, float itemPrice)
        //{
        //    return _realBridge.AddItemToStore(id, storeID, itemName, itemCategory, itemPrice);
        //}

        //public Response RemoveItemFromStore(int id, int itemID)
        //{
        //    return _realBridge.RemoveItemFromStore(id, itemID);
        //}

        public Response EditItemCategory(string storeName, string itemName, string category)
        {
            return _realBridge.EditItemCategory( storeName,  itemName, category);
        }

        public Response EditItemPrice(string storeName, string itemName, int price)
        {
            return _realBridge.EditItemPrice( storeName,  itemName, price);
        }

        public Response AppointStoreOwner(int id, Guid storeID, string userEmail)
        {
            return _realBridge.AppointStoreOwner(id, storeID, userEmail);
        }

        public Response AppointStoreManager(int id, Guid storeID, string newUserEmail)
        {
            return _realBridge.AppointStoreManager(id, storeID, newUserEmail);
        }

        public Response AddStoreManagerPermissions(int id, Guid storeID, string userEmail, string permission)
        {
            return _realBridge.AddStoreManagerPermissions(id, storeID, userEmail, permission);
        }

        public Response RemoveStoreManagerPermissions(int id, Guid storeID, string userEmail, string permission)
        {
            return _realBridge.RemoveStoreManagerPermissions(id, storeID, userEmail, permission);
        }

        public Response RemoveStoreManager(int id, Guid storeID, int userID)
        {
            return _realBridge.RemoveStoreManager(id, storeID, userID);
        }

        public Response RemoveStoreOwner(int id, Guid storeID, int userID)
        {
            return _realBridge.RemoveStoreOwner(id, storeID, userID);
        }

        public Response RemovetStoreManager(int id, Guid storeID, int userID)
        {
            return _realBridge.RemoveStoreManager(id, storeID, userID);
        }

        public Response CloseStore(int id, Guid storeID)
        {
            return _realBridge.CloseStore(id, storeID);
        }

        public Response ReopenStore(int id, Guid storeID)
        {
            return _realBridge.ReopenStore(id, storeID);
        }

        public ResponseT<List<S_Member>> GetEmployeeInfoInStore(int id, Guid storeID)
        {
            return _realBridge.GetEmployeeInfoInStore(id, storeID);
        }

        public Response GetPurchasesInfo(int id, Guid storeID)
        {
            return _realBridge.GetPurchasesInfo(id, storeID);
        }

        public Response DeleteStore(int id, Guid storeID)
        {
            return _realBridge.DeleteStore(id, storeID);
        }

        public Response DeleteMember(int id, int userID)
        {
            return _realBridge.DeleteMember(id, userID);
        }

        public ResponseT<int> UpdateFirst(int id, string newFirst)
        {
            throw new NotImplementedException();
        }

        public ResponseT<int> UpdateLast(int id, string newLast)
        {
            throw new NotImplementedException();
        }

        public ResponseT<int> UpdatePassword(int id, string newPassword)
        {
            throw new NotImplementedException();
        }

        public ResponseT<int> SetSecurityQA(int id,string q, string a)
        {
            throw new NotImplementedException();
        }

        public Response AddItemToCart(int id, Guid storeID, int itemID, int itemAmount)
        {
            return _realBridge.AddItemToCart(id, storeID, itemID, itemAmount);
        }

        public Response RemoveItemFromCart(int id, Guid storeID, int itemID)
        {
            return (_realBridge.RemoveItemFromCart(id, storeID, itemID));
        }

        public Response AddItemToStore(int id, Guid storeID, string itemName, string itemCategory, double itemPrice, int quantity)
        {
            return _realBridge.AddItemToStore(id, storeID, itemName, itemCategory, itemPrice, quantity);
        }
        public Response RemoveItemFromStore(int id, Guid storeID, int itemID)
        {
            return _realBridge.RemoveItemFromStore(id, storeID, itemID);
        }

    }
}
