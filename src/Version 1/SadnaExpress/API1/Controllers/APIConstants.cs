using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.API1.Controllers
{
    public static class APIConstants
    {
        public const string ApiRoot = "api/";

        public static class GuestData
        {
            public const string root = "guest";

            public const string register = "register";

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
        }
    }
}
