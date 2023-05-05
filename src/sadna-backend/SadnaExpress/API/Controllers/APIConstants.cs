using System;
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

            public const string register = "register";

            public const string login = "login";

            public const string exit = "exit";

            public const string enter = "enter";

            public const string storeInfo = "store-info";

            public const string itemByName = "item-by-name";

            public const string itemByCategory = "item-by-category";

            public const string itemByKeysWord = "item-by-keys-word";

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
        }

        public static class AdminData
        {
            public const string root = "admin";

            public const string isInit = "is-system-init";
            
            public const string allMembers = "all-members";
            
            public const string allpurchases = "all-purchases";
            
            public const string removeMember = "rm-member";
        }
    }
}
