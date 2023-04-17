using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.ServiceLayer;

namespace SadnaExpressTests.Integration_Tests
{
    [TestClass]
    public class PurchaseIT
    {
        protected TradingSystem trading;
        protected Guid userID;
        protected Guid buyerID;
        protected Guid storeID1;
        protected Guid storeID2;
        protected Guid ItemID1;
        protected Guid ItemID2;
        
        [TestInitialize]
        public void Setup()
        {
            trading = new TradingSystem();
            trading.SetIsSystemInitialize(true);
            // create member
            userID = trading.Enter().Value;
            trading.Register(userID, "RotemSela@gmail.com", "noga", "schwartz","asASD876!@");
            userID = trading.Login(userID, "RotemSela@gmail.com", "asASD876!@").Value;
            // open stores
            storeID1 = trading.OpenNewStore(userID, "hello").Value;
            storeID2 = trading.OpenNewStore(userID, "hello2").Value;
            // add items to stores
            ItemID1 = trading.AddItemToStore(userID, storeID1, "ipad 32", "electronic", 4000, 3).Value;
            ItemID2 = trading.AddItemToStore(userID, storeID2, "ipad 32", "electronic", 3000, 1).Value;
            // create guest
            buyerID = trading.Enter().Value;
            // add items to cart
            trading.AddItemToCart(buyerID, storeID1, ItemID1, 2);
            trading.AddItemToCart(buyerID, storeID2, ItemID2, 1);
        }

        [TestMethod]
        public void PurchaseItemsSuccess()
        {
            // Act
            trading.PurchaseCart(buyerID, "0502485415400", "Rabbi Akiva 5 Beer Sheva");
            // assert
            Assert.AreEqual(1, trading.GetStorePurchases(userID, storeID1).Value.Count);
            Assert.AreEqual(1, trading.GetStorePurchases(userID, storeID2).Value.Count);
            Assert.AreEqual(1, trading.GetStorePurchases(userID, storeID1).Value.Count);
        }
    }
}