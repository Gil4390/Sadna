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
        private ItemForOrder itemO1;
        private ItemForOrder itemO2;
        private new List<ItemForOrder> orderList;
            
        [TestInitialize]
        public void SetUp()
        {
            _orders = Orders.Instance;
            userID = Guid.NewGuid();
            storeID = Guid.NewGuid();
            Item item1 = new Item("pink doll", "toy", 100);
            Item item2 = new Item("iPad", "electronic", 4000);
            itemO1 = new ItemForOrder(item1, storeID, "", "");
            itemO2 = new ItemForOrder(item2, storeID, "", "");
            orderList = new List<ItemForOrder> {itemO1, itemO2};
        }

        [TestMethod()]
        public void AddOrderSuccess()
        {
            //act
            _orders.AddOrder(userID, orderList);
            //assert
            Assert.IsNotNull(_orders.GetOrdersByStoreId(storeID));
            Assert.IsNotNull(_orders.GetOrdersByUserId(userID));
        }
        
        [TestMethod()]
        public void AddTwoOrdersToTheSameUserSuccess()
        {
            Item item = new Item("iPad", "electronic", 4000);
            ItemForOrder itemO = new ItemForOrder(item, Guid.NewGuid(), "","");
            List<ItemForOrder> orderList2 = new List<ItemForOrder>{itemO};
            //act
            _orders.AddOrder(userID, orderList);
            _orders.AddOrder(userID, orderList2);
            //assert
            Assert.AreEqual(2,_orders.GetOrdersByUserId(userID).Count);
            Assert.AreEqual(1,_orders.GetOrdersByStoreId(storeID).Count);
            Assert.AreEqual(2, _orders.GetStoreOrders().Keys.Count);
        }
        [TestMethod()]
        public void AddTwoOrdersToTheSameStoreSuccess()
        {
            List<ItemForOrder> orderList2 = new List<ItemForOrder>{itemO2};
            //act
            _orders.AddOrder(userID, orderList);
            _orders.AddOrder(new Guid(), orderList2);
            //assert
            Assert.AreEqual(1,_orders.GetOrdersByUserId(userID).Count);
            Assert.AreEqual(2,_orders.GetOrdersByStoreId(storeID).Count);
            Assert.AreEqual(2, _orders.GetUserOrders().Keys.Count);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _orders.CleanUp();
        }
    }
}