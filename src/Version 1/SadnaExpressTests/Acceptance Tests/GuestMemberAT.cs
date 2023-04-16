using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer;

namespace SadnaExpressTests.Acceptance_Tests
{
    [TestClass]
    public class GuestMemberAT: TradingSystemAT
    {
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
        }

        #region Logout 3.1
        [TestMethod]
        public void UserLogout_HappyTest()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                Guid loggedid=proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj").Value;
                return proxyBridge.Logout(loggedid);
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //no error accured 
            Assert.IsFalse(task.Result.Value==Guid.Empty); //when logged out member gets valid id
            Assert.IsNotNull(proxyBridge.GetUser(task.Result.Value).Value); //when member logout he moves to be user in the TS
        }

        [TestMethod]
        public void UserLogoutWithoutEnter_BadTest()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                return proxyBridge.Logout(systemManagerid);
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 
            Assert.IsTrue(task.Result.Value == Guid.Empty); //Guid returns empty (default value)
        }

        [TestMethod]
        public void UserLogoutWithoutLogin_BadTest()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                return proxyBridge.Logout(tempid);
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 
            Assert.IsTrue(task.Result.Value == Guid.Empty); //Guid returns empty (default value)
        }


        [TestMethod]
        public void UserLogoutTwice_BadTest()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                Guid loggedid = proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj").Value;
                proxyBridge.Logout(loggedid);
                return proxyBridge.Logout(loggedid);
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 
            Assert.IsTrue(task.Result.Value == Guid.Empty); //Guid returns empty (default value)
        }

        [TestMethod]
        public void UserLogoutFirstBadSecGood_HappyTest()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                Guid loggedid = proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj").Value;
                proxyBridge.Logout(tempid);
                return proxyBridge.Logout(loggedid);
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //error accured 
            Assert.IsFalse(task.Result.Value == Guid.Empty); //Guid returns empty (default value)
        }
        #endregion

        #region Opening a store 3.2
        #endregion

        #region Writing a review on items the user purchased 3.3
        #endregion

        /// <summary>
        /// Guest buying actions are the same for members with little diffrences 
        /// </summary>

        #region Member Getting information about stores in the market and the products in the stores 2.1
        #endregion

        #region Member search products by general search or filters 2.2
        #endregion

        #region Member saving item in the shopping cart for some store 2.3
        #endregion

        #region Member checking the content of the shopping cart and making changes 2.4
        #endregion

        #region Member making a purchase of the shopping cart 2.5
        #endregion

        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
        }
    }
}