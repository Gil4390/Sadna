using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using SadnaExpress.ServiceLayer.Obj;
using SadnaExpress.ServiceLayer.SModels;
using SadnaExpressTests.Unit_Tests;
using static SadnaExpressTests.Mocks;

namespace SadnaExpressTests.Acceptance_Tests
{
    [TestClass]
    public class GuestMemberSystemManagerAT : TradingSystemAT
    {
        private UserUsageData userUsageData;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            userUsageData = UserUsageData.Instance;
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
            mock_Orders.AddOrderToStore(storeid1, new Order(new List<ItemForOrder> { new ItemForOrder(proxyBridge.GetItemByID(storeid1, itemid1).Value, storeid1, "RotemSela@gmail.com" ,"Zara") }, Guid.Empty));
            mock_Orders.AddOrderToStore(storeid2, new Order(new List<ItemForOrder> { new ItemForOrder(proxyBridge.GetItemByID(storeid2, itemid2).Value, storeid2, "RotemSela@gmail.com","Fox") }, Guid.Empty));
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

        #region Get Current Members info
       
        [TestMethod]
        public void StoreOwnerRequestMembersInformation_SadTest()
        {
            Task<ResponseT<List<SMember>>> task = Task<ResponseT<List<SMember>>>.Run(() => {
                Guid tempid = proxyBridge.Enter().Value;
                proxyBridge.Login(tempid, "bar@gmail.com",  "asASD159!@");
                return proxyBridge.GetMembers(memberId4);
            });
            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error accured - only system manager is able to preform this action 
        }
        #endregion
        
        #region Remove membership
        [TestCategory("Concurrency")]
        
        public void SystemManagerRemoveMembershipDuringHisPurchase_HappyTest()
        {
            //Arrange
            Guid enterId = proxyBridge.Enter().Value;
            proxyBridge.Login(enterId,"gil@gmail.com", "asASD876!@");
            proxyBridge.AddItemToCart(memberId, storeid1, itemid1, 1);
            proxyBridge.AddItemToCart(memberId, storeid2, itemid2, 1);
            Guid tempid = proxyBridge.Enter().Value;
            proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");
            Random random = new Random();
            int randomSleep = random.Next(0, 3);
            
            //Act
            Task<Response> task1 = Task.Run(() =>
            {
                Thread.Sleep(randomSleep); //in order to be in all the levels on purchase
                return proxyBridge.RemoveUserMembership(systemManagerid, "gil@gmail.com");
            });
            Task<ResponseT<List<ItemForOrder>>> task2 = Task.Run(() =>
            {
                SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                return proxyBridge.PurchaseCart(memberId, transactionDetails, transactionDetailsSupply);
            });
            Task.WaitAll();

            Assert.IsFalse(task1.Result.ErrorOccured || task2.Result.ErrorOccured); //no error should happen
            Assert.IsTrue(proxyBridge.GetMember(memberId).ErrorOccured); // the member should erase
            Assert.IsNotNull(proxyBridge.GetUser(memberId)); //the user still need to be in the system
            Assert.AreEqual(0, proxyBridge.GetDetailsOnCart(memberId).Value.Baskets.Count); // the shopping cart need to be empty because the user buy 
            // check order details
            Assert.AreEqual(2, proxyBridge.GetAllStorePurchases(systemManagerid).Value.Count);
            Assert.IsTrue(proxyBridge.GetAllStorePurchases(systemManagerid).Value.ContainsKey(storeid1));
            Assert.IsTrue(proxyBridge.GetAllStorePurchases(systemManagerid).Value.ContainsKey(storeid2));
            Assert.AreEqual(itemid1,task2.Result.Value[0].ItemID);
            Assert.AreEqual(itemid2, task2.Result.Value[1].ItemID);
            Logger.Instance.Info("*****************************************************************************");
        }
        
        [TestCategory("Concurrency")]
        [TestMethod]
        public void SystemManagerRemoveMembershipDuringLogin_HappyTest()
        {
            //Arrange
            Guid enterId = proxyBridge.Enter().Value;
            Guid tempid = proxyBridge.Enter().Value;
            proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");
            Random random = new Random();
            int randomSleep = random.Next(0, 2);
            
            //Act
            Task<Response> task1 = Task.Run(() =>
            {
                Thread.Sleep(randomSleep); //in order to be in both situations
                return proxyBridge.RemoveUserMembership(systemManagerid, "gil@gmail.com");
            });
            Task<ResponseT<Guid>> task2 = Task.Run(() =>
            {
                return proxyBridge.Login(enterId, "gil@gmail.com", "asASD876!@");
            });
            Task.WaitAll();
            
            // 2 situations: 1. The member login and then removed.
            //               2. The member removed so when he try login, get an error.                     
            bool situation1 = !task1.Result.ErrorOccured && !task2.Result.ErrorOccured;
            bool situation2 = !task1.Result.ErrorOccured && task2.Result.ErrorOccured;
            Assert.IsTrue(situation1 || situation2);
            if (situation1)
                Assert.IsNotNull(proxyBridge.GetUser(memberId).Value); //the user still need to be in the system
            Assert.IsTrue(proxyBridge.GetMember(memberId).ErrorOccured); // the member should erase
            Logger.Instance.Info("*****************************************************************************");
        }
        [TestCategory("Concurrency")]
        [TestMethod]
        public void SystemManagerRemoveMembershipDuringAddPermissions_HappyTest()
        {
            //Arrange
            Guid enterId = proxyBridge.Enter().Value;
            proxyBridge.Login(enterId, "AsiAzar@gmail.com", "A#!a12345678");
            Guid tempid = proxyBridge.Enter().Value;
            proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");
            Random random = new Random();
            int randomSleep = random.Next(0, 2);
            
            //Act
            Task<Response> task1 = Task.Run(() => 
            {
                Thread.Sleep(randomSleep); //in order to be in both situations
                return proxyBridge.AppointStoreManager(storeOwnerid, storeid1,"gil@gmail.com");
            });
            Task<Response> task2 = Task.Run(() =>
            {
                return proxyBridge.RemoveUserMembership(systemManagerid, "gil@gmail.com");
            });
            Task.WaitAll();

            // 2 situations: 1. The owner first make him store owner and then the system manger can't remove him.
            //               2.  the system manger remove him and then the owner can't             
            bool situation1 = !task1.Result.ErrorOccured && task2.Result.ErrorOccured;
            bool situation2 = task1.Result.ErrorOccured && !task2.Result.ErrorOccured;
            Assert.IsTrue(situation1 || situation2);
            if (situation2)
            {
                Assert.IsTrue(proxyBridge.GetMember(memberId).ErrorOccured); // the member should erase
            }
            else
            {
               Assert.IsTrue(proxyBridge.GetMember(memberId).Value.hasPermissions(storeid1, new List<string>{"get store history"})); // the member should erase
               Assert.IsNotNull(proxyBridge.GetMember(memberId).Value); //the member still need to be in the system 
            }
        }
        [TestCategory("Concurrency")]
        [TestMethod]
        public void SystemManagerRemoveMembershipDuringRemovePermissions_HappyTest()
        {
            //Arrange
            Guid enterId = proxyBridge.Enter().Value;
            proxyBridge.Login(enterId, "AsiAzar@gmail.com", "A#!a12345678");
            Guid tempid = proxyBridge.Enter().Value;
            proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");
            proxyBridge.AppointStoreOwner(storeOwnerid, storeid1,"gil@gmail.com");
            Random random = new Random();
            int randomSleep = random.Next(0, 2);
            
            //Act
            Task<Response> task1 = Task.Run(() => 
            {
                return proxyBridge.RemovePermission(storeOwnerid, storeid1,"gil@gmail.com", "owner permissions");
            });
            Task<Response> task2 = Task.Run(() =>
            {
                Thread.Sleep(randomSleep); //in order to be in both situations
                return proxyBridge.RemoveUserMembership(systemManagerid, "gil@gmail.com");
            });
            Task.WaitAll();

            // 2 situations: 1. The owner first remove his permission and then the system manger remove him.
            //               2.  the system manger can't remove him and then the owner remove his permission             
            bool situation1 = !task1.Result.ErrorOccured && task2.Result.ErrorOccured;
            bool situation2 = !task1.Result.ErrorOccured && !task2.Result.ErrorOccured;
            Assert.IsTrue(situation1 || situation2);
            if (situation2)
            {
                Assert.IsTrue(proxyBridge.GetMember(memberId).ErrorOccured); // the member should erase
            }
            else
            {
                Assert.IsFalse(proxyBridge.GetMember(memberId).Value.hasPermissions(storeid1, new List<string>{"owner permissions"})); // the member should erase
                Assert.IsNotNull(proxyBridge.GetMember(memberId).Value); //the member still need to be in the system 
            }
            Logger.Instance.Info("*****************************************************************************");
        }
        #endregion
        
        #region Get system revenue

        [TestMethod]
        public void SystemManagerGet2StoreAnd2UsersRevenue_HappyTest()
        {
            Guid tempid = proxyBridge.Enter().Value;
            proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");
            
            Assert.AreEqual(0, proxyBridge.GetSystemRevenue(systemManagerid, DateTime.Today).Value);
            
            Guid id = proxyBridge.Enter().Value;
            proxyBridge.AddItemToCart(id, storeid2, itemid2, 1);
            SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
            SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

            proxyBridge.PurchaseCart(id, transactionDetails, transactionDetailsSupply);
            
            Assert.AreEqual(150, proxyBridge.GetSystemRevenue(systemManagerid, DateTime.Today).Value);
            
            Guid id2 = proxyBridge.Enter().Value;
            proxyBridge.AddItemToCart(id2, storeid1, itemid1, 3);
            proxyBridge.PurchaseCart(id2, transactionDetails, transactionDetailsSupply);
            
            Assert.AreEqual(299.7, proxyBridge.GetSystemRevenue(systemManagerid, DateTime.Today).Value);
        }
        
        [TestMethod]
        public void SystemManagerGet1UserAnd2StoreInSamePurchaseRevenue_HappyTest()
        {
            Guid tempid = proxyBridge.Enter().Value;
            proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");
            
            Assert.AreEqual(0, proxyBridge.GetSystemRevenue(systemManagerid, DateTime.Today).Value);
            
            Guid id = proxyBridge.Enter().Value;
            proxyBridge.AddItemToCart(id, storeid2, itemid2, 1);
            proxyBridge.AddItemToCart(id, storeid1, itemid1, 3);
            SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
            SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

            proxyBridge.PurchaseCart(id, transactionDetails, transactionDetailsSupply);

            Assert.AreEqual(299.7, proxyBridge.GetSystemRevenue(systemManagerid, DateTime.Today).Value);
        }

        [TestMethod]
        public void SystemManagerGet2UsersAnd1StoreRevenue_HappyTest()
        {
            Guid tempid = proxyBridge.Enter().Value;
            proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");
            
            Assert.AreEqual(0, proxyBridge.GetSystemRevenue(systemManagerid, new DateTime(2023, 05, 8)).Value);
            
            Guid id = proxyBridge.Enter().Value;
            proxyBridge.AddItemToCart(id, storeid2, itemid2, 1);
            SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
            SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

            proxyBridge.PurchaseCart(id, transactionDetails, transactionDetailsSupply);
            
            Assert.AreEqual(150, proxyBridge.GetSystemRevenue(systemManagerid, DateTime.Today).Value);
            
            Guid id2 = proxyBridge.Enter().Value;
            proxyBridge.AddItemToCart(id2, storeid2, itemid2, 3);
            proxyBridge.PurchaseCart(id2, transactionDetails, transactionDetailsSupply);
            
            Assert.AreEqual(600, proxyBridge.GetSystemRevenue(systemManagerid, DateTime.Today).Value);
        }
        #endregion

        #region System activity

        [TestMethod]
        [TestCategory("Concurrency")]
        public void MultipleClientsEnter_HappyTest()

        {
            Guid tempid = proxyBridge.Enter().Value;
            proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");

            int guestCount = userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[0];

            //Arrange
            Task<ResponseT<Guid>>[] clientTasks = new Task<ResponseT<Guid>>[] {
                Task.Run(() => proxyBridge.Enter()),
                Task.Run(() => proxyBridge.Enter()),
                Task.Run(() =>  proxyBridge.Enter()),
                Task.Run(() =>  proxyBridge.Enter()),
                Task.Run(() =>  proxyBridge.Enter()),
                Task.Run(() =>  proxyBridge.Enter())
             };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsFalse(clientTasks[0].Result.ErrorOccured);
            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);
            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);
            Assert.IsFalse(clientTasks[3].Result.ErrorOccured);
            Assert.IsFalse(clientTasks[4].Result.ErrorOccured);
            Assert.IsFalse(clientTasks[5].Result.ErrorOccured);

            Assert.IsTrue(proxyBridge.GetSystemUserActivity(systemManagerid, DateTime.Today, DateTime.Today).Value[0] == guestCount + 6
                 && proxyBridge.GetSystemUserActivity(systemManagerid, DateTime.Today, DateTime.Today).Value[4] == 1);
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void MultipleClientsEnterAndLogin_HappyTest()
        {
            //Arrange
            Guid tempid = proxyBridge.Enter().Value;
            proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");

            int guestCount = userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[0];
            int memberCount = userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[1];

            Task<ResponseT<Guid>>[] clientTasks = new Task<ResponseT<Guid>>[] {
                Task.Run(() =>{ Guid guestId=proxyBridge.Enter().Value; return proxyBridge.Login(guestId, "sebatian@gmail.com", "asASD123!@"); }),
                Task.Run(() => proxyBridge.Enter()),
                Task.Run(() =>  proxyBridge.Enter()),
                Task.Run(() =>  {Guid guestId=proxyBridge.Enter().Value; return proxyBridge.Login(guestId, "bar@gmail.com", "asASD159!@"); }),
                Task.Run(() =>  proxyBridge.Enter()),
                Task.Run(() =>  proxyBridge.Enter())
             };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsFalse(clientTasks[0].Result.ErrorOccured);
            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);
            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);
            Assert.IsFalse(clientTasks[3].Result.ErrorOccured);
            Assert.IsFalse(clientTasks[4].Result.ErrorOccured);
            Assert.IsFalse(clientTasks[5].Result.ErrorOccured);

            Assert.IsTrue(proxyBridge.GetSystemUserActivity(systemManagerid, DateTime.Today, DateTime.Today).Value[0] == guestCount+4
                && proxyBridge.GetSystemUserActivity(systemManagerid, DateTime.Today, DateTime.Today).Value[1] == memberCount+2
                && proxyBridge.GetSystemUserActivity(systemManagerid, DateTime.Today, DateTime.Today).Value[4]==1);
        }


        #endregion

        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
        }
    }
}