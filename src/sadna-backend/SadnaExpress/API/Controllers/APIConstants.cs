﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.API.Controllers
{
    public static class APIConstants
    {
        public const string ApiRoot = "api/";

        public static class GuestData
        {
            public const string root = "guest";

            public const string enter = "enter";

            public const string exit = "exit";

            public const string register = "register";

            public const string login = "login";

            public const string isAdmin = "is-admin";

            public const string searchItems = "search-items";

            public const string addItemCart = "add-item-cart";

            public const string removeItemCart = "rm-item-cart";

            public const string editItemCart = "edit-item-cart";

            public const string shoppingCart = "shopping-cart";

            public const string purchaseCart = "purchase-cart";

            public const string checkPurchaseCondition = "check-purchase-conds";

            public const string placeBid = "place-bid";
        }

        public static class MemberData
        {
            public const string root = "member";

            public const string logout = "logout";

            public const string writeItemReview = "write-item-review";

            public const string itemReviews = "item-reviews";

            public const string openStore = "open-store";
            
            public const string addItemToStore = "add-item-store";
            
            public const string removeItemToStore = "rm-item-store";
            
            public const string editItemToStore = "edit-item-store";           
            
            public const string appointStoreManager = "appoint-store-manager";
            
            public const string addStoreManagerPer = "add-store-manager-per";

            public const string appointStoreOwner = "appoint-store-owner";

            public const string removeStorePer =  "rm-store-per";
            
            public const string closeStore = "close-store";

            public const string getEmployee = "get-employees";

            public const string getAllConditions = "get-conds";

            public const string addCondition = "add-cond";

            public const string addConditionForDiscount = "add-cond-dic";
                
            public const string removeCondition = "rm-cond";

            public const string createSimplePolicy = "create-simple-policy";

            public const string createComplexPolicy = "create-complex-policy";

            public const string addPolicy = "add-policy";

            public const string removePolicy = "rm-policy";
            
            public const string getItems = "get-items";

            public const string getStoreInfo = "get-store-info";

            public const string getStorePurchases = "get-store-purchases";

            public const string getAllPolicy = "get-all-policy";

            public const string getStoreRevenue = "get-store-revenue";

            public const string getBidsInStore = "get-bids-in-store";

            public const string getUserPurchases = "get-user-purchases";

            public const string getMemberPermissions = "get-member-permissions";
            
            public const string getMemberName = "get-member-name";

            public const string reactToBid = "react-to-bid";

            public const string reactToJobOffer = "react-to-job-offer";

            public const string getNotifications = "get-notifications";

            public const string MarkNotificationAsRead = "mark-notification-read";

        }

        public static class AdminData
        {
            public const string root = "admin";

            public const string isInit = "is-system-init";

            public const string InitTradingSystem = "system-init";

            public const string allMembers = "all-members";

            public const string removeMember = "rm-member";

            public const string allpurchasesStore = "all-purchases-stores";

            public const string allpurchasesUsers = "all-purchases-users";
          
            public const string getSystemRevenue = "get-system-revenue";

            public const string getSystemUserData = "get-system-user-data";

        }
    }
}
