using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;

namespace SadnaExpressTests.Unit_Tests
{
    [TestClass]
    public class StoreUnitTest
    {
        private IStoreFacade storeFacade;
        private int userId;
        private Guid storeID;
      
        [TestInitialize]
        public void Init()
        {
            storeFacade = new StoreFacade();
            storeID = Guid.NewGuid();
        }

        [TestCleanup]
        public void CleanUp()
        {
            storeFacade.CleanUp();
        }

        [TestMethod]
        public void openStoreWithOutName()
        {
            Assert.ThrowsException<Exception>(() => storeFacade.OpenNewStore(""));
        }
        
        [TestMethod]
        public void GetItemsByNameSuccess()
        {
            Guid store1 = storeFacade.OpenNewStore("hello");
            storeFacade.AddItemToStore(store1, "Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver", "garden", 4000.0, 1);
            Guid store2 = storeFacade.OpenNewStore("hi");
            storeFacade.AddItemToStore(store2, "Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver", "electronics", 5000.0, 2);
            Assert.AreEqual(2, storeFacade.GetItemsByName("Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver").Count);
            Assert.AreEqual(1, storeFacade.GetItemsByName("Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver", maxPrice:4000).Count);
            Assert.AreEqual(1, storeFacade.GetItemsByName("Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver", category:"garden").Count);
        }
        
        [TestMethod]
        public void GetItemsByNameOneExist()
        {
            Guid store1 = storeFacade.OpenNewStore("hello");
            storeFacade.AddItemToStore(store1, "Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver", "electronics", 4000.0, 1);
            Guid store2 = storeFacade.OpenNewStore("hi");
            storeFacade.AddItemToStore(store2, "Apple iPhone 11 Unlocked, 64GB/128GB/256GB, All Colours", "electronics", 5000.0, 2);
            Assert.AreEqual(1, storeFacade.GetItemsByName("Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver").Count);
        }
        
        [TestMethod]
        public void GetItemsByCategorySuccess()
        {
            Guid store1 = storeFacade.OpenNewStore("hello");
            storeFacade.AddItemToStore(store1, "Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver", "electronics", 4000.0, 1);
            Guid store2 = storeFacade.OpenNewStore("hi");
            storeFacade.AddItemToStore(store2, "Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver", "electronics", 5000.0, 2);
            storeFacade.AddItemToStore(store2, "Apple iPhone 11 Unlocked, 64GB/128GB/256GB, All Colours", "electronics", 5000.0, 2);
            Assert.AreEqual(3, storeFacade.GetItemsByCategory("electronics").Count);
            Assert.AreEqual(2, storeFacade.GetItemsByCategory("electronics", minPrice:4500, maxPrice:5000).Count);
        }
        
        [TestMethod]
        public void GetItemsByCategoryOneExist()
        {
            Guid store1 = storeFacade.OpenNewStore("hello");
            storeFacade.AddItemToStore(store1, "Royal Copenhagen Blue Fluted Full Lace BUTTER PAT 1004 Denmark 3", "dishes", 4000.0, 1);
            Guid store2 = storeFacade.OpenNewStore("hi");
            storeFacade.AddItemToStore(store2, "Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver", "electronics", 5000.0, 2);
            storeFacade.AddItemToStore(store2, "Apple iPhone 11 Unlocked, 64GB/128GB/256GB, All Colours", "electronics", 5000.0, 2);
            Assert.AreEqual(2, storeFacade.GetItemsByCategory("electronics").Count);
        }
        
        [TestMethod]
        public void GetItemsByKeyWordsSuccess()
        {
            Guid store1 = storeFacade.OpenNewStore("hello");
            storeFacade.AddItemToStore(store1, "Apple iPad Air 2 32 GB Space Gray Excellent Condition", "garden", 4000.0, 1);
            Guid store2 = storeFacade.OpenNewStore("hi");
            storeFacade.AddItemToStore(store2, "Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver", "electronics", 5000.0, 2);
            storeFacade.AddItemToStore(store2, "Apple iPhone 11 Unlocked, 64GB/128GB/256GB, All Colours", "electronics", 5000.0, 2);
            Assert.AreEqual(3, storeFacade.GetItemsByKeysWord("Apple").Count);
            Assert.AreEqual(2, storeFacade.GetItemsByKeysWord("iPad").Count);
            Assert.AreEqual(1, storeFacade.GetItemsByKeysWord("Gray").Count);
            Assert.AreEqual(2, storeFacade.GetItemsByKeysWord("Apple", category:"electronics").Count);
        }
        
    }
}
