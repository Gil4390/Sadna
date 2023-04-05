using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using System;

namespace SadnaExpressTests.Acceptance_Tests.User
{
    [TestClass]
    public class LoginTest
    {
        private static UserFacade userFacade;

        [TestInitialize]
        public void Init()
        {
            userFacade = new UserFacade();

        }

        [TestMethod]
        public void LoginHappyTest()
        {
            Assert.IsTrue(userFacade.Login(0, "asd1@gmail.com", "Asdqwe123") != -1);
            userFacade.Logout(0);
        }

        [TestMethod]
        public void LoginSadTest()
        {

            Assert.ThrowsException<Exception>(() => userFacade.Login(0, "asd1@gmail.com", "123asdqwe")); // user log in with wrong password

            Assert.ThrowsException<Exception>(() => userFacade.Login(1, "asd1@gmail.com", "Qweqwe123")); // user log in with wrong email


            Assert.ThrowsException<Exception>(() => userFacade.Login(-1, "asd1@gmail.com", "123asdqwe")); // user log in with negative id
            Assert.ThrowsException<Exception>(() => userFacade.Login(0, "", "123asdqwe")); // user log in with emty string email
            Assert.ThrowsException<Exception>(() => userFacade.Login(0, "asd1@gmail.com", "")); // user log in with empty string password

        }

        [TestMethod]
        public void LoginBadTest()
        {

            Assert.IsTrue(userFacade.Login(0, "asd1@gmail.com", "Asdqwe123") != -1); // user log in success
            Assert.ThrowsException<Exception>(() => userFacade.Login(0, "asd1@gmail.com", "Asdqwe123")); // user who is logged in try to log in again
            userFacade.Logout(0);
        }

        [TestCleanup]
        public void CleanUp()
        {
            userFacade.CleanUp();
        }

    }
}
