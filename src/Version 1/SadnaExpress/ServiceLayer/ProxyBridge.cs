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
    public class ProxyBridge : ITradingSystem
    {
        private ITradingSystem _realBridge;

        public void SetBridge(ITradingSystem Implemantation)
        {
            _realBridge = Implemantation;
        }

        public ResponseT<Dictionary<Guid, List<Order>>> GetAllStorePurchases(Guid userID)
        {
            return _realBridge.GetAllStorePurchases(userID);
        }

        public void CleanUp()
        {
            _realBridge.CleanUp();
        }

        public ResponseT<Guid> Enter()
        {
            return _realBridge.Enter();
        }

        public Response Exit(Guid id)
        {
            return _realBridge.Exit(id);
        }

        public Response Register(Guid id, string email, string firstName, string lastName, string password)
        {
            return _realBridge.Register(id, email, firstName, lastName, password);
        }


        public ResponseT<Guid> Login(Guid id, string email, string password)
        {
            return _realBridge.Login(id, email, password);
        }

        public ResponseT<bool> InitializeTradingSystem(Guid id)
        {
            return _realBridge.InitializeTradingSystem(id);
        }

        public ResponseT<Guid> Logout(Guid id)
        {
            return _realBridge.Logout(id);
        }

        public ResponseT<Guid> OpenNewStore(Guid id, string storeName)
        {
            return _realBridge.OpenNewStore(id, storeName);
        }

        public ResponseT<List<Store>> GetAllStoreInfo(Guid id)
        {
            return _realBridge.GetAllStoreInfo(id);
        }

        public ResponseT<List<Item>> GetItemsByName(Guid id, string itemName, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore)
        {
            return _realBridge.GetItemsByName(id, itemName);
        }
        public ResponseT<List<Item>> GetItemsByCategory(Guid id, string category, int minPrice, int maxPrice, int ratingItem, int ratingStore)
        {
            return _realBridge.GetItemsByCategory(id, category);
        }

        public ResponseT<List<Item>> GetItemsByKeysWord(Guid id, string keyWords, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore)
        {
            return _realBridge.GetItemsByKeysWord(id, keyWords);
        }

        public Response EditItemFromCart(Guid id, int itemID, int itemAmount)
        {
            return _realBridge.EditItemFromCart(id, itemID, itemAmount);
        }

        public ResponseT<Dictionary<string, List<string>>> getDetailsOnCart()
        {
            return _realBridge.getDetailsOnCart();
        }

        public Response PurchaseCart(Guid id, string paymentDetails)
        {
            return _realBridge.PurchaseCart(id, paymentDetails);
        }

        public Response WriteItemReview(Guid userID, Guid storeID, Guid itemID, string reviewText)
        {
            return _realBridge.WriteItemReview(userID, storeID, itemID, reviewText);
        }

        public Response RateItem(Guid id, int itemID, int score)
        {
            return _realBridge.RateItem(id, itemID, score);
        }

        public Response WriteMessageToStore(Guid id, Guid storeID, string message)
        {
            return _realBridge.WriteMessageToStore(id, storeID, message);
        }

        public Response ComplainToAdmin(Guid id, string message)
        {
            return _realBridge.ComplainToAdmin(id, message);
        }

        public Response GetPurchasesInfo(Guid id)
        {
            return _realBridge.GetPurchasesInfo(id);
        }

        public Response EditItemCategory(Guid userID,  Guid storeID, Guid itemID, string category)
        {
            return _realBridge.EditItemCategory(userID, storeID,  itemID, category);
        }

        public Response EditItemPrice(Guid userID,  Guid storeID, Guid itemID, int price)
        {
            return _realBridge.EditItemPrice(userID, storeID, itemID, price);
        }
        
        public Response EditItemName(Guid userID,  Guid storeID, Guid itemID, string name)
        {
            return _realBridge.EditItemName(userID, storeID, itemID, name);
        }
        
        public Response EditItemQuantity(Guid userID, Guid storeID, Guid itemID, int quantity)
        {
            // if you want remove put -i and to add +i
            return _realBridge.EditItemQuantity(userID, storeID, itemID, quantity);
        }
        public Response AppointStoreOwner(Guid id, Guid storeID, string userEmail)
        {
            return _realBridge.AppointStoreOwner(id, storeID, userEmail);
        }

        public Response AppointStoreManager(Guid id, Guid storeID, string newUserEmail)
        {
            return _realBridge.AppointStoreManager(id, storeID, newUserEmail);
        }

        public Response AddStoreManagerPermissions(Guid id, Guid storeID, string userEmail, string permission)
        {
            return _realBridge.AddStoreManagerPermissions(id, storeID, userEmail, permission);
        }

        public Response RemoveStoreManagerPermissions(Guid id, Guid storeID, string userEmail, string permission)
        {
            return _realBridge.RemoveStoreManagerPermissions(id, storeID, userEmail, permission);
        }

        public Response RemoveStoreManager(Guid id, Guid storeID, Guid userID)
        {
            return _realBridge.RemoveStoreManager(id, storeID, userID);
        }

        public Response RemoveStoreOwner(Guid id, Guid storeID, Guid userID)
        {
            return _realBridge.RemoveStoreOwner(id, storeID, userID);
        }

        public Response RemovetStoreManager(Guid id, Guid storeID, Guid userID)
        {
            return _realBridge.RemoveStoreManager(id, storeID, userID);
        }

        public Response CloseStore(Guid id, Guid storeID)
        {
            return _realBridge.CloseStore(id, storeID);
        }

        public Response ReopenStore(Guid id, Guid storeID)
        {
            return _realBridge.ReopenStore(id, storeID);
        }

        public ResponseT<List<PromotedMember>> GetEmployeeInfoInStore(Guid id, Guid storeID)
        {
            return _realBridge.GetEmployeeInfoInStore(id, storeID);
        }

        public Response GetPurchasesInfo(Guid id, Guid storeID)
        {
            return _realBridge.GetPurchasesInfo(id, storeID);
        }

        public ResponseT<List<Order>> GetStorePurchases(Guid userID, Guid storeID)
        {
            return _realBridge.GetStorePurchases(userID, storeID);
        }

        public Response DeleteStore(Guid id, Guid storeID)
        {
            return _realBridge.DeleteStore(id, storeID);
        }

        public Response DeleteMember(Guid id, Guid userID)
        {
            return _realBridge.DeleteMember(id, userID);
        }

        public ResponseT<Guid> UpdateFirst(Guid id, string newFirst)
        {
            throw new NotImplementedException();
        }

        public ResponseT<Guid> UpdateLast(Guid id, string newLast)
        {
            throw new NotImplementedException();
        }

        public ResponseT<Guid> UpdatePassword(Guid id, string newPassword)
        {
            throw new NotImplementedException();
        }

        public ResponseT<Guid> SetSecurityQA(Guid id,string q, string a)
        {
            throw new NotImplementedException();
        }

        public Response AddItemToCart(Guid id, Guid storeID, int itemID, int itemAmount)
        {
            return _realBridge.AddItemToCart(id, storeID, itemID, itemAmount);
        }

        public Response RemoveItemFromCart(Guid id, Guid storeID, int itemID)
        {
            return (_realBridge.RemoveItemFromCart(id, storeID, itemID));
        }

        public ResponseT<Guid> AddItemToStore(Guid id, Guid storeID, string itemName, string itemCategory, double itemPrice, int quantity)
        {
            return _realBridge.AddItemToStore(id, storeID, itemName, itemCategory, itemPrice, quantity);
        }
        public Response RemoveItemFromStore(Guid id, Guid storeID, Guid itemID)
        {
            return _realBridge.RemoveItemFromStore(id, storeID, itemID);
        }

        public void SetPaymentService(IPaymentService paymentService)
        {
            _realBridge.SetPaymentService(paymentService);
        }

        public void SetSupplierService(ISupplierService supplierService)
        {
            _realBridge.SetSupplierService(supplierService);
        }

        public void SetIsSystemInitialize(bool isInitialize)
        {
            _realBridge.SetIsSystemInitialize(isInitialize);
        }

        public ResponseT<User> GetUser(Guid userID)
        {
            return _realBridge.GetUser(userID);
        }

        public ResponseT<Member> GetMember(Guid userID)
        {
            return _realBridge.GetMember(userID);
        }
    }
}
