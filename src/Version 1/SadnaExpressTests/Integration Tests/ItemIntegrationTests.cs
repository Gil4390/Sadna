using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer;

namespace SadnaExpressTests.Integration_Tests
{
    [TestClass]
    public class ItemIntegrationTests
    {
        private Server _server;
        
        [TestInitialize]
        public void SetUp()
        {
            _server = new Server();
            _server.activateAdmin();
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
        public Guid OpenNewStore(int idx , string email , string pass , string store_name)
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
        public void Add_item_to_cart()
        {
            // int count = 1;
            // Guid g = Open_new_store(count, "tal@weka.io",  "pass12","BigFive");
            // _server.service.AddItemToStore(count, g, "Basketball", "Sports", 10);
            //
        }
    }
}