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
        [TestMethod]
        public void PaymentCompletedSuccessfully_GoodTest()
        {
            //Arrange
            Guid id = new Guid();
            proxyBridge.SetIsSystemInitialize(true);
            proxyBridge.SetPaymentService(new Mocks.Mock_5sec_PaymentService());
            //Act
            Task<ResponseT<List<ItemForOrder>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid1, itemid1, 1);
                
                return proxyBridge.PurchaseCart(id, "5411556648", "Rabbi Akiva 5");
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//error not occurred
        }
        [TestMethod]
        public void PaymentIsCollapse_BadTest()
        {
            //Arrange
            Guid id = new Guid();
            proxyBridge.SetIsSystemInitialize(true);
            proxyBridge.SetPaymentService(new Mocks.Mock_Bad_PaymentService());
            //Act
            Task<ResponseT<List<ItemForOrder>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid2, itemid2, 1);
                return proxyBridge.PurchaseCart(id, "5411556648", "Rabbi Akiva 5");
            });
            task.Wait();
            //Assert
            Assert.IsTrue(task.Result.ErrorOccured);//error occurred
        }
        [TestMethod]
        public void PaymentIsOverCreditLimit_BadTest()
        {
            //Arrange
            Guid id = new Guid();
            proxyBridge.SetIsSystemInitialize(true);
            proxyBridge.SetPaymentService(new Mocks.Mock_Bad_Credit_Limit_Payment());
            //Act
            Task<ResponseT<List<ItemForOrder>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid2, itemid2, 1);
                return proxyBridge.PurchaseCart(id, "5411556648", "Rabbi Akiva 5");
            });
            task.Wait();
            //Assert
            Assert.IsTrue(task.Result.ErrorOccured);//error occurred
        }
        #endregion

        #region Supply 1.4
        [TestMethod]
        public void SupplyCompletedSuccessfully_GoodTest()
        {
            //Arrange
            Guid id = new Guid();
            proxyBridge.SetIsSystemInitialize(true);
            proxyBridge.SetSupplierService(new Mocks.Mock_SupplierService());
            //Act
            Task<ResponseT<List<ItemForOrder>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid1, itemid1, 1);
                
                return proxyBridge.PurchaseCart(id, "5411556648", "Rabbi Akiva 5");
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//error not occurred
        }
        [TestMethod]
        public void SupplyIsCollapse_BadTest()
        {
            //Arrange
            Guid id = new Guid();
            proxyBridge.SetIsSystemInitialize(true);
            proxyBridge.SetPaymentService(new Mocks.Mock_Bad_PaymentService());
            //Act
            Task<ResponseT<List<ItemForOrder>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid2, itemid2, 1);
                return proxyBridge.PurchaseCart(id, "5411556648", "Rabbi Akiva 5");
            });
            task.Wait();
            //Assert
            Assert.IsTrue(task.Result.ErrorOccured);//error occurred
        }
        [TestMethod]
        public void SupplyAddressNotCorrect_BadTest()
        {
            //Arrange
            Guid id = new Guid();
            proxyBridge.SetIsSystemInitialize(true);
            proxyBridge.SetSupplierService(new Mocks.Mock_Bad_Address_SupplierService());
            //Act
            Task<ResponseT<List<ItemForOrder>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid2, itemid2, 1);
                return proxyBridge.PurchaseCart(id, "5411556648", "La La Land");
            });
            task.Wait();
            //Assert
            Assert.IsTrue(task.Result.ErrorOccured);//error occurred
        }
        #endregion

        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
        }
    }
}
