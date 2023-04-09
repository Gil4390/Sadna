using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
        
        
    }
}
