using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;

namespace SadnaExpressTests.Unit_Tests
{
    [TestClass()]
    public class OrderUT
    {
        private static Orders _orders;
        private Guid userID;
        private Guid storeID;
        private Guid itemID1;
        private Guid itemID2;
        private Order order;
            
        [TestInitialize]
        public void SetUp()
        {
            _orders = Orders.Instance;
            userID = Guid.NewGuid();
            storeID = Guid.NewGuid();
            itemID1 = Guid.NewGuid();
            itemID2 = Guid.NewGuid();
            order = new Order(userID, storeID, new Dictionary<Guid, int> { {itemID1,3}, {itemID2,1} }, 70);
        }

        [TestMethod()]
        public void AddOrderSuccess()
        {
            //act
            _orders.AddOrder(order);
            //assert
            Assert.IsNotNull(_orders.GetOrdersByStoreId(storeID));
            Assert.IsNotNull(_orders.GetOrdersByUserId(userID));
        }
        
        [TestMethod()]
        public void AddTwoOrdersToTheSameUserSuccess()
        {
            Order order2 = new Order(userID, new Guid(), new Dictionary<Guid, int> { {itemID1,2}, {itemID2, 3} }, 70);
            //act
            _orders.AddOrder(order);
            _orders.AddOrder(order2);
            //assert
            Assert.AreEqual(2,_orders.GetOrdersByUserId(userID).Count);
            Assert.AreEqual(1,_orders.GetOrdersByStoreId(storeID).Count);
            Assert.AreEqual(2, _orders.GetStoreOrders().Keys.Count);
        }
        [TestMethod()]
        public void AddTwoOrdersToTheSameStoreSuccess()
        {
            Order order2 = new Order(new Guid(), storeID, new Dictionary<Guid, int>{ {itemID1,1}, {itemID2,2} }, 70);
            //act
            _orders.AddOrder(order);
            _orders.AddOrder(order2);
            //assert
            Assert.AreEqual(1,_orders.GetOrdersByUserId(userID).Count);
            Assert.AreEqual(2,_orders.GetOrdersByStoreId(storeID).Count);
            Assert.AreEqual(2, _orders.GetUserOrders().Keys.Count);
        }

        [TestCleanup]
        public void CleanUp()
        {
            Orders.CleanUp();
        }
    }
}