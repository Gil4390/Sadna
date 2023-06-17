﻿using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;
using SadnaExpress.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaExpress.DomainLayer;
using SadnaExpress.DomainLayer.Store.Policy;
using SadnaExpress.ServiceLayer.Obj;
using SadnaExpress.ServiceLayer.SModels;

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
        

        public ResponseT<List<Store>> GetAllStoreInfo()
        {
            return _realBridge.GetAllStoreInfo();
        }

        public Response EditItemFromCart(Guid id, Guid storeID, Guid itemID, int itemAmount)
        {
            return _realBridge.EditItemFromCart(id, storeID, itemID, itemAmount);
        }

        public ResponseT<ShoppingCart> GetDetailsOnCart(Guid id)
        {
            return _realBridge.GetDetailsOnCart(id);
        }

        public ResponseT<List<ItemForOrder>>  PurchaseCart(Guid id, SPaymentDetails paymentDetails, SSupplyDetails usersDetail)
        {
            return _realBridge.PurchaseCart(id, paymentDetails, usersDetail);
        }

        public ResponseT<List<ItemForOrder>> GetPurchasesOfUser(Guid userID)
        {
            throw new NotImplementedException();
        }

        public Response EditItem(Guid userID, Guid storeID,  Guid itemID, string itemName, string itemCategory, double itemPrice, int quantity)
        {
            return _realBridge.EditItem(userID, storeID, itemID, itemName, itemCategory, itemPrice, quantity);
        }

        public ResponseT<List<SPolicy>> GetAllPolicy(Guid userID, Guid storeID)
        {
            return _realBridge.GetAllPolicy(userID, storeID);
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

        public Response CloseStore(Guid id, Guid storeID)
        {
            return _realBridge.CloseStore(id, storeID);
        }

        public ResponseT<List<SMemberForStore>> GetEmployeeInfoInStore(Guid id, Guid storeID)
        {
            return _realBridge.GetEmployeeInfoInStore(id, storeID);
        }
        

        public Response RemoveUserMembership(Guid userID, string email)
        {
            return _realBridge.RemoveUserMembership(userID, email);
        }

        public Response RemovePermission(Guid userID, Guid storeID, string userEmail, string permission)
        {
            return _realBridge.RemovePermission(userID, storeID, userEmail, permission);
        }
        

        public Response AddItemToCart(Guid id, Guid storeID, Guid itemID, int itemAmount)
        {
            return _realBridge.AddItemToCart(id, storeID, itemID, itemAmount);
        }

        public Response RemoveItemFromCart(Guid id, Guid storeID, Guid itemID)
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

        public ResponseT<Store> GetStore(Guid storeID)
        {
            return _realBridge.GetStore(storeID);
        }

        public void SetTSOrders(IOrders orders)
        {
            _realBridge.SetTSOrders(orders);
        }

        public ResponseT<Item> GetItemByID(Guid storeID, Guid itemID)
        {
            return _realBridge.GetItemByID(storeID, itemID);
        }

        public ResponseT<List<Notification>> GetNotifications(Guid userID)
        {
            return _realBridge.GetNotifications(userID);
        }

        public bool IsSystemInitialize()
        {
            return _realBridge.IsSystemInitialize();
        }

        public ResponseT<SPolicy[]> GetAllConditions(Guid userID,Guid store)
        {
            return _realBridge.GetAllConditions(userID,store);
        }

        public ResponseT<Condition> AddCondition(Guid userID,Guid store, string entity, string entityName, string type, object value, DateTime dt = default,
            string entityRes = default, string entityResName = default, string typeRes = default, double valueRes = default,
            string op = default, int opCond = default)
        {
            return _realBridge.AddCondition(userID,store, entity, entityName, type, value, dt, entityRes, entityName, typeRes,
                valueRes, op, opCond);
        }


        public Response RemoveCondition(Guid userID,Guid storeID, int condID)
        {
            return _realBridge.RemoveCondition(userID, storeID, condID);
        }

        
        
        public ResponseT<DiscountPolicy> CreateSimplePolicy<T>(Guid userID ,Guid store, T level, int percent, DateTime startDate, DateTime endDate)
        {
            return _realBridge.CreateSimplePolicy(userID,store, level, percent, startDate, endDate);
        }

        public ResponseT<DiscountPolicy> CreateComplexPolicy(Guid userID ,Guid store, string op, params int[] policys)
        {
            return _realBridge.CreateComplexPolicy(userID,store, op, policys);
        }

        public Response AddPolicy(Guid userID ,Guid store, int discountPolicy)
        {
            return _realBridge.AddPolicy(userID,store, discountPolicy);
        }

        public Response RemovePolicy(Guid userID ,Guid store, int discountPolicy, string type)
        {
            return _realBridge.RemovePolicy(userID,store, discountPolicy , type);
        }

        public ResponseT<List<SItem>> GetCartItems(Guid userID)
        {
            return _realBridge.GetCartItems(userID);
        }

        public ResponseT<List<SItem>> GetItemsForClient(Guid userID, string keyWords, int minPrice = 0, int maxPrice = int.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1)
        {
            return _realBridge.GetItemsForClient(userID, keyWords, minPrice, maxPrice, ratingItem, category, ratingStore);
        }

        public ResponseT<List<Item>> GetItemsInStore(Guid userID, Guid storeId)
        {
            return _realBridge.GetItemsInStore(userID,storeId);
        }

        public ResponseT<bool> IsAdmin(Guid userID)
        {
            return _realBridge.IsAdmin(userID);
        }

        public ResponseT<Dictionary<Guid, SPermission>> GetMemberPermissions(Guid userID)
        {
            return _realBridge.GetMemberPermissions(userID);
        }

        public ResponseT<SStore> GetStoreInfo(Guid userID, Guid storeId)
        {
            return _realBridge.GetStoreInfo(userID, storeId);
        }

        public Response MarkNotificationAsRead(Guid userID, Guid notificationID)
        {
            throw new NotImplementedException();
        }

        public Response CheckPurchaseConditions(Guid userID)
        {
            return _realBridge.CheckPurchaseConditions(userID);
        }

        public ResponseT<List<SMember>> GetMembers(Guid userID)
        {
            return _realBridge.GetMembers(userID);
        }

        public ResponseT<Dictionary<Guid, List<Order>>> GetPurchasesInfoUser(Guid userID)
        {
            return _realBridge.GetPurchasesInfoUser(userID);
        }

        public void getNotificationsForOfflineMembers()
        {
            throw new NotImplementedException();
        }

        public ResponseT<List<ItemForOrder>> GetStorePurchases(Guid userID, Guid storeID)
        {
            return _realBridge.GetStorePurchases(userID, storeID);
        }
        


        public Response WriteItemReview(Guid userID, Guid itemID, string reviewText)
        {
            return _realBridge.WriteItemReview(userID, itemID, reviewText);
        }

        public ResponseT<List<SReview>> GetItemReviews(Guid itemID)
        {
            return _realBridge.GetItemReviews(itemID);
        }

        public ResponseT<string> GetMemberName(Guid userID)
        {
            return _realBridge.GetMemberName(userID);
        }

        public ResponseT<double> GetStoreRevenue(Guid userID, Guid storeID, DateTime date)
        {
            return _realBridge.GetStoreRevenue(userID, storeID, date);
        }

        public ResponseT<double> GetSystemRevenue(Guid userID, DateTime date)
        {
            return _realBridge.GetSystemRevenue(userID, date);
        }

        public ResponseT<SBid> PlaceBid(Guid userID, Guid itemID, double price)
        {
            return _realBridge.PlaceBid(userID, itemID, price);
        }

        public ResponseT<SBid[]> GetBidsInStore(Guid userID, Guid storeID)
        {
            return _realBridge.GetBidsInStore(userID, storeID);
        }

        public Response ReactToBid(Guid userID, Guid itemID, Guid bidID, string bidResponse)
        {
            return _realBridge.ReactToBid(userID, itemID, bidID, bidResponse);
        }

        public ResponseT<Member> GetMember(string email)
        {
            return _realBridge.GetMember(email);
        }

        public ResponseT<Store> GetStore(string name)
        {
            return _realBridge.GetStore(name);
        }

        public Response ReactToJobOffer(Guid userID, Guid storeID, Guid newEmpID, bool offerResponse)
        {
            return _realBridge.ReactToJobOffer(userID, storeID, newEmpID, offerResponse);
        }

        public ResponseT<List<int>> GetSystemUserActivity(Guid userID, DateTime fromDate, DateTime toDate)
        {
            return _realBridge.GetSystemUserActivity(userID, fromDate, toDate);
        }
    }


    
}
