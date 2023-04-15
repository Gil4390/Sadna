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
        private Guid memberid= Guid.NewGuid();
        private Guid systemManagerid = Guid.NewGuid();

        [TestInitialize]
        public void SetUp()
        {
           // Guid memberid = Guid.NewGuid();
           // Guid systemManagerid = Guid.NewGuid();
            members = new ConcurrentDictionary<Guid, Member>();

            members.TryAdd(memberid, new Member(memberid, "shayk1934@gmail.com", "shay", "kresner", "123"));

            PromotedMember systemManager = new PromotedMember(systemManagerid, "nogaschw@gmail.com", "noga", "schwartz", "123");
            systemManager.createSystemManager();
            members.TryAdd(systemManagerid, systemManager);

            _userFacade = new UserFacade(new ConcurrentDictionary<Guid, User>(), members, new PasswordHash(), new Mock_PaymentService(), new Mock_SupplierService());
        }

        private class Mock_Bad_PaymentService : Mock_PaymentService
        {
            public override bool Pay(double amount, string transactionDetails)
            {
                Thread.Sleep(11000); // Wait for 11 seconds
                return true; // Return true after waiting
            }

        }

        private class Mock_5sec_PaymentService : Mock_PaymentService
        {
            public override bool Pay(double amount, string transactionDetails)
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
            members[systemManagerid].LoggedIn = true;
            Assert.IsTrue(_userFacade.InitializeTradingSystem(systemManagerid));
        }

        [TestMethod()]
        public void UserFacadeInitializeTradingSystemUserIsNotLoggedIn_BadTest()
        {
            members[systemManagerid].LoggedIn = false;
            Assert.ThrowsException<Exception>(() => _userFacade.InitializeTradingSystem(systemManagerid));
        }

        [TestMethod()]
        public void UserFacadeInitializeTradingSystemUserNotExist_BadTest()
        {
            Guid badId = new Guid();
            Assert.ThrowsException<Exception>(() => _userFacade.InitializeTradingSystem(badId));
        }

        [TestMethod()]
        public void UserFacadeInitializeTradingSystemUserIsNotLoggedInSystemManager_BadTest()
        {
            members[memberid].LoggedIn = true;
            Assert.ThrowsException<Exception>(() => _userFacade.InitializeTradingSystem(memberid));
        }


        [TestMethod()]
        public void UserFacadePaymentServiceNoWait_HappyTest()
        {
            //Arrange
            _userFacade.SetPaymentService(new Mock_PaymentService());
            string transactionDetails = "visa card 12345";
            double amount = 100;

            //Act
            bool value = _userFacade.PlacePayment(amount, transactionDetails);

            //Assert
            Assert.IsTrue(value);
        }

        [TestMethod()]
        public void UserFacadePaymentServiceWait5Sec_HappyTest()
        {
            //Arrange
            _userFacade.SetPaymentService(new Mock_5sec_PaymentService());
            string transactionDetails = "visa card 12345";
            double amount = 500;
            //Act
            bool value = _userFacade.PlacePayment(amount, transactionDetails);

            //Assert
            Assert.IsTrue(value);
        }

        [TestMethod()]
        public void UserFacadePaymentService_BadTest()
        {
            //Arrange
            _userFacade.SetPaymentService(new Mock_Bad_PaymentService());
            string transactionDetails = "visa card 12345";
            double amount = 300;
            //Act & Assert
            Assert.IsFalse(_userFacade.PlacePayment(amount, transactionDetails)); //operation failes cause it takes to much time
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
