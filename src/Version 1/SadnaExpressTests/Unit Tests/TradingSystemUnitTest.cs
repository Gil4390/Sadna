using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class TradingSystemUnitTest
    {
        private ITradingSystem _tradingSystem;
        private int maximumWaitTime;

        [TestInitialize]
        public void SetUp()
        {
            _tradingSystem = new TradingSystem();
            maximumWaitTime = _tradingSystem.GetMaximumWaitServiceTime();
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

        private class Mock_Bad_PaymentService : Mock_PaymentService
        {
            public override bool ValidatePayment(string transactionDetails)
            {
                Thread.Sleep(11000); // Wait for 11 seconds
                return true; // Return true after waiting
            }

        }

        private class Mock_5sec_PaymentService : Mock_PaymentService
        {
            public override bool ValidatePayment(string transactionDetails)
            {
                Thread.Sleep(5000); // Wait for 5 seconds
                return true; // Return true after waiting
            }

        }

        [TestMethod()]
        public void TradingSystemPaymentServiceNoWait_HappyTest()
        {
            //Arrange
            _tradingSystem.SetPaymentService(new Mock_PaymentService());
            string transactionDetails = "visa card 12345";

            //Act
            bool value = _tradingSystem.PlacePayment(transactionDetails).Value;
           
            //Assert
            Assert.IsTrue(value);
        }

        [TestMethod()]
        public void TradingSystemPaymentServiceWait5Sec_HappyTest()
        {
            //Arrange
            _tradingSystem.SetPaymentService(new Mock_5sec_PaymentService());
            string transactionDetails = "visa card 12345";

            //Act
            bool value = _tradingSystem.PlacePayment(transactionDetails).Value;

            //Assert
            Assert.IsTrue(value);
        }

        [TestMethod()]
        public void TradingSystemPaymentService_BadTest()
        {
            //Arrange
            _tradingSystem.SetPaymentService(new Mock_Bad_PaymentService());
            string transactionDetails = "visa card 12345";

            //Act & Assert
            Assert.IsFalse(_tradingSystem.PlacePayment(transactionDetails).Value); //operation failes cause it takes to much time- default value for bool is false do responseT returns false
        }

        [TestMethod()]
        public void TradingSystemSupplyServiceNoWait_HappyTest()
        {
            //Arrange
            _tradingSystem.SetSupplierService(new Mock_SupplierService());
            string orderDetails = "red dress";
            string userDetails = "Dina Agapov";

            //Act
            bool value = _tradingSystem.PlaceSupply(orderDetails, userDetails).Value;

            //Assert
            Assert.IsTrue(value);
        }

        [TestMethod()]
        public void TradingSystemSupplyServiceWait5Sec_HappyTest()
        {
            //Arrange
            _tradingSystem.SetSupplierService(new Mock_5sec_SupplierService());
            string orderDetails = "red dress";
            string userDetails = "Dina Agapov";

            //Act
            bool value = _tradingSystem.PlaceSupply(orderDetails, userDetails).Value;

            //Assert
            Assert.IsTrue(value);
        }

        [TestMethod()]
        public void TradingSystemSupplyService_BadTest()
        {
            //Arrange
            _tradingSystem.SetSupplierService(new Mock_Bad_SupplierService());
            string orderDetails = "red dress";
            string userDetails = "Dina Agapov";

            //Act
            bool value = _tradingSystem.PlaceSupply(orderDetails, userDetails).Value;
            //operation failes cause it takes to much time- default value for bool is false do responseT returns false
            
            //Assert
            Assert.IsFalse(value);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _tradingSystem.CleanUp();
        }
    }
}
