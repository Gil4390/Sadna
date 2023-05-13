using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.Store.Policy;
using SadnaExpress.ServiceLayer;

namespace SadnaExpressTests.Integration_Tests
{
    /// <summary>
    /// This class contains integration tests for the purchase process in the trading system.
    /// It tests the functionality of purchasing items as a guest user and handles various scenarios, such as successful purchases, failed payments, failed supplier service, and insufficient item quantities.
    /// </summary>
    [TestClass]
    public class PurchaseIT : TradingSystemIT
    {
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
        }

        /// <summary>
        /// Tests the purchase of items by a guest user when payment is successful.
        /// </summary>
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
            Assert.AreEqual(2, trading.GetStorePurchases(userID, storeID1).Value.Count);
            Assert.AreEqual(1, trading.GetStorePurchases(userID, storeID2).Value.Count);
            // see the prices
            //Assert.AreEqual(4000, trading.GetStorePurchases(userID, storeID1).Value[0].CalculatorAmount()); meanwhile the price of order not matter
            //Assert.AreEqual(3000, trading.GetStorePurchases(userID, storeID2).Value[0].CalculatorAmount());
            Assert.AreEqual(11000, Orders.Instance.GetOrdersByUserId(buyerID)[0].CalculatorAmount());
            // check the inventory updated
            Assert.AreEqual(1, trading.GetStore(storeID1).Value.GetItemByQuantity(itemID1));
            Assert.AreEqual(0, trading.GetStore(storeID2).Value.GetItemByQuantity(itemID2));
            //check that the shopping empty
            Assert.AreEqual(0, trading.GetDetailsOnCart(buyerID).Value.Baskets.Count);
        }

        /// <summary>
        /// Tests the purchase of items by a guest user when payment fails.
        /// </summary>
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
            Assert.AreEqual(0, trading.GetStorePurchases(userID, storeID1).Value.Count);
            Assert.AreEqual(0, trading.GetStorePurchases(userID, storeID2).Value.Count);
            // check the inventory stay
            Assert.AreEqual(3, trading.GetStore(storeID1).Value.GetItemByQuantity(itemID1));
            Assert.AreEqual(1, trading.GetStore(storeID2).Value.GetItemByQuantity(itemID2));
            //check that the shopping is same
            Assert.AreEqual(2, trading.GetDetailsOnCart(buyerID).Value.Baskets.Count);
        }

        /// <summary>
        /// Tests the purchase of items by a guest user when the supplier fails to provide the items.
        /// </summary>
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
            Assert.AreEqual(0, trading.GetStorePurchases(userID, storeID1).Value.Count);
            Assert.AreEqual(0, trading.GetStorePurchases(userID, storeID2).Value.Count);
            // check the inventory stay
            Assert.AreEqual(3, trading.GetStore(storeID1).Value.GetItemByQuantity(itemID1));
            Assert.AreEqual(1, trading.GetStore(storeID2).Value.GetItemByQuantity(itemID2));
            //check that the shopping is same
            Assert.AreEqual(2, trading.GetDetailsOnCart(buyerID).Value.Baskets.Count);
        }

        /// <summary>
        /// Tests the purchase of items by a guest user when the quantity of the items is not valid.
        /// </summary>
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
            Assert.AreEqual(0,trading.GetStorePurchases(userID, storeID1).Value.Count);
            Assert.AreEqual(0,trading.GetStorePurchases(userID, storeID2).Value.Count);
            // check the inventory stay
            Assert.AreEqual(1, trading.GetStore(storeID1).Value.GetItemByQuantity(itemID1));
            Assert.AreEqual(1, trading.GetStore(storeID2).Value.GetItemByQuantity(itemID2));
            //check that the shopping is same
            Assert.AreEqual(2, trading.GetDetailsOnCart(buyerID).Value.Baskets.Count);
        }
        /// <summary>
        /// Tests the Items prices in order is the same after edit.
        /// </summary>
        [TestMethod]
        public void ItemPriceStayTheSameAfterEdit()
        {
            //Arrange
            trading.SetPaymentService(new Mocks.Mock_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_SupplierService());
            trading.PurchaseCart(buyerID, "0502485415400", "Rabbi Akiva 5 Beer Sheva");
            // Act
            trading.EditItemPrice(userID, storeID1, itemID1, 10000);
            // Assert
            // see the prices
            // Assert.AreEqual(4000, trading.GetStorePurchases(userID, storeID1).Value[0].CalculatorAmount()); meanwhile the price of order not matter
            // Assert.AreEqual(3000, trading.GetStorePurchases(userID, storeID2).Value[0].CalculatorAmount());
            Assert.AreEqual(11000, Orders.Instance.GetOrdersByUserId(buyerID)[0].CalculatorAmount());
            //The item price edit
            Assert.AreEqual(10000, trading.GetStore(storeID1).Value.GetItemById(itemID1).Price);
        }

        /// <summary>
        /// Tests the Items prices with discount policy
        /// </summary>
        [TestMethod]
        public void PolicyDiscountOfItemsInCart()
        {
            // Arrange
            DiscountPolicy policy1 =trading.CreateSimplePolicy(storeID1, "Itemipad 32", 10,
                DateTime.Now, new DateTime(2024, 05, 22)).Value;
            trading.AddPolicy(storeID1, policy1.ID);
            // Act
            trading.PurchaseCart(buyerID, "0502485415400", "Rabbi Akiva 5 Beer Sheva");
            // price after discount
            Assert.AreEqual(10200, Orders.Instance.GetOrdersByUserId(buyerID)[0].CalculatorAmount());
        }
        /// <summary>
        /// Tests the Items prices with discount policy
        /// </summary>
        [TestMethod]
        public void PolicyAddDiscountOfItemsInCart()
        {
            // Arrange
            DiscountPolicy policy1 =trading.CreateSimplePolicy(storeID1, "Itemipad 32", 10,
                DateTime.Now, new DateTime(2024, 05, 22)).Value;
            DiscountPolicy policy2 =trading.CreateSimplePolicy(storeID1, "Store", 20,
                DateTime.Now, new DateTime(2024, 05, 22)).Value;
            DiscountPolicy addPolicy = trading.CreateComplexPolicy(storeID1, "add", policy1.ID, policy2.ID).Value;
            trading.AddPolicy(storeID1, addPolicy.ID);
            // Act
            trading.PurchaseCart(buyerID, "0502485415400", "Rabbi Akiva 5 Beer Sheva");
            // price after discount
            Assert.AreEqual(8600, Orders.Instance.GetOrdersByUserId(buyerID)[0].CalculatorAmount());
        }
        [TestMethod]
        public void PolicyAddDiscountOfIllegalDate()
        {
            // Arrange
            DiscountPolicy policy1 =trading.CreateSimplePolicy(storeID1, "Itemipad 32", 10,
                new DateTime(2022, 05, 22), new DateTime(2022, 05, 22)).Value;
            DiscountPolicy policy2 =trading.CreateSimplePolicy(storeID1, "Store", 20,
                DateTime.Now, new DateTime(2024, 05, 22)).Value;
            DiscountPolicy addPolicy = trading.CreateComplexPolicy(storeID1, "add", policy1.ID, policy2.ID).Value;
            trading.AddPolicy(storeID1, addPolicy.ID);
            // Act
            trading.PurchaseCart(buyerID, "0502485415400", "Rabbi Akiva 5 Beer Sheva");
            // price after discount
            Assert.AreEqual(9400, Orders.Instance.GetOrdersByUserId(buyerID)[0].CalculatorAmount());
        }

        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
        }
    }
}