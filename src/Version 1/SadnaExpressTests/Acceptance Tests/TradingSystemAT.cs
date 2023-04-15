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
        public virtual void SetUp()
        {
            proxyBridge = new ProxyBridge();
            proxyBridge.SetBridge(new TradingSystem());
            proxyBridge.SetPaymentService(new Mock_PaymentService());
            proxyBridge.SetSupplierService(new Mock_SupplierService());
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

        [TestCleanup]
        public virtual void CleanUp()
        {
            proxyBridge.CleanUp();
        }

    }
}
