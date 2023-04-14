using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer;

namespace SadnaExpressTests.Integration_Tests
{
    [TestClass]
    public class ItemAT
    {
        private Server _server;
        
        [TestInitialize]
        public void SetUp()
        {
            _server = new Server();
            _server.activateAdmin();
        }

        public Guid OpenNewStore(int idx , string email , string pass , string store_name)
        {
            _server.activateAdmin();

            Guid guestID1 = _server.service.Enter().Value;
            _server.service.Register(guestID1, email, " tal", " galmor", pass);
            Guid memberID1 = _server.service.Login(guestID1, email, pass).Value;

            _server.service.OpenNewStore(memberID1, store_name);
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