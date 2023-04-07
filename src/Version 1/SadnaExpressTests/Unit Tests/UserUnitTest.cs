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
        private int userId;
        private Guid storeID;

        [TestInitialize]
        public void Init()
        {
            userFacade = new UserFacade();
            userId = userFacade.Enter();
            storeID = new Guid();
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
            LinkedList<string> per = new LinkedList<string>();
            per.AddLast("founder permissions");
            Assert.IsTrue(userFacade.hasPermissions(userId, storeID, per));
        }

        [TestMethod]
        public void adNewOwnerUserNotRegister()
        {
            Assert.ThrowsException<Exception>(() => userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com"));
        }
        
        
        [TestMethod]
        public void AppointStoreOwnerUserNotLogin()
        {
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            Assert.ThrowsException<Exception>(() => userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com" ));
        }
        
        [TestMethod]
        public void AppointStoreOwnerThatNotExist()
        {
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            Assert.ThrowsException<Exception>(() => userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com" ));
        }
        
        [TestMethod]
        public void AppointStoreOwnerHasNotHavePermission()
        {
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            Assert.ThrowsException<Exception>(() => userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com" ));
        }
        
        [TestMethod]
        public void FounderAppointStoreOwnerSuccess()
        {
            //create founder
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            userFacade.OpenNewStore(userId, storeID);
            //create user 
            int userIdOwner = userFacade.Enter();
            userFacade.Register(userIdOwner, "nogaschw@gmail.com", "noga", "schwartz", "123");
            userFacade.Exit(userIdOwner);
            // add owner
            userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com");
            //check permission of the one we create as owner
            LinkedList<string> per = new LinkedList<string>();
            per.AddLast("owner permissions");
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
            int userIdOwner = userFacade.Enter();
            userFacade.Register(userIdOwner, "nogaschw@gmail.com", "noga", "schwartz", "123");
            userFacade.AppointStoreOwner(userId, storeID, "nogaschw@gmail.com");
            userFacade.Login(userIdOwner, "nogaschw@gmail.com", "123");
            //try add owner
            int userIdDina = userFacade.Enter();
            userFacade.Register(userIdDina, "dinaaga@gmail.com", "dina", "agapov", "123");
            userFacade.Exit(userIdDina);
            userFacade.AppointStoreOwner(userIdOwner, storeID, "dinaaga@gmail.com");
            ////check permission of the one we create as owner
            LinkedList<string> per = new LinkedList<string>();
            per.AddLast("owner permissions");
            Assert.IsTrue(userFacade.hasPermissions(userIdOwner, storeID, per));
        }
        
        public void FounderAppointStoreManagerSuccess()
        {
            //create founder
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            userFacade.OpenNewStore(userId, storeID);
            //create user 
            int userIdOwner = userFacade.Enter();
            userFacade.Register(userIdOwner, "nogaschw@gmail.com", "noga", "schwartz", "123");
            userFacade.Exit(userIdOwner);
            // add owner
            userFacade.AppointStoreManager(userId, storeID, "nogaschw@gmail.com");
            //check permission of the one we create as owner
            LinkedList<string> per = new LinkedList<string>();
            per.AddLast("get store history");
            Assert.IsTrue(userFacade.hasPermissions(userIdOwner, storeID, per));
        }
        
        public void AppointStoreManagerThatAlreadyStoreManage()
        {
            //create founder
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            userFacade.OpenNewStore(userId, storeID);
            //create user 
            int userIdOwner = userFacade.Enter();
            userFacade.Register(userIdOwner, "nogaschw@gmail.com", "noga", "schwartz", "123");
            userFacade.Exit(userIdOwner);
            // add owner
            userFacade.AppointStoreManager(userId, storeID, "nogaschw@gmail.com");
            Assert.ThrowsException<Exception>(()=>userFacade.AppointStoreManager(userId, storeID, "nogaschw@gmail.com"));
        }
        
        public void AppointStoreManagerWithOutPermissions()
        {
            //create founder
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            //create user 
            int userIdOwner = userFacade.Enter();
            userFacade.Register(userIdOwner, "nogaschw@gmail.com", "noga", "schwartz", "123");
            userFacade.Exit(userIdOwner);
            // add owner
            userFacade.AppointStoreManager(userId, storeID, "nogaschw@gmail.com");
            Assert.ThrowsException<Exception>(()=>userFacade.AppointStoreManager(userId, storeID, "nogaschw@gmail.com"));
        }
    }
}
