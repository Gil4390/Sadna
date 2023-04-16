using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer;

namespace SadnaExpressTests.Acceptance_Tests
{
    [TestClass]
    public class GuestMemberStoreOwner : TradingSystemAT
    {
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
        }

        #region Product Managment 4.1

        #region Add new item
        [TestMethod]
        public void AddingNewItemGood()
        {
            Guid userid = Guid.Empty;
            Guid storeid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                userid = proxyBridge.Enter().Value;
                proxyBridge.Register(userid, "storeOwnerMail@gmail.com", "tal", "galmor", "A#!a12345678");
                userid = proxyBridge.Login(userid, "storeOwnerMail@gmail.com", "A#!a12345678").Value;
                storeid = proxyBridge.OpenNewStore(userid, "Bamba store").Value;
                return proxyBridge.AddItemToStore(userid, storeid, "bamba", "food", 5.0, 2);
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //error not accured 

            int count = proxyBridge.GetItemsByName(userid, "bamba").Value.Count;
            Assert.AreEqual(1, count); //item was added
        }
        [TestMethod]
        public void AddingNewItemUserNotLoggedIn_Bad()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;

                return proxyBridge.AddItemToStore(tempid, storeid1, "bamba", "food", 5.0, 2);
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 

            int count = proxyBridge.GetItemsByName(tempid, "bamba").Value.Count;
            Assert.AreEqual(0, count); //item was not added
        }
        [TestMethod]
        public void AddingNewItemStoreDoesNotExist_Bad()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                tempid = proxyBridge.Login(tempid, "AsiAzar@gmail.com", "Aa12345678").Value;

                return proxyBridge.AddItemToStore(tempid, Guid.NewGuid(), "bamba", "food", 5.0, 2);
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 
        }
        [TestMethod]
        public void AddingNewItemUserIsNotAStoreOwner_Bad()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                tempid = proxyBridge.Login(tempid, "gil@gmail.com", "asASD876!@").Value;

                return proxyBridge.AddItemToStore(tempid, storeid1, "bamba", "food", 5.0, 2);
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 

            int count = proxyBridge.GetItemsByName(tempid, "bamba").Value.Count;
            Assert.AreEqual(0, count); //item was not added
        }
        [TestMethod]
        public void AddingNewItemUserIsNotTheOwnerOfThisStore_Bad()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                tempid = proxyBridge.Login(tempid, "AsiAzar@gmail.com", "Aa12345678").Value;

                return proxyBridge.AddItemToStore(tempid, storeid2, "bamba", "food", 5.0, 2);
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 

            int count = proxyBridge.GetItemsByName(tempid, "bamba").Value.Count;
            Assert.AreEqual(0, count); //item was not added
        }

        [TestMethod]
        public void AddingNewItemConcurrent()
        {
            //2 owners attempting to add an item with same name

        }
        #endregion

        #region Remove item
        [TestMethod]
        public void RemoveItemGood()
        {

        }
        [TestMethod]
        public void RemoveItemBad()
        {

        }
        [TestMethod]
        public void RemoveItemConcurrent()
        {

        }
        #endregion

        #region Edit item
        [TestMethod]
        public void EditItemGood()
        {

        }
        [TestMethod]
        public void EditItemBad()
        {

        }
        [TestMethod]
        public void EditItemConcurrent()
        {

        }
        #endregion

        #endregion


        #region Appointing a new store owner 4.4

        #endregion


        #region Appointing a new store manager 4.6

        #endregion


        #region changing a store manager permission 4.7

        #endregion


        #region closing a store 4.9

        #endregion


        #region request store employees’ information 4.11

        #endregion


        #region request store purchase history 4.13

        #endregion

        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
        }


        //public Guid Open_new_store(int idx , string email , string pass , string store_name)
        //{
        //    Login(idx, email, pass);
        //    _server.service.OpenNewStore(idx, store_name);
        //    foreach (Store store in _server.service.GetStores().Values)
        //    {
        //        if (store.StoreName == store_name)
        //            return store.StoreID;
        //    }
        //    return new Guid();
        //}

        //[TestMethod]
        //public void Open_new_store()
        //{
        //    int count = 1;
        //    Guid g = Open_new_store(count, "tal@weka.io",  "pass12","BigFive");
        //    Assert.IsFalse(g == new Guid());
        //}
        //[TestMethod]
        //public void Open_store_already_in()
        //{
        //    int count = 1;
        //    Guid g1 = Open_new_store(count, "tal@weka.io",  "pass12","BigFive");
        //    count++;
        //    Guid g2 = Open_new_store(count, "tal@weka.io",  "pass12","BigFive");
        //    int countNum = 0;
        //    foreach (Store store in _server.service.GetStores().Values)
        //    {
        //        if (store.StoreName == "BigFive")
        //            countNum++;
        //    }
        //    Assert.IsTrue(countNum == 1);
        //    Assert.IsFalse(g1 == g2);
        //}
        //[TestMethod]
        //public void Open_store_then_delete()
        //{
        //    SetUp();
        //    int count = 1;
        //    Guid g1 = Open_new_store(count, "tal@weka.io",  "pass12","BigFive");
        //    _server.service.CloseStore(count, g1);
        //    Assert.IsTrue(_server.service.GetStores()[g1].Active);
        //}

    }
}