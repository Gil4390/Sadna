using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer;
using static SadnaExpressTests.Mocks;

namespace SadnaExpressTests.Acceptance_Tests
{
    [TestClass]
    public class GuestMemberSystemManagerAT : TradingSystemAT
    {
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
        }

        #region Purchases information history
        [TestMethod]
        public void SystemManagerRequestPurchasesInformationhistory_HappyTest()
        {
            //Arrange
            Mock_Orders mock_Orders = new Mock_Orders();
            proxyBridge.SetTSOrders(mock_Orders);

            Guid tempid = Guid.Empty;
            Task<ResponseT<Dictionary<Guid, List<Order>>>> task = Task<ResponseT<Dictionary<Guid, List<Order>>>>.Run(() => {
                tempid = proxyBridge.Enter().Value;
                proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");
                return proxyBridge.GetAllStorePurchases(systemManagerid);
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //no error accured - system manager is able to preform this action 
            Assert.IsTrue(task.Result.Value.Count == 0); //there are no orders history
        }

        [TestMethod]
        public void NoSystemManagerRequestPurchasesInformationhistory_BadTest()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Dictionary<Guid, List<Order>>>> task = Task<ResponseT<Dictionary<Guid, List<Order>>>>.Run(() => {
                tempid = proxyBridge.Enter().Value;
                proxyBridge.Login(tempid, "gil@gmail.com", "asASD876!@");
                return proxyBridge.GetAllStorePurchases(memberId);
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error accured - only system manager is able to preform this action 
        }

        [TestMethod]
        public void SystemManagerRequestPurchasesInformationhistoryGetInfo_HappyTest()
        {
            //Arrange
            Mock_Orders mock_Orders = new Mock_Orders();
            mock_Orders.AddOrderToStore(storeid1, new Order(new List<ItemForOrder> { new ItemForOrder(proxyBridge.GetItemByID(storeid1, itemid1).Value, storeid1) }));
            mock_Orders.AddOrderToStore(storeid2, new Order(new List<ItemForOrder> { new ItemForOrder(proxyBridge.GetItemByID(storeid2, itemid2).Value, storeid2) }));
            proxyBridge.SetTSOrders(mock_Orders);

            Guid tempid = Guid.Empty;
            Task<ResponseT<Dictionary<Guid, List<Order>>>> task = Task<ResponseT<Dictionary<Guid, List<Order>>>>.Run(() => {
                tempid = proxyBridge.Enter().Value;
                proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");
                return proxyBridge.GetAllStorePurchases(systemManagerid);
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //error accured - only system manager is able to preform this action 
            Assert.IsTrue(task.Result.Value.Count == 2); //there are 2 store orders history
        }

        #endregion

        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
        }
    }
}