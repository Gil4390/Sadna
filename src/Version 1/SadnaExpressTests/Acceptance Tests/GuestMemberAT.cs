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
        [TestMethod]
        public void MemberOpenStore_HappyTest()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                Guid loggedid = proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj").Value;
                return proxyBridge.OpenNewStore(loggedid,"My store");
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //no error accured 
            Assert.IsFalse(task.Result.Value == Guid.Empty); 
            Assert.IsNotNull(proxyBridge.GetStore(task.Result.Value)); //check that store exist
        }

        [TestMethod]
        public void MemberOpenStoreWithEmptyStoreName_BadTest()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                Guid loggedid = proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj").Value;
                return proxyBridge.OpenNewStore(loggedid, "");
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //no error accured 
            Assert.IsTrue(task.Result.Value == Guid.Empty);
            Assert.IsTrue(proxyBridge.GetStore(task.Result.Value).ErrorOccured); //check that store does not exists
        }

        [TestMethod]
        public void MemberOpenStoreNameThatAlreadyExist_BadTest()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                Guid loggedid = proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj").Value;
                return proxyBridge.OpenNewStore(loggedid, "Zara");
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 
            Assert.IsTrue(task.Result.Value == Guid.Empty);
            Assert.IsTrue(proxyBridge.GetStore(task.Result.Value).ErrorOccured); //check that store does not exists
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void MultipleMembersAddStores_HappyTest()
        {
            Guid id1 = Guid.Empty;
            Guid id2 = Guid.Empty;
            Guid id3 = Guid.Empty;
            Guid id4 = Guid.Empty;
            Task<ResponseT<Guid>>[] clientTasks = new Task<ResponseT<Guid>>[] {
                Task.Run(() => {
                    id1=proxyBridge.Enter().Value;
                    Guid loggedid = proxyBridge.Login(id1, "RotemSela@gmail.com", "AS87654askj").Value;
                    return proxyBridge.OpenNewStore(loggedid, "Zara");
                }),
                Task.Run(() => {
                    id2=proxyBridge.Enter().Value;
                    Guid loggedid = proxyBridge.Login(id2, "gil@gmail.com", "asASD876!@").Value;
                    return proxyBridge.OpenNewStore(loggedid, "Apple");
                }),
                Task.Run(() => {
                    id3=proxyBridge.Enter().Value;
                    proxyBridge.Register(id3, "tal.galmor@weka.io","tal", "galmor", "123AaC!@#");
                    Guid loggedid = proxyBridge.Login(id3, "tal.galmor@weka.io", "123AaC!@#").Value;
                    return proxyBridge.OpenNewStore(loggedid, "Jumbo");
                })
             };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsTrue(clientTasks[0].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetStore(clientTasks[0].Result.Value).ErrorOccured);

            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);//no error occurred
            Assert.IsNotNull(proxyBridge.GetStore(clientTasks[1].Result.Value));

            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);//no error occurred
            Assert.IsNotNull(proxyBridge.GetStore(clientTasks[2].Result.Value));
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void MultipleMembersAddStoresSameName_HappyTest()
        {
            Guid id1 = Guid.Empty;
            Guid id2 = Guid.Empty;
            Task<ResponseT<Guid>>[] clientTasks = new Task<ResponseT<Guid>>[] {
                Task.Run(() => {
                    id1=proxyBridge.Enter().Value;
                    Guid loggedid = proxyBridge.Login(id1, "gil@gmail.com", "asASD876!@").Value;
                    return proxyBridge.OpenNewStore(loggedid, "Jumbo");
                }),
                Task.Run(() => {
                    id2=proxyBridge.Enter().Value;
                    proxyBridge.Register(id2, "tal.galmor@weka.io","tal", "galmor", "123AaC!@#");
                    Guid loggedid = proxyBridge.Login(id2, "tal.galmor@weka.io", "123AaC!@#").Value;
                    return proxyBridge.OpenNewStore(loggedid, "Jumbo");
                })
             };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsTrue(proxyBridge.GetStore(clientTasks[0].Result.Value).Value==null && proxyBridge.GetStore(clientTasks[1].Result.Value).Value != null||
                proxyBridge.GetStore(clientTasks[0].Result.Value).Value != null && proxyBridge.GetStore(clientTasks[1].Result.Value).Value == null);
        }


        #endregion

        #region Writing a review on items the user purchased 3.3
        [TestMethod]
        public void MemberWriteReviewOnItemPurchased_HappyTest()
        {
            //Arrange
            Mock_Orders mock_Orders = new Mock_Orders();
            mock_Orders.AddOrderToUser(memberid, new Order(new List<ItemForOrder> { new ItemForOrder(proxyBridge.GetItemByID(storeid1, itemid1).Value,storeid1) }));
            proxyBridge.SetTSOrders(mock_Orders);

            //Act
            Guid tempid = Guid.Empty;
            Task<Response> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                Guid loggedid = proxyBridge.Login(tempid, "gil@gmail.com", "asASD876!@").Value;
                return proxyBridge.WriteItemReview(memberid, storeid1, itemid1, "very good item!");
            });

            task.Wait();

            //Assert
            Assert.IsFalse(task.Result.ErrorOccured); //no error accured 
            Assert.IsTrue(proxyBridge.GetItemReviews(storeid1, itemid1).Value[memberid].Count==1);
        }

        [TestMethod]
        public void MemberWriteReviewOnItemHeDidNotPurchased_BadTest()
        {
            //Arrange
            Mock_Orders mock_Orders = new Mock_Orders();
            proxyBridge.SetTSOrders(mock_Orders);

            //Act
            Guid tempid = Guid.Empty;
            Task<Response> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                Guid loggedid = proxyBridge.Login(tempid, "gil@gmail.com", "asASD876!@").Value;
                return proxyBridge.WriteItemReview(memberid, storeid1, itemid1, "very good item!");
            });

            task.Wait();

            //Assert
            Assert.IsTrue(task.Result.ErrorOccured); // error accured 
            Assert.IsFalse(proxyBridge.GetItemReviews(storeid1, itemid1).Value.ContainsKey(memberid));
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void MultipleMembersWriteAreviewOnSameItem_HappyTest()
        {
            //Arrange
            Mock_Orders mock_Orders = new Mock_Orders();
            mock_Orders.AddOrderToUser(memberid, new Order(new List<ItemForOrder> { new ItemForOrder(proxyBridge.GetItemByID(storeid1, itemid1).Value, storeid1) }));
            mock_Orders.AddOrderToUser(systemManagerid, new Order(new List<ItemForOrder> { new ItemForOrder(proxyBridge.GetItemByID(storeid1, itemid1).Value, storeid1) }));
            proxyBridge.SetTSOrders(mock_Orders);


            Guid id1 = Guid.Empty;
            Guid id2 = Guid.Empty;
            Guid id3 = Guid.Empty;
            Guid id4 = Guid.Empty;
            Task<Response>[] clientTasks = new Task<Response>[] {
                Task.Run(() => {
                    id1=proxyBridge.Enter().Value;
                    Guid loggedid = proxyBridge.Login(id1, "RotemSela@gmail.com", "AS87654askj").Value;
                    return proxyBridge.WriteItemReview(systemManagerid, storeid1, itemid1, "very good item!");
                }),
                Task.Run(() => {
                    id2 = proxyBridge.Enter().Value;
                    Guid loggedid = proxyBridge.Login(id2, "gil@gmail.com", "asASD876!@").Value;
                    return proxyBridge.WriteItemReview(memberid, storeid1, itemid1, "very bad item!");
                })
             };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsFalse(clientTasks[0].Result.ErrorOccured);//no error occurred
            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetItemReviews(storeid1, itemid1).Value.Count==2);            
        }
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