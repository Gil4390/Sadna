using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.ServiceLayer;

namespace SadnaExpressTests.Integration_Tests
{
    [TestClass]
    public class CheckPurchaseConditionIT: TradingSystemIT
    {
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
        }

        [TestMethod()]
        public void UserTryToPurchaseCartSuccess()
        {
            //Arrange
            trading.AddCondition(storeID1, "Item", "ipad 32", "min quantity", 1);
            //Act
            Response task1 = trading.CheckPurchaseConditions(buyerID);
            //Assert
            Assert.IsFalse(task1.ErrorOccured);
        }
        
        [TestMethod()]
        public void UserTryToPurchaseCartFail()
        {
            //Arrange
            trading.AddCondition(storeID1, "Item", "ipad 32","min quantity", 3, DateTime.MaxValue);
            //Act
            Response task1 = trading.CheckPurchaseConditions(buyerID);
            //Assert
            Assert.IsTrue(task1.ErrorOccured);
            Assert.AreEqual("The quantity of item ipad 32 is 2 while the minimum quantity is 3", task1.ErrorMessage);
        }
        
        [TestMethod()]
        public void UserTryToPurchaseCartOneCondFail()
        {
            //Arrange
            trading.AddCondition(storeID1, "Item", "ipad 32","min quantity", 1, DateTime.MaxValue);
            trading.AddCondition(storeID2, "Store", "","min value", 5000, DateTime.MaxValue);

            //Act
            Response task1 = trading.CheckPurchaseConditions(buyerID);
            //Assert
            Assert.IsTrue(task1.ErrorOccured);
            Assert.AreEqual("The price of store hello2 is 3000 while the minimum price is 5000", task1.ErrorMessage);
        }
        
        [TestMethod()]
        public void UserTryToPurchaseCartTwoCondFail()
        {  
            //Arrange
            trading.AddCondition(storeID1, "Item", "ipad 32","min quantity", 3, DateTime.MaxValue);
            trading.AddCondition(storeID1, "Store", "","min value", 5000, DateTime.MaxValue);

            //Act
            Response task1 = trading.CheckPurchaseConditions(buyerID);
            //Assert
            Assert.IsTrue(task1.ErrorOccured);
            Assert.AreEqual("The quantity of item ipad 32 is 2 while the minimum quantity is 3", task1.ErrorMessage);
        }
    }
}