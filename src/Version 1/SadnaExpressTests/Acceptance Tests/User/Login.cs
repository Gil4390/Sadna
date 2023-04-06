using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using System;

namespace SadnaExpressTests.Acceptance_Tests.User
{
    [TestClass]
    public class Login : TradingSystemAT
    {

        [TestInitialize]
        public void SetUp()
        {
            base.SetUp();

        }

        [TestMethod]
        public void LoginHappyTest()
        {
            Assert.IsTrue(proxyBridge.Login(0, "asd1@gmail.com", "Asdqwe123").Value!=-1);
            proxyBridge.Logout(0);
        }

        [TestMethod]
        public void LoginSadTest()
        {

            Assert.ThrowsException<Exception>(() => proxyBridge.Login(0, "asd1@gmail.com", "123asdqwe")); // user log in with wrong password

            Assert.ThrowsException<Exception>(() => proxyBridge.Login(1, "asd1@gmail.com", "Qweqwe123")); // user log in with wrong email


            Assert.ThrowsException<Exception>(() => proxyBridge.Login(-1, "asd1@gmail.com", "123asdqwe")); // user log in with negative id
            Assert.ThrowsException<Exception>(() => proxyBridge.Login(0, "", "123asdqwe")); // user log in with emty string email
            Assert.ThrowsException<Exception>(() => proxyBridge.Login(0, "asd1@gmail.com", "")); // user log in with empty string password

        }

        [TestMethod]
        public void LoginBadTest()
        {

            Assert.IsTrue(proxyBridge.Login(0, "asd1@gmail.com", "Asdqwe123").Value != -1); // user log in success
            Assert.ThrowsException<Exception>(() => proxyBridge.Login(0, "asd1@gmail.com", "Asdqwe123")); // user who is logged in try to log in again
            proxyBridge.Logout(0);
        }

        [TestCleanup]
        public void CleanUp()
        {
            proxyBridge.CleanUp();
        }

    }
}
