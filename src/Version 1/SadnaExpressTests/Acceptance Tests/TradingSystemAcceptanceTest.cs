using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.ServiceLayer;
using System;

using SadnaExpressTests;
using static SadnaExpressTests.Mocks;

namespace SadnaExpressTests.Acceptance_Tests
{
    [TestClass()]
    public class TradingsystemTest
    {


        private ProxyBridge _proxyBridge;

        public TradingsystemTest()
        {
            _proxyBridge = new ProxyBridge();
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


        [TestMethod()]
        public void TradingSystem_HappyTest()
        {
            _proxyBridge.SetPaymentService(new Mock_PaymentService());
            _proxyBridge.SetSupplierService(new Mock_SupplierService());
            _proxyBridge.Register(0, "first123@gmail.com", "first", "last", "First1234");
          //  Assert.IsTrue(_proxyBridge.Login(0, "first123@gmail.com", "First1234") != -1);

            Assert.IsTrue(_proxyBridge.CheckSupplierConnection());
            Assert.IsTrue(_proxyBridge.CheckPaymentConnection());
        }

        [TestMethod()]
        public void TradingSystem_SadTest1()
        {
            _proxyBridge.SetPaymentService(new Mock_PaymentService());
            _proxyBridge.SetSupplierService(new Mock_Bad_SupplierService());
            _proxyBridge.Register(0, "first123@gmail.com", "first", "last", "First1234");
           // Assert.IsTrue(_proxyBridge.Login(0, "first123@gmail.com", "First1234") != -1);

            //Assert.True(tradingsystem.login(0, "first123@gmail.com", "First1234") == -1);

            Assert.ThrowsException<Exception>(() => _proxyBridge.CheckSupplierConnection());
            //Assert.False(tradingsystem.checkSupplierConnection());
            Assert.IsTrue(_proxyBridge.CheckPaymentConnection());
        }

        [TestMethod()]
        public void TradingSystem_SadTest2()
        {
            _proxyBridge.SetPaymentService(new Mock_Bad_PaymentService());
            _proxyBridge.SetSupplierService(new Mock_SupplierService());
            _proxyBridge.Register(0, "first123@gmail.com", "first", "last", "First1234");
           // Assert.IsTrue(_proxyBridge.Login(0, "first123@gmail.com", "First1234") != -1);

            Assert.IsTrue(_proxyBridge.CheckSupplierConnection());
            Assert.ThrowsException<Exception>(() => _proxyBridge.CheckPaymentConnection());
            //Assert.False(tradingsystem.checkPaymentConnection());
        }

        [TestCleanup]
        public void CleanUp()
        {
            _proxyBridge.CleanUp();
        }

    }
}
