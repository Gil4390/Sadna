using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpressTests.Unit_Tests
{
    [TestClass]
    public class UserUnitTest
    {
        private IUserFacade userFacade;
        private Guid userId;
        private Guid storeID;

        [TestInitialize]
        public void Init()
        {
            userFacade = new UserFacade();
            userId = userFacade.Enter();
            storeID = Guid.NewGuid();
        }

        [TestCleanup]
        public void CleanUp()
        {
            userFacade.CleanUp();
        }

        [TestMethod]
        public void openStoreUserNotRegister()
        {
            Assert.ThrowsException<Exception>(() => userFacade.OpenNewStore(userId, storeID));
        }

        [TestMethod]
        public void openStoreUserNotLogin()
        {
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            Assert.ThrowsException<Exception>(() => userFacade.OpenNewStore(userId, storeID));
        }

        [TestMethod]
        public void openStoreUserSuccess()
        {
            //the member should get founder permissions
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            userFacade.OpenNewStore(userId, storeID);
            List<string> per = new List<string>();
            per.Add("founder permissions");
            Assert.IsTrue(userFacade.hasPermissions(userId, storeID, per));
        }

        [TestMethod]
        public void adNewOwnerUserNotRegister()
        {
            Assert.ThrowsException<Exception>(() =>
                userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com"));
        }


        [TestMethod]
        public void AppointStoreOwnerUserNotLogin()
        {
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            Assert.ThrowsException<Exception>(() =>
                userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com"));
        }

        [TestMethod]
        public void AppointStoreOwnerThatNotExist()
        {
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            Assert.ThrowsException<Exception>(() =>
                userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com"));
        }

        [TestMethod]
        public void AppointStoreOwnerHasNotHavePermission()
        {
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            Assert.ThrowsException<Exception>(() =>
                userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com"));
        }

        [TestMethod]
        public void FounderAppointStoreOwnerSuccess()
        {
            //create founder
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            userFacade.OpenNewStore(userId, storeID);
            //create user 
            Guid userIdOwner = userFacade.Enter();
            userFacade.Register(userIdOwner, "nogaschw@gmail.com", "noga", "schwartz", "123");
            userFacade.Exit(userIdOwner);
            // add owner
            userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com");
            //check permission of the one we create as owner
            List<string> per = new List<string>();
            per.Add("owner permissions");
            Assert.IsTrue(userFacade.hasPermissions(userIdOwner, storeID, per));
        }

        [TestMethod]
        public void OwnerAppointStoreOwnerSuccess()
        {
            //create founder
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            userFacade.OpenNewStore(userId, storeID);
            //create owner
            Guid userIdOwner = userFacade.Enter();
            userFacade.Register(userIdOwner, "nogaschw@gmail.com", "noga", "schwartz", "123");
            userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com");
            userFacade.Login(userIdOwner, "nogaschw@gmail.com", "123");
            //try add owner
            Guid userIdDina = userFacade.Enter();
            userFacade.Register(userIdDina, "dinaaga@gmail.com", "dina", "agapov", "123");
            userFacade.Exit(userIdDina);
            userFacade.AppointStoreOwner(userIdOwner, storeID, "dinaaga@gmail.com");
            ////check permission of the one we create as owner
            List<string> per = new List<string>();
            per.Add("owner permissions");
            Assert.IsTrue(userFacade.hasPermissions(userIdOwner, storeID, per));
        }

        [TestMethod]
        public void FounderAppointStoreManagerSuccess()
        {
            //create founder
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            userFacade.OpenNewStore(userId, storeID);
            //create user 
            Guid userIdOwner = userFacade.Enter();
            userFacade.Register(userIdOwner, "nogaschw@gmail.com", "noga", "schwartz", "123");
            userFacade.Exit(userIdOwner);
            // add owner
            userFacade.AppointStoreManager(userId, storeID, "nogaschw@gmail.com");
            //check permission of the one we create as owner
            List<string> per = new List<string>();
            per.Add("get store history");
            Assert.IsTrue(userFacade.hasPermissions(userIdOwner, storeID, per));
        }

        [TestMethod]
        public void AppointStoreManagerThatAlreadyStoreManage()
        {
            //create founder
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            userFacade.OpenNewStore(userId, storeID);
            //create user 
            Guid userIdOwner = userFacade.Enter();
            userFacade.Register(userIdOwner, "nogaschw@gmail.com", "noga", "schwartz", "123");
            userFacade.Exit(userIdOwner);
            // add owner
            userFacade.AppointStoreManager(userId, storeID, "nogaschw@gmail.com");
            Assert.ThrowsException<Exception>(() =>
                userFacade.AppointStoreManager(userId, storeID, "nogaschw@gmail.com"));
        }

        [TestMethod]
        public void AppointStoreManagerWithOutPermissions()
        {
            //create founder
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            //create user 
            Guid userIdOwner = userFacade.Enter();
            userFacade.Register(userIdOwner, "nogaschw@gmail.com", "noga", "schwartz", "123");
            userFacade.Exit(userIdOwner);
            // add owner
            Assert.ThrowsException<Exception>(() =>
                userFacade.AppointStoreManager(userId, storeID, "nogaschw@gmail.com"));
        }
        [TestMethod]
        public void AddStoreManagerPermissionsSuccess()
        {
            //create founder
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            userFacade.OpenNewStore(userId, storeID);
            //create user 
            Guid userIdManager = userFacade.Enter();
            userFacade.Register(userIdManager, "nogaschw@gmail.com", "noga", "schwartz", "123");
            userFacade.Exit(userIdManager);
            userFacade.AppointStoreManager(userId, storeID, "nogaschw@gmail.com");
            // remove permission
            userFacade.AddStoreManagerPermissions(userId, storeID, "nogaschw@gmail.com", "add new manager");
            //check permission 
            List<string> per = new List<string>();
            per.Add("add new manager");
            Assert.IsTrue(userFacade.hasPermissions(userIdManager, storeID, per));        
        }
        [TestMethod]
        public void RemoveStoreManagerPermissionsFromMemberFail()
        {
            //create founder
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            userFacade.OpenNewStore(userId, storeID);
            //create user 
            Guid userIdManager = userFacade.Enter();
            userFacade.Register(userIdManager, "nogaschw@gmail.com", "noga", "schwartz", "123");
            userFacade.Exit(userIdManager);
            // remove permission fail
            Assert.ThrowsException<Exception>(() =>userFacade.RemoveStoreManagerPermissions(userId, storeID, "nogaschw@gmail.com", "get store history"));
        }
        [TestMethod]
        public void RemoveStoreManagerPermissionsSuccess()
        {
            //create founder
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            userFacade.OpenNewStore(userId, storeID);
            //create user 
            Guid userIdManager = userFacade.Enter();
            userFacade.Register(userIdManager, "nogaschw@gmail.com", "noga", "schwartz", "123");
            userFacade.Exit(userIdManager);
            userFacade.AppointStoreManager(userId, storeID, "nogaschw@gmail.com");
            // add permission
            userFacade.RemoveStoreManagerPermissions(userId, storeID, "nogaschw@gmail.com", "get store history");
            //check permission 
            List<string> per = new List<string>();
            per.Add("get store history");
            Assert.IsFalse(userFacade.hasPermissions(userIdManager, storeID, per));        
        }
    }
}