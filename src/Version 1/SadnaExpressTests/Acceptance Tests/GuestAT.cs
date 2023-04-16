using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;

namespace SadnaExpressTests.Acceptance_Tests
{
    [TestClass]
    public class GuestAT : TradingSystemAT
    {
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
        }

        #region Getting information about stores in the market and the products in the stores 2.1
        #endregion

        #region Guest search products by general search or filters 2.2
        [TestMethod]
        public void GuestSearchProductsByCategoryHome_HappyTest()
        {
            Guid id = new Guid();
            Task<ResponseT<List<Item>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.GetItemsByCategory(id,"Home");
            });
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(task.Result.Value.Count==1);
        }

        [TestMethod]
        public void GuestSearchProductsByCategoryClothes_HappyTest()
        {
            Guid id = new Guid();
            Task<ResponseT<List<Item>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.GetItemsByCategory(id, "clothes");
            });
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(task.Result.Value.Count == 3);
        }

        [TestMethod]
        public void GuestSearchProductsByCategoryClothesMixMaxPrice_HappyTest()
        {
            Guid id = new Guid();
            Task<ResponseT<List<Item>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.GetItemsByCategory(id, "clothes",90);
            });
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(task.Result.Value.Count == 2);
        }

        [TestMethod]
        public void GuestSearchProductsByCategoryThatDoesntExist_BadTest()
        {
            Guid id = new Guid();
            Task<ResponseT<List<Item>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.GetItemsByCategory(id, "shay");
            });
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(task.Result.Value.Count == 0);
        }

        [TestMethod]
        public void GuestSearchProductsByNameTowel_HappyTest()
        {
            Guid id = new Guid();
            Task<ResponseT<List<Item>>> task = Task.Run(() => {
                return GetItemsByName("Towel");
            });
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(task.Result.Value.Count == 1);
        }

        private ResponseT<List<Item>> GetItemsByName(string itemName, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1)
        {
            Guid id = proxyBridge.Enter().Value;
            return proxyBridge.GetItemsByName(id, itemName, minPrice, maxPrice, ratingItem, category, ratingStore);
        }
        private ResponseT<List<Item>> GetItemsByCategory(string category, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, int ratingStore = -1)
        {
            Guid id = proxyBridge.Enter().Value;
            return proxyBridge.GetItemsByCategory(id, category, minPrice, maxPrice, ratingItem, ratingStore);
        }

        private ResponseT<List<Item>> GetItemsByKeysWord(string keyWords, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1)
        {
            Guid id = proxyBridge.Enter().Value;
            return proxyBridge.GetItemsByKeysWord(id, keyWords, minPrice, maxPrice, ratingItem, category, ratingStore);
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void MultipleClientsSearchForItems1_HappyTest()
        {
            Task<ResponseT<List<Item>>>[] clientTasks = new Task<ResponseT<List<Item>>>[] {
                Task.Run(() => GetItemsByName("Towel")),
                Task.Run(() => GetItemsByCategory("clothes")),
                Task.Run(() =>  GetItemsByCategory("clothes",90))
             };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsFalse(clientTasks[0].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[0].Result.Value.Count==1);

            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[1].Result.Value.Count == 3);

            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[2].Result.Value.Count == 2);
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void MultipleClientsSearchForItems2_HappyTest()
        {
            Task<ResponseT<List<Item>>>[] clientTasks = new Task<ResponseT<List<Item>>>[] {
                Task.Run(() => GetItemsByName("Towel")),
                Task.Run(() => GetItemsByCategory("Home")),
                Task.Run(() =>  GetItemsByCategory("clothes",90)),
                Task.Run(() =>  GetItemsByKeysWord("to"))
             };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsFalse(clientTasks[0].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[0].Result.Value.Count == 1);

            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[1].Result.Value.Count == 1);

            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[2].Result.Value.Count == 2);

            Assert.IsFalse(clientTasks[3].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[3].Result.Value.Count == 1);
        }
        #endregion

        #region Guest saving item in the shopping cart for some store 2.3
        #endregion

        #region Guest checking the content of the shopping cart and making changes 2.4
        #endregion

        #region Guest making a purchase of the shopping cart 2.5
        #endregion

        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
        }
    }
}