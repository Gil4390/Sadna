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


        private static Tradingsystem tradingsystem;

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
            tradingsystem = new Tradingsystem(new Mock_SupplierService(), new Mock_PaymentService());

            tradingsystem.register(0, "first123@gmail.com", "first", "last", "First1234");
            Assert.IsTrue(tradingsystem.login(0, "first123@gmail.com", "First1234") != -1);

            Assert.IsTrue(tradingsystem.checkSupplierConnection());
            Assert.IsTrue(tradingsystem.checkPaymentConnection());
        }

        [TestMethod()]
        public void TradingSystem_SadTest1()
        {
            tradingsystem = new Tradingsystem(new Mock_Bad_SupplierService(), new Mock_PaymentService());
            tradingsystem.register(0, "first123@gmail.com", "first", "last", "First1234");
            Assert.IsTrue(tradingsystem.login(0, "first123@gmail.com", "First1234") != -1);

            //Assert.True(tradingsystem.login(0, "first123@gmail.com", "First1234") == -1);
            
            Assert.ThrowsException<Exception>(() => tradingsystem.checkSupplierConnection());
            //Assert.False(tradingsystem.checkSupplierConnection());
            Assert.IsTrue(tradingsystem.checkPaymentConnection());
        }

        [TestMethod()]
        public void TradingSystem_SadTest2()
        {
            tradingsystem = new Tradingsystem(new Mock_SupplierService(), new Mock_Bad_PaymentService());

            tradingsystem.register(0, "first123@gmail.com", "first", "last", "First1234");
            Assert.IsTrue(tradingsystem.login(0, "first123@gmail.com", "First1234") != -1);

            Assert.IsTrue(tradingsystem.checkSupplierConnection());
            Assert.ThrowsException<Exception>(() => tradingsystem.checkPaymentConnection());
            //Assert.False(tradingsystem.checkPaymentConnection());
        }

        [TestCleanup]
        public void CleanUp()
        {
            tradingsystem.CleanUp();
        }

    }
}
