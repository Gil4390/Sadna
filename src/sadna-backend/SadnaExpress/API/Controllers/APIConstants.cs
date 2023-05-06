using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaExpress.DomainLayer.Store.DiscountPolicy;

namespace SadnaExpress.API.Controllers
{
    public static class APIConstants
    {
        public const string ApiRoot = "api/";

        public static class GuestData
        {
            public const string root = "guest";

            public const string register = "register";

            public const string login = "login";

            public const string isAdmin = "is-admin";

            public const string exit = "exit";

            public const string enter = "enter";

            public const string storeInfo = "store-info";

            public const string itemByName = "item-by-name";

            public const string itemByCategory = "item-by-category";

            public const string searchItems = "search-items";

            public const string addItemCart = "add-item-cart";

            public const string removeItemCart = "rm-item-cart";

            public const string editItemCart = "edit-item-cart";

            public const string shoppingCart = "shopping-cart";

            public const string purchaseCart = "purchase-cart";

        }

        public static class MemberData
        {
            public const string root = "member";

            public const string logout = "logout";

            public const string openStore = "open-store";
            
            public const string writeItemReview = "write-item-review";
            
            public const string itemReviews = "item-reviews";
            
            public const string addItemToStore = "add-item-store";
            
            public const string removeItemToStore = "rm-item-store";
            
            public const string editItemToStore = "edit-item-store";
            
            public const string appointStoreOwner = "appoint-store-owner";
            
            public const string appointStoreManager = "appoint-store-manager";
            
            public const string appointStoreManagerPer = "appoint-store-manager-per";
            
            public const string removeStoreManagerPer =  "rm-store-manager-per";
            
            public const string removeStoreOwner = "rm-store-owner";

            public const string closeStore = "close-store";

            public const string getEmployee = "get-employees";

            public const string getStorePurchases = "get-store-purchases";

            public const string deleteStore = "rm-store";

            public const string updateFirst = "update-first";
                
            public const string updateLast = "update-last";
                
            public const string updatePass = "update-pass";
                
            public const string setSQA = "set-sqa";

            public const string getStores = "get-stores";
            
            public const string getStoreOwners = "get-stores-owners";
            
            public const string getStoresOwnerList = "get-store-owner";

            public const string getUser = "get-user";

            public const string getMember = "get-member";

            public const string getAllConditions = "get-conds";

            public const string getCondition = "get-cond";

            public const string addCondition = "add-cond";
                
            public const string removeCondition = "rm-cond";

            public const string addDiscountCondition = "add-discount-cond";

            public const string createSimplePolicy = "create-simple-policy";

            public const string createComplexPolicy = "create-complex-policy";

            public const string addPolicy = "add-policy";

            public const string removePolicy = "rm-policy";
            
            public const string getNotifications = "get-notifications";
            
            public const string getItems = "get-items";

            public const string getMemberPermissions = "get-member-permissions";

            public const string getStoreInfo = "get-store-info";

        }

        public static class AdminData
        {
            public const string root = "admin";

            public const string isInit = "is-system-init";

            public const string InitTradingSystem = "system-init";

            public const string allMembers = "all-members";
            
            public const string allpurchases = "all-purchases";
            
            public const string allpurchasesUser = "all-purchases-user";
            
            public const string allpurchasesUsers = "all-purchases-users";
            
            public const string allpurchasesStore= "all-purchases-store";
            
            public const string removeMember = "rm-member";
        }
    }
}
