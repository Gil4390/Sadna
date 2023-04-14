using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SadnaExpressTests.Mocks;

namespace SadnaExpressTests.Unit_Tests
{
    [TestClass()]
    public class StoreFacadeUnitTest
    {
        private IStoreFacade _storeFacade;

        [TestInitialize]
        public void SetUp()
        {
            _storeFacade = new StoreFacade();
        }

        private class Mock_Bad_SupplierService : Mock_SupplierService
        {
            public override bool ShipOrder(string orderDetails, string userDetails)
            {
                Thread.Sleep(11000); // Wait for 11 seconds
                return true; // Return true after waiting
            }

        }

        private class Mock_5sec_SupplierService : Mock_SupplierService
        {
            public override bool ShipOrder(string orderDetails, string userDetails)
            {
                Thread.Sleep(5000); // Wait for 5 seconds
                return true; // Return true after waiting
            }

        }

        [TestMethod()]
        public void StoreFacadeSupplyServiceNoWait_HappyTest()
        {
            //Arrange
            _storeFacade.SetSupplierService(new Mock_SupplierService());
            string orderDetails = "red dress";
            string userDetails = "Dina Agapov";

            //Act
            bool value = _storeFacade.PlaceSupply(orderDetails, userDetails);

            //Assert
            Assert.IsTrue(value);
        }

        [TestMethod()]
        public void StoreFacadeSupplyServiceWait5Sec_HappyTest()
        {
            //Arrange
            _storeFacade.SetSupplierService(new Mock_5sec_SupplierService());
            string orderDetails = "red dress";
            string userDetails = "Dina Agapov";

            //Act
            bool value = _storeFacade.PlaceSupply(orderDetails, userDetails);

            //Assert
            Assert.IsTrue(value);
        }

        [TestMethod()]
        public void StoreFacadeSupplyService_BadTest()
        {
            //Arrange
            _storeFacade.SetSupplierService(new Mock_Bad_SupplierService());
            string orderDetails = "red dress";
            string userDetails = "Dina Agapov";

            //Act
            bool value = _storeFacade.PlaceSupply(orderDetails, userDetails);
            //operation failes cause it takes to much time- returns false
            
            //Assert
            Assert.IsFalse(value);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _storeFacade.CleanUp();
        }
    }
}
