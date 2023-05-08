using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer;
using System;

namespace SadnaExpressTests.Integration_Tests
{
    [TestClass]
    public class ValidCartContentIT : TradingSystemIT
    {
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
        }
        #region cart contect is Valid after multiple different actions
        /// <summary>
        /// Guest enters the trading system
        /// add item to cart
        /// Register to the system and then logging in
        /// check cart item math the item before registering
        /// </summary>
        [TestMethod]
        public void ValidCartContentAfterDiffActions1()
        {
            Guid expectedUserId1 = Guid.Empty;
            Guid expectedBuyerId1 = Guid.Empty;
            Guid ExpectedStoreId1 = Guid.Empty;
            Guid ExpectedStoreId2 = Guid.Empty;


            Guid ExpectedItemId2 = Guid.Empty;

            Guid ExpectedItemId1 = Guid.Empty;



            expectedUserId1 = trading.Enter().Value;
            trading.Register(expectedUserId1, "ExpectedStoreOwner1@gmail.com", "Store", "Owner", "asASD876!@");
            expectedUserId1 = trading.Login(expectedUserId1, "ExpectedStoreOwner1@gmail.com", "asASD876!@").Value;
            //// open stores
            ExpectedStoreId1 = trading.OpenNewStore(expectedUserId1, "Store1").Value;
            //// add items to stores

            ExpectedItemId1 = trading.AddItemToStore(expectedUserId1, ExpectedStoreId1, "ipad 1", "electronic", 4000, 3).Value;
            //// create guest
            expectedBuyerId1 = trading.Enter().Value;
            //// add items to cart
            trading.AddItemToCart(expectedBuyerId1, ExpectedStoreId1, ExpectedItemId1, 2);

            ExpectedItem itemInCart = new ExpectedItem { ItemId = ExpectedItemId1, StoreId = ExpectedStoreId1, ItemName = "ipad 1", ItemCategory = "electronic", ItemPrice = 4000, StoreQuantity = 3, QuantityInCart = 2 };

            // now buyer register to the system and then log in
            trading.Register(expectedBuyerId1, "ExpectedBuyer1@gmail.com", "expected", "Buyer", "asASD876!@");
            expectedBuyerId1 = trading.Login(expectedBuyerId1, "ExpectedBuyer1@gmail.com", "asASD876!@").Value;

            int quantity = trading.GetDetailsOnCart(expectedBuyerId1).Value.GetItemQuantityInCart(ExpectedStoreId1, ExpectedItemId1);
            Item storeItem = trading.GetItemsInStore(expectedUserId1, ExpectedStoreId1).Value[0];

            // check if items in cart match to before registering and logging in

            bool result = itemInCart.CompareInCart(storeItem.ItemID, storeItem.Name, storeItem.Category, storeItem.Price, storeItem.Quantity, ExpectedStoreId1, quantity);
            Assert.IsTrue(result);

        }

        /// <summary>
        /// Guest enters the trading system
        /// Register to the system and then logging in
        /// add item to cart
        /// logout
        /// login again
        /// check cart item math the item before logging out and logging in
        /// </summary>
        [TestMethod]
        public void ValidCartContentAfterDiffActions2()
        {
            Guid expectedUserId1 = Guid.Empty;
            Guid expectedBuyerId1 = Guid.Empty;
            Guid ExpectedStoreId1 = Guid.Empty;
            Guid ExpectedStoreId2 = Guid.Empty;


            Guid ExpectedItemId2 = Guid.Empty;

            Guid ExpectedItemId1 = Guid.Empty;


            expectedUserId1 = trading.Enter().Value;
            trading.Register(expectedUserId1, "ExpectedStoreOwner2@gmail.com", "Store", "Owner", "asASD876!@");
            expectedUserId1 = trading.Login(expectedUserId1, "ExpectedStoreOwner2@gmail.com", "asASD876!@").Value;
            //// open stores
            ExpectedStoreId1 = trading.OpenNewStore(expectedUserId1, "Store1").Value;
            //// add items to stores

            ExpectedItemId1 = trading.AddItemToStore(expectedUserId1, ExpectedStoreId1, "iphone 11", "electronic", 3000, 4).Value;
            //// create guest
            expectedBuyerId1 = trading.Enter().Value;

            // register and log in
            trading.Register(expectedBuyerId1, "ExpectedBuyer2@gmail.com", "expected", "Buyer", "asASD876!@");
            expectedBuyerId1 = trading.Login(expectedBuyerId1, "ExpectedBuyer2@gmail.com", "asASD876!@").Value;


            //// add items to cart
            trading.AddItemToCart(expectedBuyerId1, ExpectedStoreId1, ExpectedItemId1, 1);

            ExpectedItem itemInCart = new ExpectedItem { ItemId = ExpectedItemId1, StoreId = ExpectedStoreId1, ItemName = "iphone 11", ItemCategory = "electronic", ItemPrice = 3000, StoreQuantity = 4, QuantityInCart = 1 };

            // log out
            expectedBuyerId1 = trading.Logout(expectedBuyerId1).Value;
            expectedBuyerId1 = trading.Enter().Value;

            // login
            expectedBuyerId1 = trading.Login(expectedBuyerId1, "ExpectedBuyer2@gmail.com", "asASD876!@").Value;

            // get cart values

            int quantity = trading.GetDetailsOnCart(expectedBuyerId1).Value.GetItemQuantityInCart(ExpectedStoreId1, ExpectedItemId1);
            Item storeItem = trading.GetItemsInStore(expectedUserId1, ExpectedStoreId1).Value[0];

            // check if items in cart match to before logout and logging in

            bool result = itemInCart.CompareInCart(storeItem.ItemID, storeItem.Name, storeItem.Category, storeItem.Price, storeItem.Quantity, ExpectedStoreId1, quantity);
            Assert.IsTrue(result);

        }


        /// <summary>
        /// Guest enters the trading system
        /// Register to the system and then logging in
        /// add 2 item to cart
        /// logout
        /// enter the system
        /// check cart item do not math the item before logging out
        /// log in
        /// check cart item match the cart items berfore logging out
        /// </summary>
        [TestMethod]
        public void ValidCartContentAfterDiffActions3()
        {
            Guid expectedUserId1 = Guid.Empty;
            Guid expectedBuyerId1 = Guid.Empty;
            Guid ExpectedStoreId1 = Guid.Empty;
            Guid ExpectedStoreId2 = Guid.Empty;


            Guid ExpectedItemId2 = Guid.Empty;

            Guid ExpectedItemId1 = Guid.Empty;


            expectedUserId1 = trading.Enter().Value;
            trading.Register(expectedUserId1, "ExpectedStoreOwner3@gmail.com", "Store", "Owner", "asASD876!@");
            expectedUserId1 = trading.Login(expectedUserId1, "ExpectedStoreOwner3@gmail.com", "asASD876!@").Value;
            //// open stores
            ExpectedStoreId1 = trading.OpenNewStore(expectedUserId1, "Store1").Value;
            ExpectedStoreId2 = trading.OpenNewStore(expectedUserId1, "Store2").Value;
            //// add items to stores

            ExpectedItemId1 = trading.AddItemToStore(expectedUserId1, ExpectedStoreId1, "iphone 11", "electronic", 3000, 4).Value;
            ExpectedItemId2 = trading.AddItemToStore(expectedUserId1, ExpectedStoreId2, "ipad 2", "electronic", 1500, 1).Value;
            //// create guest
            expectedBuyerId1 = trading.Enter().Value;

            // register and log in
            trading.Register(expectedBuyerId1, "ExpectedBuyer3@gmail.com", "expected", "Buyer", "asASD876!@");
            expectedBuyerId1 = trading.Login(expectedBuyerId1, "ExpectedBuyer3@gmail.com", "asASD876!@").Value;


            //// add items to cart
            trading.AddItemToCart(expectedBuyerId1, ExpectedStoreId1, ExpectedItemId1, 2);

            ExpectedItem itemInCart = new ExpectedItem { ItemId = ExpectedItemId1, StoreId = ExpectedStoreId1, ItemName = "iphone 11", ItemCategory = "electronic", ItemPrice = 3000, StoreQuantity = 4, QuantityInCart = 2 };

            trading.AddItemToCart(expectedBuyerId1, ExpectedStoreId2, ExpectedItemId2, 1);

            ExpectedItem itemInCart2 = new ExpectedItem { ItemId = ExpectedItemId2, StoreId = ExpectedStoreId2, ItemName = "ipad 2", ItemCategory = "electronic", ItemPrice = 1500, StoreQuantity = 1, QuantityInCart = 1 };


            // log out
            expectedBuyerId1 = trading.Logout(expectedBuyerId1).Value;
            expectedBuyerId1 = trading.Enter().Value;


            // get cart values

            int quantity = trading.GetDetailsOnCart(expectedBuyerId1).Value.GetItemQuantityInCart(ExpectedStoreId1, ExpectedItemId1);
            Item storeItem = trading.GetItemsInStore(expectedUserId1, ExpectedStoreId1).Value[0];

            // check if items in cart match to before logout and logging in

            bool result = itemInCart.CompareInCart(storeItem.ItemID, storeItem.Name, storeItem.Category, storeItem.Price, storeItem.Quantity, ExpectedStoreId1, quantity);
            Assert.IsFalse(result);


            quantity = trading.GetDetailsOnCart(expectedBuyerId1).Value.GetItemQuantityInCart(ExpectedStoreId2, ExpectedItemId2);
            storeItem = trading.GetItemsInStore(expectedUserId1, ExpectedStoreId2).Value[0];

            result = itemInCart2.CompareInCart(storeItem.ItemID, storeItem.Name, storeItem.Category, storeItem.Price, storeItem.Quantity, ExpectedStoreId2, quantity);
            Assert.IsFalse(result);

            // login
            expectedBuyerId1 = trading.Login(expectedBuyerId1, "ExpectedBuyer3@gmail.com", "asASD876!@").Value;

            // get cart values

            quantity = trading.GetDetailsOnCart(expectedBuyerId1).Value.GetItemQuantityInCart(ExpectedStoreId1, ExpectedItemId1);
            storeItem = trading.GetItemsInStore(expectedUserId1, ExpectedStoreId1).Value[0];

            // check if items in cart match to before logout and logging in

            result = itemInCart.CompareInCart(storeItem.ItemID, storeItem.Name, storeItem.Category, storeItem.Price, storeItem.Quantity, ExpectedStoreId1, quantity);
            Assert.IsTrue(result);

            quantity = trading.GetDetailsOnCart(expectedBuyerId1).Value.GetItemQuantityInCart(ExpectedStoreId2, ExpectedItemId2);
            storeItem = trading.GetItemsInStore(expectedUserId1, ExpectedStoreId2).Value[0];

            result = itemInCart2.CompareInCart(storeItem.ItemID, storeItem.Name, storeItem.Category, storeItem.Price, storeItem.Quantity, ExpectedStoreId2, quantity);
            Assert.IsTrue(result);

        }

        /// <summary>
        /// Guest enters the trading system
        /// Register to the system and then logging in
        /// add item to cart
        /// logout
        /// enter the system
        /// add item to cart as cart
        /// check cart item do not math the item before logging out
        /// log in
        /// check cart item match the cart items berfore logging out + new item that was added as guest
        /// </summary>
        [TestMethod]
        public void ValidCartContentAfterDiffActions4()
        {
            Guid expectedUserId1 = Guid.Empty;
            Guid expectedBuyerId1 = Guid.Empty;
            Guid ExpectedStoreId1 = Guid.Empty;
            Guid ExpectedStoreId2 = Guid.Empty;


            Guid ExpectedItemId2 = Guid.Empty;

            Guid ExpectedItemId1 = Guid.Empty;


            expectedUserId1 = trading.Enter().Value;
            trading.Register(expectedUserId1, "ExpectedStoreOwner1@gmail.com", "Store", "Owner", "asASD876!@");
            expectedUserId1 = trading.Login(expectedUserId1, "ExpectedStoreOwner1@gmail.com", "asASD876!@").Value;
            //// open stores
            ExpectedStoreId1 = trading.OpenNewStore(expectedUserId1, "Store1").Value;
            ExpectedStoreId2 = trading.OpenNewStore(expectedUserId1, "Store2").Value;
            //// add items to stores

            ExpectedItemId1 = trading.AddItemToStore(expectedUserId1, ExpectedStoreId1, "iphone 11", "electronic", 3000, 4).Value;
            ExpectedItemId2 = trading.AddItemToStore(expectedUserId1, ExpectedStoreId2, "ipad 2", "electronic", 1500, 1).Value;
            //// create guest
            expectedBuyerId1 = trading.Enter().Value;

            // register and log in
            trading.Register(expectedBuyerId1, "ExpectedBuyer2@gmail.com", "expected", "Buyer", "asASD876!@");
            expectedBuyerId1 = trading.Login(expectedBuyerId1, "ExpectedBuyer2@gmail.com", "asASD876!@").Value;


            //// add items to cart
            trading.AddItemToCart(expectedBuyerId1, ExpectedStoreId1, ExpectedItemId1, 3);

            ExpectedItem itemInCart1 = new ExpectedItem { ItemId = ExpectedItemId1, StoreId = ExpectedStoreId1, ItemName = "iphone 11", ItemCategory = "electronic", ItemPrice = 3000, StoreQuantity = 4, QuantityInCart = 3 };

            // log out
            expectedBuyerId1 = trading.Logout(expectedBuyerId1).Value;
            expectedBuyerId1 = trading.Enter().Value;

            // add new diffrent item to cart
            trading.AddItemToCart(expectedBuyerId1, ExpectedStoreId2, ExpectedItemId2, 1);

            ExpectedItem itemInCart2 = new ExpectedItem { ItemId = ExpectedItemId2, StoreId = ExpectedStoreId2, ItemName = "ipad 2", ItemCategory = "electronic", ItemPrice = 1500, StoreQuantity = 1, QuantityInCart = 1 };

            // get cart values

            int quantity = trading.GetDetailsOnCart(expectedBuyerId1).Value.GetItemQuantityInCart(ExpectedStoreId1, ExpectedItemId1);
            Item storeItem = trading.GetItemsInStore(expectedUserId1, ExpectedStoreId1).Value[0];

            // check if items in cart match to before logout and logging in

            bool result = itemInCart1.CompareInCart(storeItem.ItemID, storeItem.Name, storeItem.Category, storeItem.Price, storeItem.Quantity, ExpectedStoreId1, quantity);
            Assert.IsFalse(result);


            // login
            expectedBuyerId1 = trading.Login(expectedBuyerId1, "ExpectedBuyer2@gmail.com", "asASD876!@").Value;

            // get cart values

            quantity = trading.GetDetailsOnCart(expectedBuyerId1).Value.GetItemQuantityInCart(ExpectedStoreId1, ExpectedItemId1);
            storeItem = trading.GetItemsInStore(expectedUserId1, ExpectedStoreId1).Value[0];

            // check if items in cart match to before logout and logging in

            result = itemInCart1.CompareInCart(storeItem.ItemID, storeItem.Name, storeItem.Category, storeItem.Price, storeItem.Quantity, ExpectedStoreId1, quantity);
            Assert.IsTrue(result);


            quantity = trading.GetDetailsOnCart(expectedBuyerId1).Value.GetItemQuantityInCart(ExpectedStoreId2, ExpectedItemId2);
            storeItem = trading.GetItemsInStore(expectedUserId1, ExpectedStoreId2).Value[0];

            // check if items in cart match to before logout and logging in

            result = itemInCart2.CompareInCart(storeItem.ItemID, storeItem.Name, storeItem.Category, storeItem.Price, storeItem.Quantity, ExpectedStoreId2, quantity);
            Assert.IsTrue(result);
        }


        /// <summary>
        /// Guest enters the trading system
        /// Register to the system and then logging in
        /// add 2 item to cart
        /// logout
        /// enter the system as guest
        /// add item to cart that has value 1 and then purchase it 
        /// log in
        /// check cart item do match the item before logging out that was not purchased
        /// check cart item that was added before logging out and was purchased is not in the cart
        /// 
        /// </summary>
        [TestMethod]
        public void ValidCartContentAfterDiffActions5()
        {
            Guid expectedUserId1 = Guid.Empty;
            Guid expectedBuyerId1 = Guid.Empty;
            Guid ExpectedStoreId1 = Guid.Empty;
            Guid ExpectedStoreId2 = Guid.Empty;


            Guid ExpectedItemId2 = Guid.Empty;

            Guid ExpectedItemId1 = Guid.Empty;


            expectedUserId1 = trading.Enter().Value;
            trading.Register(expectedUserId1, "ExpectedStoreOwner5@gmail.com", "Store", "Owner", "asASD876!@");
            expectedUserId1 = trading.Login(expectedUserId1, "ExpectedStoreOwner5@gmail.com", "asASD876!@").Value;
            //// open stores
            ExpectedStoreId1 = trading.OpenNewStore(expectedUserId1, "Store1").Value;
            ExpectedStoreId2 = trading.OpenNewStore(expectedUserId1, "Store2").Value;
            //// add items to stores

            ExpectedItemId1 = trading.AddItemToStore(expectedUserId1, ExpectedStoreId1, "iphone 11", "electronic", 3000, 4).Value;
            ExpectedItemId2 = trading.AddItemToStore(expectedUserId1, ExpectedStoreId2, "ipad 2", "electronic", 1500, 1).Value;
            //// create guest
            expectedBuyerId1 = trading.Enter().Value;

            // register and log in
            trading.Register(expectedBuyerId1, "ExpectedBuyer5@gmail.com", "expected", "Buyer", "asASD876!@");
            expectedBuyerId1 = trading.Login(expectedBuyerId1, "ExpectedBuyer5@gmail.com", "asASD876!@").Value;


            //// add items to cart
            trading.AddItemToCart(expectedBuyerId1, ExpectedStoreId1, ExpectedItemId1, 3);

            ExpectedItem itemInCart1 = new ExpectedItem { ItemId = ExpectedItemId1, StoreId = ExpectedStoreId1, ItemName = "iphone 11", ItemCategory = "electronic", ItemPrice = 3000, StoreQuantity = 4, QuantityInCart = 3 };

            trading.AddItemToCart(expectedBuyerId1, ExpectedStoreId2, ExpectedItemId2, 1);

            ExpectedItem itemInCart2 = new ExpectedItem { ItemId = ExpectedItemId2, StoreId = ExpectedStoreId2, ItemName = "ipad 2", ItemCategory = "electronic", ItemPrice = 1500, StoreQuantity = 1, QuantityInCart = 1 };


            trading.Logout(expectedUserId1);
            // log out
            expectedBuyerId1 = trading.Logout(expectedBuyerId1).Value;
            expectedBuyerId1 = trading.Enter().Value;

            // add new diffrent item to cart
            trading.AddItemToCart(expectedBuyerId1, ExpectedStoreId2, ExpectedItemId2, 1);

            ExpectedItem itemInCart3 = new ExpectedItem { ItemId = ExpectedItemId2, StoreId = ExpectedStoreId2, ItemName = "ipad 2", ItemCategory = "electronic", ItemPrice = 1500, StoreQuantity = 1, QuantityInCart = 1 };

            // get cart values

            int quantity = trading.GetDetailsOnCart(expectedBuyerId1).Value.GetItemQuantityInCart(ExpectedStoreId1, ExpectedItemId1);
            Item storeItem = trading.GetItemsInStore(expectedUserId1, ExpectedStoreId1).Value[0];

            // check if items in cart match to before logout and logging in

            bool result = itemInCart1.CompareInCart(storeItem.ItemID, storeItem.Name, storeItem.Category, storeItem.Price, storeItem.Quantity, ExpectedStoreId1, quantity);
            Assert.IsFalse(result);


            // purchase cart as guest
            var res = trading.PurchaseCart(expectedBuyerId1, "5411556648", "Rabbi Akiva 5");
            Assert.IsFalse(res.ErrorOccured);//no error occurred purhcase succeed

            // cart now empy as guest

            // login
            expectedBuyerId1 = trading.Login(expectedBuyerId1, "ExpectedBuyer5@gmail.com", "asASD876!@").Value;


            // get cart values

            quantity = trading.GetDetailsOnCart(expectedBuyerId1).Value.GetItemQuantityInCart(ExpectedStoreId1, ExpectedItemId1);
            storeItem = trading.GetItemsInStore(expectedUserId1, ExpectedStoreId1).Value[0];

            // check if items in cart match to before logout and logging in

            result = itemInCart1.CompareInCart(storeItem.ItemID, storeItem.Name, storeItem.Category, storeItem.Price, storeItem.Quantity, ExpectedStoreId1, quantity);
            Assert.IsTrue(result);


            quantity = trading.GetDetailsOnCart(expectedBuyerId1).Value.GetItemQuantityInCart(ExpectedStoreId2, ExpectedItemId2);
            storeItem = trading.GetItemsInStore(expectedUserId1, ExpectedStoreId2).Value[0];

            // check if items in cart match to before logout and logging in

            result = itemInCart2.CompareInCart(storeItem.ItemID, storeItem.Name, storeItem.Category, storeItem.Price, storeItem.Quantity, ExpectedStoreId2, quantity);
            Assert.IsFalse(result);

            result = itemInCart3.CompareInCart(storeItem.ItemID, storeItem.Name, storeItem.Category, storeItem.Price, storeItem.Quantity, ExpectedStoreId2, quantity);
            Assert.IsFalse(result);
        }


        #endregion

        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
        }

    }


    public class ExpectedItem
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public double ItemPrice { get; set; }

        public string ItemCategory { get; set; }

        public int QuantityInCart { get; set; }

        public Guid StoreId { get; set; }

        public int StoreQuantity { get; set; }


        public bool CompareInCart(Guid itemID, string name, string category, double price, int quantity1, Guid expectedStoreId1, int quantity2)
        {
            return ItemId.Equals(itemID) && ItemName.Equals(name) && ItemCategory.Equals(category) && ItemPrice.Equals(price) && StoreQuantity.Equals(quantity1) && StoreId.Equals(expectedStoreId1) && QuantityInCart.Equals(quantity2);
        }
    }
}
