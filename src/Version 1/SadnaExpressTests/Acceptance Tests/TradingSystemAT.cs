using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.ServiceLayer;
using System;

using SadnaExpressTests;
using static SadnaExpressTests.Mocks;

namespace SadnaExpressTests.Acceptance_Tests
{
    [TestClass()]
    public class TradingSystemAT
    {
        protected ProxyBridge proxyBridge;

        [TestInitialize]
        public void SetUp()
        {
            proxyBridge = new ProxyBridge();
            proxyBridge.SetBridge(new TradingSystem());
        }

        public void activateAdmin()
        {
           //Guid adminID = service.Enter().Value;
           //service.Register(adminID, "Admin@BGU.co.il", "admin", "admin", "admin");
           //Guid registerdAdminID = service.Login(adminID, "Admin@BGU.co.il", "admin").Value;
           //tradingSystemOpen = service.InitializeTradingSystem(registerdAdminID).Value;
           //Guid userID = service.Logout(registerdAdminID).Value;
           //service.Exit(userID);
            
        }

        private class Mock_Bad_SupplierService : Mock_SupplierService
        {
            // bad connection
            public override bool Connect()
            {
                return false;
            }

        }

        private class Mock_Bad_PaymentService : Mock_PaymentService
        {
            // bad connection
            public override bool Connect()
            {
                return false;
            }

        }


        //[TestMethod()]
        //public void TradingSystem_HappyTest()
        //{
        //    proxyBridge.SetPaymentService(new Mock_PaymentService());
        //    proxyBridge.SetSupplierService(new Mock_SupplierService());
        //    proxyBridge.Register(0, "first123@gmail.com", "first", "last", "First1234");
        //  //  Assert.IsTrue(_proxyBridge.Login(0, "first123@gmail.com", "First1234") != -1);

        //    Assert.IsTrue(proxyBridge.CheckSupplierConnection());
        //    Assert.IsTrue(proxyBridge.CheckPaymentConnection());
        //}

        //[TestMethod()]
        //public void TradingSystem_SadTest1()
        //{
        //    proxyBridge.SetPaymentService(new Mock_PaymentService());
        //    proxyBridge.SetSupplierService(new Mock_Bad_SupplierService());
        //    proxyBridge.Register(0, "first123@gmail.com", "first", "last", "First1234");
        //   // Assert.IsTrue(_proxyBridge.Login(0, "first123@gmail.com", "First1234") != -1);

        //    //Assert.True(tradingsystem.login(0, "first123@gmail.com", "First1234") == -1);

        //    Assert.ThrowsException<Exception>(() => proxyBridge.CheckSupplierConnection());
        //    //Assert.False(tradingsystem.checkSupplierConnection());
        //    Assert.IsTrue(proxyBridge.CheckPaymentConnection());
        //}

        //[TestMethod()]
        //public void TradingSystem_SadTest2()
        //{
        //    proxyBridge.SetPaymentService(new Mock_Bad_PaymentService());
        //    proxyBridge.SetSupplierService(new Mock_SupplierService());
        //    proxyBridge.Register(0, "first123@gmail.com", "first", "last", "First1234");
        //   // Assert.IsTrue(_proxyBridge.Login(0, "first123@gmail.com", "First1234") != -1);

        //    Assert.IsTrue(proxyBridge.CheckSupplierConnection());
        //    Assert.ThrowsException<Exception>(() => proxyBridge.CheckPaymentConnection());
        //    //Assert.False(tradingsystem.checkPaymentConnection());
        //}

        [TestCleanup]
        public void CleanUp()
        {
            proxyBridge.CleanUp();
        }

    }
}
