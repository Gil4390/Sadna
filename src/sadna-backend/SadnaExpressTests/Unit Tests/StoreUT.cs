using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DataLayer;

namespace SadnaExpressTests.Unit_Tests
{
    [TestClass()]
    public class StoreUT
    {
        private Store store;
        private Guid item1;
        
        [TestInitialize]
        public void SetUp()
        {
            DatabaseContextFactory.TestMode = true;
            DBHandler.Instance.CleanDB();
            store = new Store("Hello");
            item1 = store.AddItem("Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver", "electronics", 5000.0, 2);
        }

        [TestMethod()]
        public void RemoveItemSuccess()
        {
            //Act
            store.RemoveItem(item1);
            //Assert
            Assert.ThrowsException<Exception>(() => store.RemoveItem(item1));
        }
        
        [TestMethod()]
        public void EditItemQuantitySuccessNegative()
        {
            //Act
            store.EditItemQuantity(item1, -2);
            //Assert
            Assert.ThrowsException<Exception>(() => store.EditItemQuantity(item1, -1));
        }
        
        [TestMethod()]
        public void EditItemQuantitySuccessPositive()
        {
            //Act
            store.EditItemQuantity(item1, 2);
            //Assert
            store.EditItemQuantity(item1, -4); //not throw an error
            Assert.ThrowsException<Exception>(() => store.EditItemQuantity(item1, -1));
        }
        
        [TestMethod()]
        public void EditItemQuantityFail()
        {
            Assert.ThrowsException<Exception>(() => store.EditItemQuantity(item1, -3));
        }
    }
}