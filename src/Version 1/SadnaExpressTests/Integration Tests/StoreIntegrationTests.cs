using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer;

namespace SadnaExpressTests.Integration_Tests
{
    [TestClass]
    public class StoreIntegrationTests
    {
        private Server _server;
        
        [TestInitialize]
        public void SetUp()
        {
            _server = new Server();
        }
        public void Login(int idx , string email , string pass)
        {
            Thread client1 = new Thread(() =>
            {
                _server.service.Enter();
                _server.service.Register(idx, email, " tal", " galmor", pass);
                _server.service.Login(idx, email, pass);
            });
            client1.Start();
            client1.Join();
        }
        public Guid Open_new_store(int idx , string email , string pass , string store_name)
        {
            _server.activateAdmin();
            Login(idx, email, pass);
            _server.service.OpenNewStore(idx, store_name);
            foreach (Store store in _server.service.GetStores().Values)
            {
                if (store.getName() == store_name)
                    return store.StoreID;
            }
            return new Guid();
        }
        
        [TestMethod]
        public void Open_new_store()
        {
            int count = 1;
            Guid g = Open_new_store(count, "tal@weka.io",  "pass12","BigFive");
            Assert.IsFalse(g == new Guid());
        }
        [TestMethod]
        public void Open_store_already_in()
        {
            int count = 1;
            Guid g1 = Open_new_store(count, "tal@weka.io",  "pass12","BigFive");
            count++;
            Guid g2 = Open_new_store(count, "tal@weka.io",  "pass12","BigFive");
            int countNum = 0;
            foreach (Store store in _server.service.GetStores().Values)
            {
                if (store.getName() == "BigFive")
                    countNum++;
            }
            Assert.IsTrue(countNum == 1);
            Assert.IsFalse(g1 == g2);
        }
        [TestMethod]
        public void Open_store_then_delete()
        {
            SetUp();
            int count = 1;
            Guid g1 = Open_new_store(count, "tal@weka.io",  "pass12","BigFive");
            _server.service.CloseStore(count, g1);
            Assert.IsTrue(_server.service.GetStores()[g1].Active);
        }
    }
}