using System;
using System.Threading;
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

        [TestMethod]
        public void Test()
        {

        }

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