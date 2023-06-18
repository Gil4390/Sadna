using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ExternalServices;
using SadnaExpress.ServiceLayer;
using SadnaExpress.ServiceLayer.SModels;
using SadnaExpress.Services;

namespace SadnaExpressTests.Integration_Tests
{
    [TestClass]
    public class ExternalServicesIT: TradingSystemIT
    {
        private PaymentService paymentService;
        private SupplierService supplierService;

      [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            paymentService = new PaymentService("https://php-server-try.000webhostapp.com/");
            supplierService = new SupplierService("https://php-server-try.000webhostapp.com/");
            trading.SetPaymentService(paymentService);
            trading.SetSupplierService(supplierService);
        }

        /// <summary>
        /// Check handshake if success should return OK
        /// </summary>
        [TestMethod]
        public void CheckHandshake()
        {
            string res = paymentService.Handshake();
            Assert.IsTrue(res == "OK");
        }
        
        /// <summary>
        /// Check handshake and send it , wait for 15 sec - expect fail
        /// </summary>
        [TestMethod]
        public void CheckHandshakePassLimitedTime()
        {
            Task<string> task = Task.Run(async () =>
            {
                Thread.Sleep(11000);
                string res;
                return res = paymentService.Handshake(); ;
            });

            bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(10000)) && task.Result == "OK"; ;
            Assert.AreEqual("OK", task.Result);
            Assert.IsFalse(isCompletedSuccessfully);
        }
        
        /// <summary>
        /// Check Pay with valid params
        /// </summary>
        [TestMethod]
        public void CheckPaySuccess()
        {
            SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
            SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");
            Assert.IsFalse(trading.PurchaseCart(buyerID, transactionDetails, transactionDetailsSupply).ErrorOccured);
        }
        /// <summary>
        /// Check Pay with invalid params
        /// </summary>
        [TestMethod]
        public void CheckPayWrongParams()
        {
            SPaymentDetails transactionDetails = new SPaymentDetails("-", "-","-","-","-","-");
            SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");
            Assert.IsTrue(trading.PurchaseCart(buyerID, transactionDetails, transactionDetailsSupply).ErrorOccured);
        }
        
        /// <summary>
        /// Check Pay with pass limit time
        /// </summary>
        [TestMethod]
        public void CheckPayWrongPassLimitedTime()
        {

            SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "984", "123456789");
            SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");
            Assert.IsTrue(trading.PurchaseCart(buyerID, transactionDetails, transactionDetailsSupply).ErrorOccured);
        }
        
                /// <summary>
        /// Check Pay with valid params
        /// </summary>
        [TestMethod]
        public void CheckSupplySuccess()
        {
            SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
            SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");
            Assert.IsFalse(trading.PurchaseCart(buyerID, transactionDetails, transactionDetailsSupply).ErrorOccured);
        }
        /// <summary>
        /// Check Supply with invalid params
        /// </summary>
        [TestMethod]
        public void CheckSupplyWrongParams()
        {

            SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "986", "123456789");
            SSupplyDetails transactionDetailsSupply = new SSupplyDetails("-", "-","-","-","-");
            Assert.IsTrue(trading.PurchaseCart(buyerID, transactionDetails, transactionDetailsSupply).ErrorOccured);
        }

        public void CheckPaySuccess_Mock()
        {
            trading.SetPaymentService(new Mocks.Mock_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_SupplierService());
            SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
            SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent", "38 Tacher st.", "Richmond", "England", "4284200");
            Assert.IsFalse(trading.PurchaseCart(buyerID, transactionDetails, transactionDetailsSupply).ErrorOccured);
        }
        /// <summary>
        /// Check Pay with invalid params
        /// </summary>
        [TestMethod]
        public void CheckPayWrongParams_Mock()
        {
            trading.SetPaymentService(new Mocks.Mock_bad_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_SupplierService());
            SPaymentDetails transactionDetails = new SPaymentDetails("-", "-", "-", "-", "-", "-");
            SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent", "38 Tacher st.", "Richmond", "England", "4284200");
            Assert.IsTrue(trading.PurchaseCart(buyerID, transactionDetails, transactionDetailsSupply).ErrorOccured);
        }


        /// <summary>
        /// Check Pay with valid params
        /// </summary>
        [TestMethod]
        public void CheckSupplySuccess_Mocm()
        {
            trading.SetPaymentService(new Mocks.Mock_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_SupplierService());
            SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
            SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent", "38 Tacher st.", "Richmond", "England", "4284200");
            Assert.IsFalse(trading.PurchaseCart(buyerID, transactionDetails, transactionDetailsSupply).ErrorOccured);
        }
        /// <summary>
        /// Check Supply with invalid params
        /// </summary>
        [TestMethod]
        public void CheckSupplyWrongParams_Mock()
        {
            trading.SetPaymentService(new Mocks.Mock_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_Bad_SupplierService());
            SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "986", "123456789");
            SSupplyDetails transactionDetailsSupply = new SSupplyDetails("-", "-", "-", "-", "-");
            Assert.IsTrue(trading.PurchaseCart(buyerID, transactionDetails, transactionDetailsSupply).ErrorOccured);
        }

    }
}