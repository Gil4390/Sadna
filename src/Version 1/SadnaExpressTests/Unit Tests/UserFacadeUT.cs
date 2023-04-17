using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.User;
using static SadnaExpressTests.Mocks;

namespace SadnaExpressTests.Unit_Tests
{
    [TestClass()]
    public class UserFacadeUT
    {
        #region Properties
        private IUserFacade _userFacade;
        private Guid userId;
        private Guid storeID;
        private ConcurrentDictionary<Guid, Member> members;
        private Guid memberid = Guid.NewGuid();
        private Guid systemManagerid = Guid.NewGuid();
        #endregion

        #region SetUp
        [TestInitialize]
        public void SetUp()
        {
            members = new ConcurrentDictionary<Guid, Member>();
            members.TryAdd(memberid, new Member(memberid, "AssiAzar@gmail.com", "shay", "kresner", "ShaY1787%$%"));
            PromotedMember systemManager = new PromotedMember(systemManagerid, "RotemSela@gmail.com", "noga", "schwartz", "ShaY1787%$%");
            systemManager.createSystemManager();
            members.TryAdd(systemManagerid, systemManager);
            _userFacade = new UserFacade(new ConcurrentDictionary<Guid, User>(), members, new PasswordHash(), new Mock_PaymentService(), new Mock_SupplierService());
            _userFacade.SetIsSystemInitialize(true);
            userId = _userFacade.Enter();
            storeID = Guid.NewGuid();
        }

        #endregion

        #region Mocks
       
        #endregion

        #region Tests

        #region Initialize Trading System Tests
        [TestMethod()]
        public void UserFacadeInitializeTradingSystem_HappyTest()
        {
            //Arrange
            _userFacade.SetIsSystemInitialize(false);
            members[systemManagerid].LoggedIn = true;

            //Act & Assert
            Assert.IsTrue(_userFacade.InitializeTradingSystem(systemManagerid));
        }

        [TestMethod()]
        public void UserFacadeInitializeTradingSystemTSAlreayInitialized_BadTest()
        {
            //Arrange
            _userFacade.SetIsSystemInitialize(true);
            members[systemManagerid].LoggedIn = true;

            //Act & Assert
            Assert.ThrowsException<Exception>(() => _userFacade.InitializeTradingSystem(systemManagerid));
        }

        [TestMethod()]
        public void UserFacadeInitializeTradingSystemUserIsNotLoggedIn_BadTest()
        {
            //Arrange
            _userFacade.SetIsSystemInitialize(false);
            members[systemManagerid].LoggedIn = false;

            //Act & Assert
            Assert.ThrowsException<Exception>(() => _userFacade.InitializeTradingSystem(systemManagerid));
        }

        [TestMethod()]
        public void UserFacadeInitializeTradingSystemUserNotExist_BadTest()
        {
            //Arrange
            _userFacade.SetIsSystemInitialize(false);
            Guid badId = new Guid();

            //Act & Assert
            Assert.ThrowsException<Exception>(() => _userFacade.InitializeTradingSystem(badId));
        }

        [TestMethod()]
        public void UserFacadeInitializeTradingSystemUserIsNotLoggedInSystemManager_BadTest()
        {
            //Arrange
            _userFacade.SetIsSystemInitialize(false);
            members[memberid].LoggedIn = true;

            //Act & Assert
            Assert.ThrowsException<Exception>(() => _userFacade.InitializeTradingSystem(memberid));
        }


        #endregion

        #region Payment Service Tests
        [TestMethod()]
        public void UserFacadePaymentServiceNoWait_HappyTest()
        {
            //Arrange
            _userFacade.SetPaymentService(new Mock_PaymentService());
            string transactionDetails = "visa card ShaY1787%$%45";
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
            string transactionDetails = "visa card ShaY1787%$%45";
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
            string transactionDetails = "visa card ShaY1787%$%45";
            double amount = 300;
            //Act & Assert
            Assert.IsFalse(_userFacade.PlacePayment(amount, transactionDetails)); //operation failes cause it takes to much time
        }
        #endregion

        #region Supply Service Tests
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
        #endregion

        #region Open Store Tests
        [TestMethod]
        public void openStoreUserNotRegister()
        {
            Assert.ThrowsException<Exception>(() => _userFacade.OpenNewStore(userId, storeID));
        }

        [TestMethod]
        public void openStoreUserNotLogin()
        {
            _userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "ShaY1787%$%");
            Assert.ThrowsException<Exception>(() => _userFacade.OpenNewStore(userId, storeID));
        }

        [TestMethod]
        public void openStoreUserSuccess()
        {
            //the member should get founder permissions
            _userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "ShaY1787%$%");
            _userFacade.Login(userId, "shayk1934@gmail.com", "ShaY1787%$%");
            _userFacade.OpenNewStore(userId, storeID);
            List<string> per = new List<string>();
            per.Add("founder permissions");
            Assert.IsTrue(_userFacade.hasPermissions(userId, storeID, per));
        }
        #endregion

        #region Store Owner Tests
        [TestMethod]
        public void addNewOwnerUserNotRegister()
        {
            Assert.ThrowsException<Exception>(() =>
                _userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com"));
        }


        [TestMethod]
        public void AppointStoreOwnerUserNotLogin()
        {
            _userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "ShaY1787%$%");
            Assert.ThrowsException<Exception>(() =>
                _userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com"));
        }

        [TestMethod]
        public void AppointStoreOwnerThatNotExist()
        {
            _userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "ShaY1787%$%");
            _userFacade.Login(userId, "shayk1934@gmail.com", "ShaY1787%$%");
            Assert.ThrowsException<Exception>(() =>
                _userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com"));
        }

        [TestMethod]
        public void AppointStoreOwnerHasNotHavePermission()
        {
            _userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "ShaY1787%$%");
            _userFacade.Login(userId, "shayk1934@gmail.com", "ShaY1787%$%");
            Assert.ThrowsException<Exception>(() =>
                _userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com"));
        }
        #endregion

        #region Founder Tests
        [TestMethod]
        public void FounderAppointStoreOwnerSuccess()
        {
            //create founder
            _userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "ShaY1787%$%");
            _userFacade.Login(userId, "shayk1934@gmail.com", "ShaY1787%$%");
            _userFacade.OpenNewStore(userId, storeID);
            //create user 
            Guid userIdOwner = _userFacade.Enter();
            _userFacade.Register(userIdOwner, "nogaschw@gmail.com", "noga", "schwartz", "ShaY1787%$%");
            _userFacade.Exit(userIdOwner);
            // add owner
            _userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com");
            //check permission of the one we create as owner
            List<string> per = new List<string>();
            per.Add("owner permissions");
            Assert.IsTrue(_userFacade.hasPermissions(userIdOwner, storeID, per));
        }

        [TestMethod]
        public void OwnerAppointStoreOwnerSuccess()
        {
            //create founder
            _userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "ShaY1787%$%");
            _userFacade.Login(userId, "shayk1934@gmail.com", "ShaY1787%$%");
            _userFacade.OpenNewStore(userId, storeID);
            //create owner
            Guid userIdOwner = _userFacade.Enter();
            _userFacade.Register(userIdOwner, "nogaschw@gmail.com", "noga", "schwartz", "ShaY1787%$%");
            _userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com");
            _userFacade.Login(userIdOwner, "nogaschw@gmail.com", "ShaY1787%$%");
            //try add owner
            Guid userIdDina = _userFacade.Enter();
            _userFacade.Register(userIdDina, "dinaaga@gmail.com", "dina", "agapov", "ShaY1787%$%");
            _userFacade.Exit(userIdDina);
            _userFacade.AppointStoreOwner(userIdOwner, storeID, "dinaaga@gmail.com");
            ////check permission of the one we create as owner
            List<string> per = new List<string>();
            per.Add("owner permissions");
            Assert.IsTrue(_userFacade.hasPermissions(userIdOwner, storeID, per));
        }

        [TestMethod]
        public void FounderAppointStoreManagerSuccess()
        {
            //create founder
            _userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "ShaY1787%$%");
            _userFacade.Login(userId, "shayk1934@gmail.com", "ShaY1787%$%");
            _userFacade.OpenNewStore(userId, storeID);
            //create user 
            Guid userIdOwner = _userFacade.Enter();
            _userFacade.Register(userIdOwner, "nogaschw@gmail.com", "noga", "schwartz", "ShaY1787%$%");
            _userFacade.Exit(userIdOwner);
            // add owner
            _userFacade.AppointStoreManager(userId, storeID, "nogaschw@gmail.com");
            //check permission of the one we create as owner
            List<string> per = new List<string>();
            per.Add("get store history");
            Assert.IsTrue(_userFacade.hasPermissions(userIdOwner, storeID, per));
        }
        #endregion

        #region Store manager Tests
        [TestMethod]
        public void AppointStoreManagerThatAlreadyStoreManage()
        {
            //create founder
            _userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "ShaY1787%$%");
            _userFacade.Login(userId, "shayk1934@gmail.com", "ShaY1787%$%");
            _userFacade.OpenNewStore(userId, storeID);
            //create user 
            Guid userIdOwner = _userFacade.Enter();
            _userFacade.Register(userIdOwner, "nogaschw@gmail.com", "noga", "schwartz", "ShaY1787%$%");
            _userFacade.Exit(userIdOwner);
            // add owner
            _userFacade.AppointStoreManager(userId, storeID, "nogaschw@gmail.com");
            Assert.ThrowsException<Exception>(() =>
                _userFacade.AppointStoreManager(userId, storeID, "nogaschw@gmail.com"));
        }

        [TestMethod]
        public void AppointStoreManagerWithOutPermissions()
        {
            //create founder
            _userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "ShaY1787%$%");
            _userFacade.Login(userId, "shayk1934@gmail.com", "ShaY1787%$%");
            //create user 
            Guid userIdOwner = _userFacade.Enter();
            _userFacade.Register(userIdOwner, "nogaschw@gmail.com", "noga", "schwartz", "ShaY1787%$%");
            _userFacade.Exit(userIdOwner);
            // add owner
            Assert.ThrowsException<Exception>(() =>
                _userFacade.AppointStoreManager(userId, storeID, "nogaschw@gmail.com"));
        }
        [TestMethod]
        public void AddStoreManagerPermissionsSuccess()
        {
            //create founder
            _userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "ShaY1787%$%");
            _userFacade.Login(userId, "shayk1934@gmail.com", "ShaY1787%$%");
            _userFacade.OpenNewStore(userId, storeID);
            //create user 
            Guid userIdManager = _userFacade.Enter();
            _userFacade.Register(userIdManager, "nogaschw@gmail.com", "noga", "schwartz", "ShaY1787%$%");
            _userFacade.Exit(userIdManager);
            _userFacade.AppointStoreManager(userId, storeID, "nogaschw@gmail.com");
            // remove permission
            _userFacade.AddStoreManagerPermissions(userId, storeID, "nogaschw@gmail.com", "add new manager");
            //check permission 
            List<string> per = new List<string>();
            per.Add("add new manager");
            Assert.IsTrue(_userFacade.hasPermissions(userIdManager, storeID, per));        
        }
        [TestMethod]
        public void RemoveStoreManagerPermissionsFromMemberFail()
        {
            //create founder
            _userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "ShaY1787%$%");
            _userFacade.Login(userId, "shayk1934@gmail.com", "ShaY1787%$%");
            _userFacade.OpenNewStore(userId, storeID);
            //create user 
            Guid userIdManager = _userFacade.Enter();
            _userFacade.Register(userIdManager, "nogaschw@gmail.com", "noga", "schwartz", "ShaY1787%$%");
            _userFacade.Exit(userIdManager);
            // remove permission fail
            Assert.ThrowsException<Exception>(() =>_userFacade.RemoveStoreManagerPermissions(userId, storeID, "nogaschw@gmail.com", "get store history"));
        }
        [TestMethod]
        public void RemoveStoreManagerPermissionsSuccess()
        {
            //create founder
            _userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "ShaY1787%$%");
            _userFacade.Login(userId, "shayk1934@gmail.com", "ShaY1787%$%");
            _userFacade.OpenNewStore(userId, storeID);
            //create user 
            Guid userIdManager = _userFacade.Enter();
            _userFacade.Register(userIdManager, "nogaschw@gmail.com", "noga", "schwartz", "ShaY1787%$%");
            _userFacade.Exit(userIdManager);
            _userFacade.AppointStoreManager(userId, storeID, "nogaschw@gmail.com");
            // add permission
            _userFacade.RemoveStoreManagerPermissions(userId, storeID, "nogaschw@gmail.com", "get store history");
            //check permission 
            List<string> per = new List<string>();
            per.Add("get store history");
            Assert.IsFalse(_userFacade.hasPermissions(userIdManager, storeID, per));        
        }
        #endregion

        #endregion

        #region CleanUp
        [TestCleanup]
        public void CleanUp()
        {
            _userFacade.CleanUp();
        }
        #endregion
    }
}