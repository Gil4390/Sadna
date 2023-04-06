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

        [TestMethod]
        public void openStoreUserNotRegister()
        {
            try
            {
                userFacade.OpenStore(userId, userId);
                Assert.Fail(); // should throw exception
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        [TestMethod]
        public void openStoreUserNotLogin()
        {
            try
            {
                userFacade.Register(userId,"shayk1934@gmail.com", "shay", "kresner", "123");
                userFacade.OpenStore(userId, userId);
                Assert.Fail(); // should throw exception
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [TestMethod]
        public void openStoreUserSuccess() 
        {
            //the member should get founder permissions
            userFacade.Register(userId,"shayk1934@gmail.com", "shay", "kresner", "123");
            userFacade.Login(userId, "shayk1934@gmail.com", "123");
            userFacade.OpenStore(userId, 0);
            LinkedList<string> per = new LinkedList<string>();
            per.AddLast("founder permissions");
            Assert.IsTrue(userFacade.hasPermissions(userId, 0, per));
            //and only to the relevant store
            Assert.IsFalse(userFacade.hasPermissions(userId, 1, per));
        }
        
        
    }
}
