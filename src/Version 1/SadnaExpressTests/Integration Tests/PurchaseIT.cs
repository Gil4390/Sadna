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
        protected Guid itemID1;
        protected Guid itemID2;
        
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
            itemID1 = trading.AddItemToStore(userID, storeID1, "ipad 32", "electronic", 4000, 3).Value;
            itemID2 = trading.AddItemToStore(userID, storeID2, "ipad 32", "electronic", 3000, 1).Value;
            // create guest
            buyerID = trading.Enter().Value;
            // add items to cart
            trading.AddItemToCart(buyerID, storeID1, itemID1, 2);
            trading.AddItemToCart(buyerID, storeID2, itemID2, 1);
        }

        [TestMethod]
        public void PurchaseItemsGuestSuccess()
        {
            //Arrange
            trading.SetPaymentService(new Mocks.Mock_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_SupplierService());
            // Act
            trading.PurchaseCart(buyerID, "0502485415400", "Rabbi Akiva 5 Beer Sheva");
            // Assert
            // check the order not created
            Assert.AreEqual(1, trading.GetStorePurchases(userID, storeID1).Value.Count);
            Assert.AreEqual(1, trading.GetStorePurchases(userID, storeID2).Value.Count);
            // check the inventory updated
            Assert.AreEqual(1, trading.GetStore(storeID1).Value.GetItemByQuantity(itemID1));
            Assert.AreEqual(0, trading.GetStore(storeID2).Value.GetItemByQuantity(itemID2));
            //check that the shopping empty
            Assert.AreEqual(0, trading.GetDetailsOnCart(buyerID).Value.Baskets.Count);
        }
        
        [TestMethod]
        public void PurchaseItemsGuestPaymentFail()
        {
            //Arrange
            trading.SetPaymentService(new Mocks.Mock_Bad_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_SupplierService());
            // Act
            Assert.IsTrue(trading.PurchaseCart(buyerID, "0502485415400", "Rabbi Akiva 5 Beer Sheva").ErrorOccured);
            // Assert
            // check the order not created
            Assert.IsNull(trading.GetStorePurchases(userID, storeID1).Value);
            Assert.IsNull(trading.GetStorePurchases(userID, storeID2).Value);
            // check the inventory stay
            Assert.AreEqual(3, trading.GetStore(storeID1).Value.GetItemByQuantity(itemID1));
            Assert.AreEqual(1, trading.GetStore(storeID2).Value.GetItemByQuantity(itemID2));
            //check that the shopping is same
            Assert.AreEqual(2, trading.GetDetailsOnCart(buyerID).Value.Baskets.Count);
        }
        [TestMethod]
        public void PurchaseItemsGuestSupplierFail()
        {
            //Arrange
            trading.SetPaymentService(new Mocks.Mock_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_Bad_SupplierService());
            // Act
            Assert.IsTrue(trading.PurchaseCart(buyerID, "0502485415400", "Rabbi Akiva 5 Beer Sheva").ErrorOccured);
            // Assert
            // check the order created
            Assert.IsNull(trading.GetStorePurchases(userID, storeID1).Value);
            Assert.IsNull(trading.GetStorePurchases(userID, storeID2).Value);
            // check the inventory stay
            Assert.AreEqual(3, trading.GetStore(storeID1).Value.GetItemByQuantity(itemID1));
            Assert.AreEqual(1, trading.GetStore(storeID2).Value.GetItemByQuantity(itemID2));
            //check that the shopping is same
            Assert.AreEqual(2, trading.GetDetailsOnCart(buyerID).Value.Baskets.Count);
        }
        
        [TestMethod]
        public void PurchaseItemsGuestTheQuantitySmallerFail()
        {
            //Arrange
            trading.SetPaymentService(new Mocks.Mock_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_SupplierService());
            trading.EditItemQuantity(userID, storeID1, itemID1, -2);
            // Act
            Assert.IsTrue(trading.PurchaseCart(buyerID, "0502485415400", "Rabbi Akiva 5 Beer Sheva").ErrorOccured);
            // Assert
            // check the order created
            Assert.IsNull(trading.GetStorePurchases(userID, storeID1).Value);
            Assert.IsNull(trading.GetStorePurchases(userID, storeID2).Value);
            // check the inventory stay
            Assert.AreEqual(1, trading.GetStore(storeID1).Value.GetItemByQuantity(itemID1));
            Assert.AreEqual(1, trading.GetStore(storeID2).Value.GetItemByQuantity(itemID2));
            //check that the shopping is same
            Assert.AreEqual(2, trading.GetDetailsOnCart(buyerID).Value.Baskets.Count);
        }
    }
}