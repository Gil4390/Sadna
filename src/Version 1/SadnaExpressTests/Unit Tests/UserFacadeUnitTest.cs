using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SadnaExpressTests.Mocks;

namespace SadnaExpressTests.Unit_Tests
{
    [TestClass()]
    public class UserFacadeUnitTest
    {
        private IUserFacade _userFacade;
        private ConcurrentDictionary<Guid, Member> members;

        [TestInitialize]
        public void SetUp()
        {
            Guid userId = new Guid();
            Guid memberid = new Guid();
            members = new ConcurrentDictionary<Guid, Member>();
            members.TryAdd(memberid, new Member(memberid, "shayk1934@gmail.com", "shay", "kresner", "123"));
            _userFacade = new UserFacade(new ConcurrentDictionary<Guid, User>(), members, new PasswordHash(), new Mock_PaymentService(), new Mock_SupplierService());
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
        public void UserFacadeInitializeTradingSystem_HappyTest() 
        {
            //members[0].LoggedIn = true;
           // Assert.IsTrue(_userFacade.InitializeTradingSystem(0));
        }

        [TestMethod()]
        public void UserFacadeInitializeTradingSystemUserIsNotLoggedIn_BadTest()
        {
            //members[0].LoggedIn = false;
            //Assert.ThrowsException<Exception>(() => _userFacade.InitializeTradingSystem(0));
        }

        [TestMethod()]
        public void UserFacadeInitializeTradingSystemUserNotExist_BadTest()
        {
            //int badId = 8;
            //Assert.ThrowsException<Exception>(() => _userFacade.InitializeTradingSystem(badId));
        }


        [TestMethod()]
        public void UserFacadePaymentServiceNoWait_HappyTest()
        {
            //Arrange
            _userFacade.SetPaymentService(new Mock_PaymentService());
            string transactionDetails = "visa card 12345";

            //Act
            bool value = _userFacade.PlacePayment(transactionDetails);

            //Assert
            Assert.IsTrue(value);
        }

        [TestMethod()]
        public void UserFacadePaymentServiceWait5Sec_HappyTest()
        {
            //Arrange
            _userFacade.SetPaymentService(new Mock_5sec_PaymentService());
            string transactionDetails = "visa card 12345";

            //Act
            bool value = _userFacade.PlacePayment(transactionDetails);

            //Assert
            Assert.IsTrue(value);
        }

        [TestMethod()]
        public void UserFacadePaymentService_BadTest()
        {
            //Arrange
            _userFacade.SetPaymentService(new Mock_Bad_PaymentService());
            string transactionDetails = "visa card 12345";

            //Act & Assert
            Assert.IsFalse(_userFacade.PlacePayment(transactionDetails)); //operation failes cause it takes to much time- default value for bool is false do responseT returns false
        }

        [TestMethod()]
        public void UserFacadeSupplyServiceNoWait_HappyTest()
        {
            //Arrange
            _userFacade.SetSupplierService(new Mock_SupplierService());
            string orderDetails = "red dress";
            string userDetails = "Dina Agapov";

            //Act
            bool value = _userFacade.PlaceSupply(orderDetails, userDetails);

            //Assert
            Assert.IsTrue(value);
        }

        [TestMethod()]
        public void UserFacadeSupplyServiceWait5Sec_HappyTest()
        {
            //Arrange
            _userFacade.SetSupplierService(new Mock_5sec_SupplierService());
            string orderDetails = "red dress";
            string userDetails = "Dina Agapov";

            //Act
            bool value = _userFacade.PlaceSupply(orderDetails, userDetails);

            //Assert
            Assert.IsTrue(value);
        }

        [TestMethod()]
        public void UserFacadeSupplyService_BadTest()
        {
            //Arrange
            _userFacade.SetSupplierService(new Mock_Bad_SupplierService());
            string orderDetails = "red dress";
            string userDetails = "Dina Agapov";

            //Act
            bool value = _userFacade.PlaceSupply(orderDetails, userDetails);
            //operation failes cause it takes to much time- returns false

            //Assert
            Assert.IsFalse(value);
        }




        [TestCleanup]
        public void CleanUp()
        {
            _userFacade.CleanUp();
        }

    }
}
