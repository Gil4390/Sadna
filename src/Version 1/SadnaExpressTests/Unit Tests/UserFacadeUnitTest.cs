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
        private ConcurrentDictionary<int, Member> members;

        [TestInitialize]
        public void SetUp()
        {
            int userId = 0;
            int memberid = 0;
            members = new ConcurrentDictionary<int, Member>();
            members.TryAdd(memberid, new Member(0, "shayk1934@gmail.com", "shay", "kresner", "123"));
            _userFacade = new UserFacade(new ConcurrentDictionary<int, User>(), members, userId, new PasswordHash(), new Mock_PaymentService());
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
        public void UserFacadeInitializeTradingSystem_HappyTest() 
        {
            members[0].LoggedIn = true;
            Assert.IsTrue(_userFacade.InitializeTradingSystem(0));
        }

        [TestMethod()]
        public void UserFacadeInitializeTradingSystemUserIsNotLoggedIn_BadTest()
        {
            members[0].LoggedIn = false;
            Assert.ThrowsException<Exception>(() => _userFacade.InitializeTradingSystem(0));
        }

        [TestMethod()]
        public void UserFacadeInitializeTradingSystemUserNotExist_BadTest()
        {
            int badId = 8;
            Assert.ThrowsException<Exception>(() => _userFacade.InitializeTradingSystem(badId));
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

       

        [TestCleanup]
        public void CleanUp()
        {
            _userFacade.CleanUp();
        }

    }
}
