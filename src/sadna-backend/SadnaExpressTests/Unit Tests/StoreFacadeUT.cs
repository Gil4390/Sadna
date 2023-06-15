﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.DataLayer;

namespace SadnaExpressTests.Unit_Tests
{
    [TestClass()]
    public class StoreFacadeUT : TradingSystemUT
    {
        private IStoreFacade storeFacade;
        private Guid storeID;
        private static Orders _orders;
        private Guid userID1;
        private Guid userID2;
        private Guid storeID1;
        private Guid itemID1;
        private Guid itemID2;
        private ItemForOrder itemO1;
        private Order order;
        private List<ItemForOrder> orderList;
        int numOfOpenStores;
        int numOfTotalItemsInAllStores;

        #region SetUp
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            storeFacade = new StoreFacade();
            storeFacade.SetIsSystemInitialize(true);
            storeID = Guid.NewGuid();

            //Write review setup
            _orders = Orders.Instance;
            userID1 = Guid.NewGuid();
            userID2 = Guid.NewGuid();
            storeID1 = storeFacade.OpenNewStore("Bamba store");
            itemID2 = Guid.NewGuid();
            itemID1 = storeFacade.AddItemToStore(storeID1, "Bamba shosh limited edition", "food", 20.0, 2);
            itemO1 = new ItemForOrder(storeFacade.GetItemByID(storeID1, itemID1), storeID, "nomatter","Bamba store");
            orderList = new List<ItemForOrder> {itemO1};
            _orders.AddOrder(userID1, orderList, AddToDB:false);
            numOfOpenStores = 1;
            numOfTotalItemsInAllStores = 1;
        }
        #endregion

        #region Tests

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
            Assert.AreEqual(2, storeFacade.GetItemsByKeysWord("Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver").Count);
            Assert.AreEqual(1, storeFacade.GetItemsByKeysWord("Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver", maxPrice:4000).Count);
            Assert.AreEqual(1, storeFacade.GetItemsByKeysWord("Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver", category:"garden").Count);
        }
        
        [TestMethod]
        public void GetItemsByNameOneExist()
        {
            Guid store1 = storeFacade.OpenNewStore("hello");
            storeFacade.AddItemToStore(store1, "Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver", "electronics", 4000.0, 1);
            Guid store2 = storeFacade.OpenNewStore("hi");
            storeFacade.AddItemToStore(store2, "Apple iPhone 11 Unlocked, 64GB/128GB/256GB, All Colours", "electronics", 5000.0, 2);
            Assert.AreEqual(1, storeFacade.GetItemsByKeysWord("Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver").Count);
        }
        
        [TestMethod]
        public void GetItemsByCategorySuccess()
        {
            Guid store1 = storeFacade.OpenNewStore("hello");
            storeFacade.AddItemToStore(store1, "Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver", "electronics", 4000.0, 1);
            Guid store2 = storeFacade.OpenNewStore("hi");
            storeFacade.AddItemToStore(store2, "Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver", "electronics", 5000.0, 2);
            storeFacade.AddItemToStore(store2, "Apple iPhone 11 Unlocked, 64GB/128GB/256GB, All Colours", "electronics", 5000.0, 2);
            Assert.AreEqual(3, storeFacade.GetItemsByKeysWord("",category:"electronics").Count);
            Assert.AreEqual(2, storeFacade.GetItemsByKeysWord("", minPrice:4500, maxPrice:5000, category:"electronics").Count);
        }
        
        [TestMethod]
        public void GetItemsByCategoryOneExist()
        {
            Guid store1 = storeFacade.OpenNewStore("hello");
            storeFacade.AddItemToStore(store1, "Royal Copenhagen Blue Fluted Full Lace BUTTER PAT 1004 Denmark 3", "dishes", 4000.0, 1);
            Guid store2 = storeFacade.OpenNewStore("hi");
            storeFacade.AddItemToStore(store2, "Apple iPad Air A1474 32GB Wi-Fi 9.7 inch Silver", "electronics", 5000.0, 2);
            storeFacade.AddItemToStore(store2, "Apple iPhone 11 Unlocked, 64GB/128GB/256GB, All Colours", "electronics", 5000.0, 2);
            Assert.AreEqual(2, storeFacade.GetItemsByKeysWord("", category:"electronics").Count);
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
            Assert.AreEqual(2, storeFacade.GetItemsByKeysWord("ipad").Count);
            Assert.AreEqual(1, storeFacade.GetItemsByKeysWord("GRay").Count);
            Assert.AreEqual(2, storeFacade.GetItemsByKeysWord("apple", category:"electronics").Count);
        }


        [TestMethod]
        public void WriteItemReviewSuccess()
        {
            storeFacade.WriteItemReview(userID1, itemID1, "very bad bamba");
            Assert.AreEqual(1, storeFacade.GetItemReviews(itemID1).Count);
        }

        [TestMethod]
        public void WriteItemReviewUserDidNotBuyItem()
        {
            Assert.ThrowsException<NullReferenceException>(() => storeFacade.WriteItemReview(userID2, itemID1, "very bad bamba"));
        }

        [TestMethod]
        public void WriteItemReviewUserDidNotBuyItem2()
        {
            Assert.ThrowsException<Exception>(() => storeFacade.WriteItemReview(userID1, itemID2, "very bad bamba"));
        }

        [TestMethod]
        public void GetAllStoreInfoGood()
        {
            Assert.AreEqual(numOfOpenStores, storeFacade.GetAllStoreInfo().Count);
            Guid store1 = storeFacade.OpenNewStore("hello");
            numOfOpenStores++;
            Assert.AreEqual(numOfOpenStores, storeFacade.GetAllStoreInfo().Count);



            Guid itemID3 = storeFacade.AddItemToStore(storeID1, "Bamba with noughat ", "food", 10.0, 1);
            Guid itemID4 = storeFacade.AddItemToStore(store1, "Regular Bamba", "food", 5.0, 1);
            numOfTotalItemsInAllStores += 2;


            int itemCount = 0;
            foreach (var store in storeFacade.GetAllStoreInfo())
            {
                itemCount += store.itemsInventory.items_quantity.Count;
            }
            Assert.AreEqual(numOfTotalItemsInAllStores, itemCount);
        }

        #endregion 

        #region CleanUp
        [TestCleanup]
        public void CleanUp()
        {
            DBHandler.Instance.CleanDB();
            storeFacade.CleanUp();
        }
        #endregion

    }
}
