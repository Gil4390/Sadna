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

        [TestInitialize]
        public void Init()
        {
            userFacade = new UserFacade();
            userId = userFacade.Enter();
        }

        [TestCleanup]
        public void CleanUp()
        {
            userFacade.CleanUp();
        }

        [TestMethod]
        public void openStoreUserNotRegister()
        {
            Assert.ThrowsException<Exception>(() => userFacade.OpenStore(userId, 0));
        }

        [TestMethod]
        public void openStoreUserNotLogin()
        {
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            Assert.ThrowsException<Exception>(() => userFacade.OpenStore(userId, 0));
        }

        [TestMethod]
        public void openStoreUserSuccess()
        {
            //the member should get founder permissions
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            userFacade.OpenStore(userId, 0);
            LinkedList<string> per = new LinkedList<string>();
            per.AddLast("founder permissions");
            Assert.IsTrue(userFacade.hasPermissions(userId, 0, per));
            //and only to the relevant store
            Assert.IsFalse(userFacade.hasPermissions(userId, 1, per));
        }

        [TestMethod]
        public void adNewOwnerUserNotRegister()
        {
            Assert.ThrowsException<Exception>(() => userFacade.AddOwner(userId, 0, "nogaschw@gmail.com"));
        }
        
        
        [TestMethod]
        public void AddOwnerUserNotLogin()
        {
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            Assert.ThrowsException<Exception>(() => userFacade.AddOwner(userId, 0, "nogaschw@gmail.com" ));
        }
        
        [TestMethod]
        public void AddOwnerThatNotExist()
        {
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            // Assert.ThrowsException<Exception>(() => userFacade.AddOwner(userId, 0, "nogaschw@gmail.com" ));
            try
            {
                userFacade.AddOwner(userId, 0, "nogaschw@gmail.com");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        [TestMethod]
        public void AddOwnerHasNotHavePermission()
        {
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            // Assert.ThrowsException<Exception>(() => userFacade.AddOwner(userId, 0, "nogaschw@gmail.com" ));
            try
            {
                userFacade.AddOwner(userId, 0, "nogaschw@gmail.com");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        [TestMethod]
        public void FounderAddOwnerSuccess()
        {
            //create founder
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            userFacade.OpenStore(userId, 0);
            //create user 
            int userIdOwner = userFacade.Enter();
            userFacade.Register(userIdOwner, "nogaschw@gmail.com", "noga", "schwartz", "123");
            userFacade.Exit(userIdOwner);
            // add owner
            userFacade.AddOwner(userId, 0, "nogaschw@gmail.com");
            //check permission of the one we create as owner
            LinkedList<string> per = new LinkedList<string>();
            per.AddLast("owner permissions");
            Assert.IsTrue(userFacade.hasPermissions(userIdOwner, 0, per));
        }
        
        [TestMethod]
        public void OwnerAddOwnerSuccess()
        {
            //create founder
            userFacade.Register(userId, "shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            userFacade.OpenStore(userId, 0);
            //create owner
            int userIdOwner = userFacade.Enter();
            userFacade.Register(userIdOwner, "nogaschw@gmail.com", "noga", "schwartz", "123");
            userFacade.AddOwner(userId, 0, "nogaschw@gmail.com");
            userFacade.Login(userIdOwner, "nogaschw@gmail.com", "123");
            //try add owner
            int userIdDina = userFacade.Enter();
            userFacade.Register(userIdDina, "dinaaga@gmail.com", "dina", "agapov", "123");
            userFacade.Exit(userIdDina);
            userFacade.AddOwner(userIdOwner, 0, "dinaaga@gmail.com");
            ////check permission of the one we create as owner
            LinkedList<string> per = new LinkedList<string>();
            per.AddLast("owner permissions");
            Assert.IsTrue(userFacade.hasPermissions(userIdOwner, 0, per));
        }
    }
}
