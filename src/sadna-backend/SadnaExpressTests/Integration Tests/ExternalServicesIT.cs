using System.Collections.Generic;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.ExternalServices;
using SadnaExpress.ServiceLayer.SModels;

namespace SadnaExpressTests.Integration_Tests
{
    [TestClass]
    public class ExternalServicesIT: TradingSystemIT
    {
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
        }

        /// <summary>
        /// Check handshake if success should return OK
        /// </summary>
        [TestMethod]
        public void CheckHandshake()
        {
            trading.SetPaymentService(new Mocks.Mock_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_SupplierService());
            string res = trading.Handshake().ErrorMessage;
            Assert.IsTrue(res == "OK");
        }
        /// <summary>
        /// Check handshake and send it wrong params - expect fail
        /// </summary>
        [TestMethod]
        public void CheckHandshakeWrongParams()
        {
            trading.SetPaymentService(new Mocks.Mock_bad_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_SupplierService());
            string res = trading.Handshake().ErrorMessage;
            Assert.IsFalse(res == "OK");
        }
        
        /// <summary>
        /// Check handshake and send it , wait for 15 sec - expect fail
        /// </summary>
        [TestMethod]
        public void CheckHandshakePassLimitedTime()
        {
            trading.SetPaymentService(new Mocks.Mock_bad_15sec_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_SupplierService());
            string res = trading.Handshake().ErrorMessage;
            Assert.AreEqual("OK", res);
        }
        
        /// <summary>
        /// Check Pay with valid params
        /// </summary>
        [TestMethod]
        public void CheckPaySuccess()
        {
            trading.SetPaymentService(new Mocks.Mock_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_SupplierService());
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
            trading.SetPaymentService(new Mocks.Mock_bad_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_SupplierService());
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
            trading.SetPaymentService(new Mocks.Mock_bad_15sec_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_SupplierService());
            SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
            SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");
            Assert.IsTrue(trading.PurchaseCart(buyerID, transactionDetails, transactionDetailsSupply).ErrorOccured);
        }
        
                /// <summary>
        /// Check Pay with valid params
        /// </summary>
        [TestMethod]
        public void CheckSupplySuccess()
        {
            trading.SetPaymentService(new Mocks.Mock_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_SupplierService());
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
            trading.SetPaymentService(new Mocks.Mock_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_Bad_SupplierService());
            SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
            SSupplyDetails transactionDetailsSupply = new SSupplyDetails("-", "-","-","-","-");
            Assert.IsTrue(trading.PurchaseCart(buyerID, transactionDetails, transactionDetailsSupply).ErrorOccured);
        }
        
    }
}