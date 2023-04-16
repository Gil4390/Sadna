using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SadnaExpressTests.Mocks;

namespace SadnaExpressTests.Acceptance_Tests
{
    
    [TestClass]
    public class SystemAT : TradingSystemAT
    {
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            proxyBridge.SetIsSystemInitialize(false);
        }

        #region Init Trading System 1.1
        [TestMethod()]
        public void InitTradingSystem_HappyTest()
        {
            //Arrange
            proxyBridge.GetMember(systemManagerid).Value.LoggedIn = true;

            //Act
            Task<ResponseT<bool>> task = Task.Run(() => {
                return proxyBridge.InitializeTradingSystem(systemManagerid);
            });

            task.Wait();

            //Assert
            Assert.IsFalse(task.Result.ErrorOccured); //no error accured 
            Assert.IsTrue(task.Result.Value); //value is true
        }

        [TestMethod()]
        public void InitTradingSystemSystemIsAlreadyInit_BadTest()
        {
            //Arrange
            proxyBridge.GetMember(systemManagerid).Value.LoggedIn = true;
            proxyBridge.SetIsSystemInitialize(true);

            //Act
            Task<ResponseT<bool>> task = Task.Run(() => {
                return proxyBridge.InitializeTradingSystem(systemManagerid);
            });

            task.Wait();

            //Assert
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 
            Assert.IsFalse(task.Result.Value); //value is false
        }

        [TestMethod()]
        public void InitTradingSystemUserIsntSystemManager_BadTest()
        {
            //Arrange
            Guid tempid = Guid.Empty;


            //Act
            Task<ResponseT<bool>> task = Task.Run(() => {
                return proxyBridge.InitializeTradingSystem(tempid);
            });

            task.Wait();

            //Assert
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 
            Assert.IsFalse(task.Result.Value); //value is false
        }

        [TestMethod()]
        public void InitTradingSystemManagerIsAbleToLogInAndInit_HappyTest()
        {
            //Arrange
            Guid tempid = Guid.Empty;


            //Act
            Task<ResponseT<bool>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");
                return proxyBridge.InitializeTradingSystem(systemManagerid);
            });

            task.Wait();

            //Assert
            Assert.IsFalse(task.Result.ErrorOccured); //no error accured 
            Assert.IsTrue(task.Result.Value); //value is true
        }

        [TestMethod()]
        public void InitTradingSystemManagerDidNotLogIn_BadTest()
        {
            //Arrange
            Guid tempid = Guid.Empty;


            //Act
            Task<ResponseT<bool>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                return proxyBridge.InitializeTradingSystem(systemManagerid);
            });

            task.Wait();

            //Assert
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 
            Assert.IsFalse(task.Result.Value); //value is false
        }

        [TestMethod()]
        public void InitTradingSystemManagerDidNotEnter_BadTest()
        {
            //Arrange


            //Act
            Task<ResponseT<bool>> task = Task.Run(() => {
                return proxyBridge.InitializeTradingSystem(systemManagerid);
            });

            task.Wait();

            //Assert
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 
            Assert.IsFalse(task.Result.Value); //value is false
        }

        [TestMethod()]
        public void InitTradingSystemPaymentServiceIsNotConnected_BadTest()
        {
            //Arrange
            proxyBridge.SetPaymentService(new Mock_Bad_PaymentService());
            Guid tempid = Guid.Empty;

            //Act
            Task<ResponseT<bool>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");
                return proxyBridge.InitializeTradingSystem(systemManagerid);
            });

            task.Wait();

            //Assert
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 
            Assert.IsFalse(task.Result.Value); //value is false
        }

        [TestMethod()]
        public void InitTradingSystemSupplyServiceIsNotConnected_BadTest()
        {
            //Arrange
            proxyBridge.SetSupplierService(new Mock_Bad_SupplierService());
            Guid tempid = Guid.Empty;

            //Act
            Task<ResponseT<bool>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");
                return proxyBridge.InitializeTradingSystem(systemManagerid);
            });

            task.Wait();

            //Assert
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 
            Assert.IsFalse(task.Result.Value); //value is false
        }
        #endregion

        #region Payment 1.3
        #endregion

        #region Supply 1.4
        #endregion

        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
        }
    }
}
