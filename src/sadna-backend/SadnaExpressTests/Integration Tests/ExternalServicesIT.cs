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
            trading.SetPaymentService(new PaymentService());
            trading.SetSupplierService(new SupplierService());
            base.Setup();
        }

        /// <summary>
        /// Check handshake if success should return OK
        /// </summary>
        [TestMethod]
        public void CheckHandshake()
        {
            string res = trading.Handshake().ErrorMessage;
            Assert.IsTrue(res == "OK");
        }
        /// <summary>
        /// Check handshake and send it wrong params - expect fail
        /// </summary>
        [TestMethod]
        public void CheckHandshakeWrongParams()
        {
            string res = trading.Handshake().ErrorMessage;
            Assert.IsFalse(res == "OK");
        }
        
        /// <summary>
        /// Check handshake and send it , wait for 15 sec - expect fail
        /// </summary>
        [TestMethod]
        public void CheckHandshakePassLimitedTime()
        {
            string res = trading.Handshake().ErrorMessage;
            Assert.AreEqual("OK", res);
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

    }
}