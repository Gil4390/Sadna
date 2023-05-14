using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using SadnaExpress.ServiceLayer.SModels;

namespace SadnaExpressTests.Acceptance_Tests
{
    [TestClass]
    public class GuestMemberStoreOwnerManagerAT : TradingSystemAT
    {
        Guid store4Founder = Guid.Empty;
        Guid store5Founder = Guid.Empty;
        Guid storeID4 = Guid.Empty;
        Guid storeID5 = Guid.Empty;
        Guid store5Owner = Guid.Empty;
        Guid store5Manager = Guid.Empty;
        Guid loggedInUserID1 = Guid.Empty;
        Guid loggedInUserID2 = Guid.Empty;

        Guid itemID1 = Guid.Empty;
        Guid itemID2 = Guid.Empty;


        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            store4Founder = proxyBridge.Enter().Value;
            proxyBridge.Register(store4Founder, "storeFounder4Mail@gmail.com", "radwan", "ganem", "A#!a12345678");
            store4Founder = proxyBridge.Login(store4Founder, "storeFounder4Mail@gmail.com", "A#!a12345678").Value;
            storeID4 = proxyBridge.OpenNewStore(store4Founder, "Store 4").Value;

            store5Founder = proxyBridge.Enter().Value;
            proxyBridge.Register(store5Founder, "storeFounderMail@gmail.com", "tal", "galmor", "A#!a12345678");
            store5Founder = proxyBridge.Login(store5Founder, "storeFounderMail@gmail.com", "A#!a12345678").Value;
            storeID5 = proxyBridge.OpenNewStore(store5Founder, "Candy store").Value;

            store5Owner = proxyBridge.Enter().Value;
            proxyBridge.Register(store5Owner, "storeOwnerMail2@gmail.com", "dina", "agapov", "A#!a12345678");
            store5Owner = proxyBridge.Login(store5Owner, "storeOwnerMail2@gmail.com", "A#!a12345678").Value;

            proxyBridge.AppointStoreOwner(store5Founder, storeID5, "storeOwnerMail2@gmail.com");
            
            proxyBridge.AppointStoreOwner(store5Founder, storeid1, "storeOwnerMail2@gmail.com");

            
            store5Manager = proxyBridge.Enter().Value;
            proxyBridge.Register(store5Manager, "storeManagerMail2@gmail.com", "bar", "lerrer", "A#!a12345678");
            store5Manager = proxyBridge.Login(store5Manager, "storeManagerMail2@gmail.com", "A#!a12345678").Value;

            proxyBridge.AppointStoreManager(store5Founder, storeID5, "storeManagerMail2@gmail.com");


            loggedInUserID1 = proxyBridge.Enter().Value;
            proxyBridge.Register(loggedInUserID1, "logmail1@gmail.com", "usi1", "last1", "A#!a12345678");
            loggedInUserID1 = proxyBridge.Login(loggedInUserID1, "logmail1@gmail.com", "A#!a12345678").Value;
            loggedInUserID2 = proxyBridge.Enter().Value;
            proxyBridge.Register(loggedInUserID2, "logmail2@gmail.com", "usi2", "last2", "A#!a12345678");
            loggedInUserID2 = proxyBridge.Login(loggedInUserID2, "logmail2@gmail.com", "A#!a12345678").Value;


            itemID1 = proxyBridge.AddItemToStore(store5Founder, storeID5, "doritos", "food", 6.0, 10).Value;
            itemID2 = proxyBridge.AddItemToStore(store5Founder, storeID5, "bisli", "food", 4.0, 20).Value;


        }
        
         #region Notification

        #region
        
        public List<Notification> unreadMessages(List<Notification> notifications)
        {
            List<Notification> notificationsUnRead = new List<Notification>();
            foreach (Notification notification in notifications)
            {
                if(!notification.Read)
                    notificationsUnRead.Add(notification);
            }

            return notificationsUnRead;
        }
        [TestMethod]
        public void BuyProductGetNotificationOffline()
        {
            int pre = unreadMessages(proxyBridge.GetNotifications(store5Owner).Value).Count;
            //Arrange
            proxyBridge.Logout(store5Owner);
            //Act
            Guid enterId = proxyBridge.Enter().Value;
            proxyBridge.Login(enterId, "gil@gmail.com", "asASD876!@");
            Item item = proxyBridge.GetItemsInStore(store5Owner, storeID5).Value[0];
            proxyBridge.AddItemToCart(memberId, storeID5, item.ItemID, 1);
            proxyBridge.PurchaseCart(memberId, "5044222", "Rabbi Akiva 5");
            //Assert
            Assert.AreEqual(unreadMessages(proxyBridge.GetNotifications(store5Owner).Value).Count,pre + 1);
        }
        [TestMethod]
        public void BuyProductGetNotificationOnline()
        {
            int pre = unreadMessages(proxyBridge.GetNotifications(store5Owner).Value).Count;
            //Act
            Guid enterId = proxyBridge.Enter().Value;
            proxyBridge.Login(enterId, "gil@gmail.com", "asASD876!@");
            Item item = proxyBridge.GetItemsInStore(store5Owner, storeID5).Value[0];
            proxyBridge.AddItemToCart(memberId, storeID5, item.ItemID, 1);
            proxyBridge.PurchaseCart(memberId, "5044222", "Rabbi Akiva 5");
            //Assert
            Assert.AreEqual(unreadMessages(proxyBridge.GetNotifications(store5Owner).Value).Count,pre + 1);
        }
        
        [TestMethod]
        public void CloseStoreGetNotificationOffline()
        {
            int pre = unreadMessages(proxyBridge.GetNotifications(store5Owner).Value).Count;
            proxyBridge.Logout(store5Owner);
            //Act
            proxyBridge.CloseStore(store5Founder, storeID5);
            //Assert
            Assert.AreEqual(unreadMessages(proxyBridge.GetNotifications(store5Owner).Value).Count,pre + 1);
        }
        [TestMethod]
        public void CloseStoreGetNotificationOnline()
        {
            int pre = unreadMessages(proxyBridge.GetNotifications(store5Owner).Value).Count;
            //Arrange
            proxyBridge.GetMember(store5Owner ).Value.LoggedIn = true;
            //Act
            proxyBridge.CloseStore(store5Founder, storeID5);
            //Assert
            Assert.AreEqual(unreadMessages(proxyBridge.GetNotifications(store5Owner).Value).Count,pre + 1);
        }
        [TestMethod]
        public void CloseStoreNotGettingTheSameMessageTwice()
        {
            int pre = unreadMessages(proxyBridge.GetMember(store5Owner).Value.AwaitingNotification).Count;
            //Arrange
            proxyBridge.GetMember(store5Owner ).Value.LoggedIn = false;
            proxyBridge.GetMember(store5Founder ).Value.LoggedIn = true;
            //Act
            proxyBridge.RemovePermission(store5Founder, storeID5, "storeOwnerMail2@gmail.com","owner permissions");
            //Assert
            // USER OFFLINE - gets the message
            Assert.AreEqual(pre + 1,unreadMessages(proxyBridge.GetMember(store5Owner).Value.AwaitingNotification).Count);
            // USER ONLINE - marks the message as read
            proxyBridge.GetMember(store5Owner ).Value.LoggedIn = true;
            unreadMessages(proxyBridge.GetMember(store5Owner).Value.AwaitingNotification)[0].Read = true;
            Assert.AreEqual(0, unreadMessages(proxyBridge.GetMember(store5Owner).Value.AwaitingNotification).Count);
            // USER OFFLINE 
            proxyBridge.GetMember(store5Owner ).Value.LoggedIn = false;
            // USER ONLINE - enters the system again and the message is still marked as read
            proxyBridge.GetMember(store5Owner ).Value.LoggedIn = true;
            Assert.AreEqual(0, unreadMessages(proxyBridge.GetMember(store5Owner).Value.AwaitingNotification).Count);
        }

        
        [TestMethod]
        public void removeStoreOwnerGetNotificationOnline()
        {
            int pre = unreadMessages(proxyBridge.GetMember(store5Owner).Value.AwaitingNotification).Count;
            //Arrange
            proxyBridge.GetMember(store5Founder ).Value.LoggedIn = true;
            //Act
            proxyBridge.RemovePermission(store5Founder, storeID5, "storeOwnerMail2@gmail.com", "owner permissions");
            //Assert
            Assert.AreEqual(pre + 1,unreadMessages(proxyBridge.GetMember(store5Owner).Value.AwaitingNotification).Count);
        }
    
        [TestMethod]
        public void removeStoreOwnerGetNotificationOffline()
        {
            int pre = unreadMessages(proxyBridge.GetMember(store5Owner).Value.AwaitingNotification).Count;
            //Arrange
            proxyBridge.GetMember(store5Owner ).Value.LoggedIn = false;
            proxyBridge.GetMember(store5Founder ).Value.LoggedIn = true;
            //Act
            proxyBridge.RemovePermission(store5Founder, storeID5, "storeOwnerMail2@gmail.com","owner permissions");
            //Assert
            Assert.AreEqual(pre + 1,unreadMessages(proxyBridge.GetMember(store5Owner).Value.AwaitingNotification).Count);
        }
     
        
        [TestMethod]
        public void removeStoreOwnerNotificationUnReadUpdated()
        {
            int pre = unreadMessages(proxyBridge.GetMember(store5Owner).Value.AwaitingNotification).Count;
            //Arrange
            proxyBridge.GetMember(store5Owner ).Value.LoggedIn = false;
            proxyBridge.GetMember(store5Founder ).Value.LoggedIn = true;
            //Act
            proxyBridge.RemovePermission(store5Founder, storeID5, "storeOwnerMail2@gmail.com", "owner permissions");
            //Assert
            Assert.AreEqual(pre + 1,unreadMessages(proxyBridge.GetMember(store5Owner).Value.AwaitingNotification).Count);
            proxyBridge.GetMember(store5Founder ).Value.LoggedIn = true;
            unreadMessages(proxyBridge.GetMember(store5Owner).Value.AwaitingNotification)[0].Read =true;
            Assert.AreEqual(0, unreadMessages(proxyBridge.GetMember(store5Owner).Value.AwaitingNotification).Count);
            
        }
        
        [TestMethod]
        public void appointStoreOwnerGettingNotificationWhenMemberBuyProduct()
        {
            int pre = unreadMessages(proxyBridge.GetMember(store5Manager).Value.AwaitingNotification).Count;
            proxyBridge.AppointStoreOwner(store5Founder, storeID5, "storeManagerMail2@gmail.com");
            Guid enterId = proxyBridge.Enter().Value;
            proxyBridge.Login(enterId, "gil@gmail.com", "asASD876!@");
            Item item = proxyBridge.GetItemsInStore(store5Owner, storeID5).Value[0];
            proxyBridge.AddItemToCart(memberId, storeID5, item.ItemID, 1);
            proxyBridge.PurchaseCart(memberId, "5044222", "Rabbi Akiva 5");
            
            Assert.AreEqual(pre + 1, unreadMessages(proxyBridge.GetMember(store5Manager).Value.AwaitingNotification).Count);
            
        }
        
        
        
        
        
        
        
        
        // not in this version
        public void OpenStoreNotification()
        {   
            //Arrange
            proxyBridge.GetMember(store5Founder).Value.LoggedIn = false;
            proxyBridge.CloseStore(store5Founder, storeID5);
            //Act
            proxyBridge.ReopenStore(store5Founder, storeID5);
            //Assert
            Assert.AreEqual(proxyBridge.GetMember(store5Founder).Value.AwaitingNotification.Count,2);
        }
        #endregion

        #endregion
        
        #region Product Managment 4.1

        #region Add new item
        [TestMethod]
        public void AddingNewItemGood()
        {
            Task<ResponseT<Guid>> task = Task.Run(() => {
                return proxyBridge.AddItemToStore(store5Owner, storeID5, "chips", "food", 5.0, 2);
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //error not occured 

            int count = proxyBridge.GetItemsForClient(store5Owner, "chips").Value.Count;
            Assert.AreEqual(1, count); //item was added
        }
        [TestMethod]
        public void AddingNewItemUserNotLoggedIn_Bad()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;

                return proxyBridge.AddItemToStore(tempid, storeID5, "bamba", "food", 5.0, 2);
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error occured 

            int count = proxyBridge.GetItemsForClient(tempid, "bamba").Value.Count;
            Assert.AreEqual(0, count); //item was not added
        }
        [TestMethod]
        public void AddingNewItemStoreDoesNotExist_Bad()
        {
            Task<ResponseT<Guid>> task = Task.Run(() => {
                return proxyBridge.AddItemToStore(storeOwnerid, Guid.NewGuid(), "bamba", "food", 5.0, 2);
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error occured  
        }
        [TestMethod]
        public void AddingNewItemUserIsNotAStoreOwner_Bad()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                tempid = proxyBridge.Login(tempid, "gil@gmail.com", "asASD876!@").Value;

                return proxyBridge.AddItemToStore(tempid, storeID5, "bamba", "food", 5.0, 2);
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error occured  

            int count = proxyBridge.GetItemsForClient(tempid, "bamba").Value.Count;
            Assert.AreEqual(0, count); //item was not added
        }
        [TestMethod]
        public void AddingNewItemUserIsNotTheOwnerOfThisStore_Bad()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                tempid = proxyBridge.Login(tempid, "AsiAzar@gmail.com", "Aa12345678").Value;

                return proxyBridge.AddItemToStore(tempid, storeid2, "bamba", "food", 5.0, 2);
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error occured  

            int count = proxyBridge.GetItemsForClient(tempid, "bamba").Value.Count;
            Assert.AreEqual(0, count); //item was not added
        }

        [TestMethod]
        public void AddingNewItem2OwnersAddingItemWithSameNameAtOnce_Concurrent_Bad()
        {
            //2 owners attempting to add an item with same name one should fail and one should succeed
            Task<ResponseT<Guid>> task1 = Task.Run(() => {
                return proxyBridge.AddItemToStore(store5Owner, storeID5, "chips", "food", 5.0, 2);
            });

            
            Task<ResponseT<Guid>> task2 = Task.Run(() => {
                return proxyBridge.AddItemToStore(store5Founder, storeID5, "chips", "food", 5.0, 2);
            });

            task1.Wait();
            task2.Wait();
            bool error1occured = task1.Result.ErrorOccured;
            bool error2occured = task2.Result.ErrorOccured;
            Assert.IsTrue(error1occured || error2occured); //at lest one should fail
            Assert.IsTrue(!(error1occured && error2occured)); //at least one should succeed

            int count = proxyBridge.GetItemsForClient(userid, "chips").Value.Count;
            Assert.AreEqual(1, count); //item was added
        }
        #endregion

        #region Remove item
        [TestMethod]
        public void RemoveItem_Good()
        {
            Task<Response> task = Task.Run(() => {
                return proxyBridge.RemoveItemFromStore(store5Owner, storeID5, itemID2);
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //error not occured  

            int count = proxyBridge.GetItemsForClient(store5Founder, "bisli").Value.Count;
            Assert.AreEqual(0, count); //item was removed
        }
        [TestMethod]
        public void RemoveItemNotExist_Bad()
        {
            Task<Response> task = Task.Run(() => {
                return proxyBridge.RemoveItemFromStore(store5Owner, storeID5, Guid.NewGuid());
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error occured 

            int count = proxyBridge.GetItemsForClient(store5Founder, "bisli").Value.Count;
            Assert.AreEqual(1, count); //item was removed
        }
        [TestMethod]
        public void RemoveItemConcurrent_Bad()
        {
            Task<Response> task1 = Task.Run(() => {
                return proxyBridge.RemoveItemFromStore(store5Owner, storeID5, itemID2);
            });


            Task<Response> task2 = Task.Run(() => {
                return proxyBridge.RemoveItemFromStore(store5Owner, storeID5, itemID2);
            });

            task1.Wait();
            task2.Wait();
            bool error1occured = task1.Result.ErrorOccured;
            bool error2occured = task2.Result.ErrorOccured;
            Assert.IsTrue(error1occured || error2occured); //at least one should fail
            Assert.IsTrue(!(error1occured && error2occured)); //at least one should succeed

            int count = proxyBridge.GetItemsForClient(userid, "bisli").Value.Count;
            Assert.AreEqual(0, count); //item was removed
        }
        [TestMethod]
        public void RemoveItemConcurrent_Good()
        {
            Task<Response> task1 = Task.Run(() => {
                return proxyBridge.RemoveItemFromStore(store5Owner, storeID5, itemID1);
            });


            Task<Response> task2 = Task.Run(() => {
                return proxyBridge.RemoveItemFromStore(store5Owner, storeID5, itemID2);
            });

            task1.Wait();
            task2.Wait();
            bool error1occured = task1.Result.ErrorOccured;
            bool error2occured = task2.Result.ErrorOccured;
            Assert.IsTrue(!error1occured && !error2occured); //need success

            int count = proxyBridge.GetItemsForClient(userid, "bisli").Value.Count;
            Assert.AreEqual(0, count); //item was removed
            int count2 = proxyBridge.GetItemsForClient(userid, "doritos").Value.Count;
            Assert.AreEqual(0, count2); //item was removed
        }
        #endregion

        #region Edit item
        [TestMethod]
        public void EditItemName_Good()
        {
            Task<Response> task = Task.Run(() => {
                return proxyBridge.EditItemName(store5Owner, storeID5, itemID2, "bisli bbq");
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //error not occured  

            int count = proxyBridge.GetItemsForClient(store5Founder, "bisli bbq").Value.Count;
            Assert.AreEqual(1, count);
        }
        [TestMethod]
        public void EditItemCategory_Good()
        {
            Task<Response> task = Task.Run(() => {
                return proxyBridge.EditItemCategory(store5Owner, storeID5, itemID2, "snacks");
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //error not occured  

            int count = proxyBridge.GetItemsForClient(store5Founder, "", category:"snacks").Value.Count;
            Assert.AreEqual(1, count);
        }
        [TestMethod]
        public void EditItem_Bad()
        {
            Task<Response> task = Task.Run(() => {
                return proxyBridge.EditItemName(store5Owner, storeID5, itemID2, "");
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error occured  

            int count = proxyBridge.GetItemsForClient(store5Founder, "bisli").Value.Count;
            Assert.AreEqual(1, count);
        }
        [TestMethod]
        public void EditItemConcurrent_Good()
        {
            Task<Response> task1 = Task.Run(() => {
                return proxyBridge.EditItemName(store5Owner, storeID5, itemID2, "aaa");
            });
            Task<Response> task2 = Task.Run(() => {
                return proxyBridge.EditItemName(store5Owner, storeID5, itemID2, "bbb");
            });

            task1.Wait();
            task2.Wait();
            Assert.IsFalse(task1.Result.ErrorOccured); //error not occured
            Assert.IsFalse(task2.Result.ErrorOccured); //error not occured  
                                                       

            int count1 = proxyBridge.GetItemsForClient(store5Founder, "aaa").Value.Count;
            int count2 = proxyBridge.GetItemsForClient(store5Founder, "bbb").Value.Count;

            Assert.AreEqual(1, count1+count2);

        }
        #endregion

        #endregion

        #region Policys 4.2

        [TestMethod]
        public void GetAllPoliciesFromLoadData()
        {
            //Act
            List<SPolicy> sPolicies = proxyBridge.GetAllPolicy(storeOwnerid, storeid1).Value;
            //Assert
            Assert.AreEqual(3, sPolicies.Count);
        }
        #endregion

        #region Appointing a new store owner 4.4

        [TestMethod]
        public void AppointingNewStoreOwner_Good()
        {
            Task<Response> task = Task.Run(() => {
                return proxyBridge.AppointStoreOwner(store5Founder, storeID5, "logmail1@gmail.com");
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //error not occured 

            proxyBridge.AddItemToStore(loggedInUserID1, storeID5, "testItem", "test", 50.0, 45);

            int count = proxyBridge.GetItemsForClient(store5Owner, "testItem").Value.Count;
            Assert.AreEqual(1, count); //item was added because the user has permissions
        }

        [TestMethod]
        public void AppointingNewStoreOwner_Bad()
        {
            Task<Response> task = Task.Run(() => {
                return proxyBridge.AppointStoreOwner(store5Founder, storeID5, "storeOwnerMail2@gmail.com");
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error should occur 
        }

        [TestMethod]
        public void AppointingNewStoreOwnerNoPermission_Bad()
        {
            Task<Response> task = Task.Run(() => {
                return proxyBridge.AppointStoreOwner(loggedInUserID1, storeID5, "logmail2@gmail.com");
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error should occur 
        }

        [TestMethod]
        public void AppointingNewStoreOwnerCircular_Bad()
        {
            //setup: founder => owner
            //here: founder => logi1 => owner (error)
            Task<Response> task = Task.Run(() => {
                proxyBridge.AppointStoreOwner(store5Founder, storeID5, "logmail1@gmail.com");
                return proxyBridge.AppointStoreOwner(loggedInUserID1, storeID5, "storeFounderMail@gmail.com");
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error should occur 
        }

        [TestMethod]
        public void AppointingNewStoreOwnerBy2StoreOwners_Concurrent_Bad()
        {
            Task<Response> task1 = Task.Run(() => {
                return proxyBridge.AppointStoreOwner(store5Founder, storeID5, "logmail1@gmail.com");
            });


            Task<Response> task2 = Task.Run(() => {
                return proxyBridge.AppointStoreOwner(store5Owner, storeID5, "logmail1@gmail.com");
            });

            task1.Wait();
            task2.Wait();
            bool error1occured = task1.Result.ErrorOccured;
            bool error2occured = task2.Result.ErrorOccured;
            Assert.IsTrue(error1occured || error2occured); //at lest one should fail
            Assert.IsTrue(!(error1occured && error2occured)); //at least one should succeed

            proxyBridge.AddItemToStore(loggedInUserID1, storeID5, "testItem", "test", 50.0, 45);
            int count = proxyBridge.GetItemsForClient(store5Owner, "testItem").Value.Count;
            Assert.AreEqual(1, count); //item was added because the user has permissions
        }

        #endregion
        
        #region Remove store owner by his direct manager 4.5
        [TestMethod]
        public void RemoveStoreOwner_Good()
        {
            Task<Response> task = Task.Run(() => {
                return proxyBridge.RemovePermission(store5Founder, storeID5, "storeOwnerMail2@gmail.com", "owner permissions");
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //error not occured 
            Assert.AreNotEqual(typeof(PromotedMember), proxyBridge.GetMember(store5Owner).Value.GetType());
        }
        [TestMethod]
        [TestCategory("Concurrency")]
        public void StoreOwnerRemoveHisAppointWhileOtherRemoveHim_Good()
        {
            // create appoint
            proxyBridge.AppointStoreOwner(store5Owner, storeID5, "gil@gmail.com");
            Task<Response>[] clientTasks = new Task<Response>[] {
                Task.Run(() => {
                    return proxyBridge.RemovePermission(store5Owner,storeID5, "gil@gmail.com", "owner permissions");
                }),
                Task.Run(() =>
                {
                    return proxyBridge.RemovePermission(store5Founder, storeID5, "storeOwnerMail2@gmail.com", "owner permissions");
                })
            };
            // Wait for all clients to complete
            Task.WaitAll(clientTasks);
            // 2 situations: 1. The owner remove the owner and then removed
            //               2. The owner removed so he can't remove a owner 

            bool situation1 = !clientTasks[0].Result.ErrorOccured && !clientTasks[1].Result.ErrorOccured;
            bool situation2 = clientTasks[0].Result.ErrorOccured && !clientTasks[1].Result.ErrorOccured;
            Assert.IsTrue(situation1 || situation2);
            // check them both not employee in store
            Assert.AreNotEqual(typeof(PromotedMember), proxyBridge.GetMember(store5Owner).Value.GetType());
            Assert.AreNotEqual(typeof(PromotedMember), proxyBridge.GetMember(memberId).Value.GetType());
        }
        [TestMethod]
        [TestCategory("Concurrency")]
        public void StoreOwnerAddAppointWhileFounderRemoveHim_Good()
        {
            Assert.AreEqual(typeof(PromotedMember), proxyBridge.GetMember(store5Owner).Value.GetType());

            Task<Response>[] clientTasks = new Task<Response>[] {
                Task.Run(() =>
                {
                    return proxyBridge.RemovePermission(store5Founder, storeID5, "storeOwnerMail2@gmail.com","owner permissions");
                }),
                Task.Run(() =>
                {
                    Thread.Sleep(1);
                    return proxyBridge.AppointStoreOwner(store5Owner, storeID5, "gil@gmail.com");
                })
            };
            // Wait for all clients to complete
            Task.WaitAll(clientTasks);
            // 2 situations: 1. The owner add the owner and then removed
            //               2. The owner removed so he can't add a owner  
            bool situation1 = !clientTasks[0].Result.ErrorOccured && !clientTasks[1].Result.ErrorOccured;
            bool situation2 = !clientTasks[0].Result.ErrorOccured && clientTasks[1].Result.ErrorOccured;
            Assert.IsTrue(situation1 || situation2);
            // check them both not employee in store
            if (situation1)
                Assert.IsFalse(proxyBridge.GetMember(store5Founder).Value.GetEmployeeInfoInStore(storeID5).Contains((PromotedMember)proxyBridge.GetMember(memberId).Value));
            Assert.AreNotEqual(typeof(PromotedMember), proxyBridge.GetMember(store5Owner).Value.GetType());
        }
        
        [TestMethod]
        [TestCategory("Concurrency")]
        public void StoreOwnerAddItemWhileFounderRemoveHim_Good()
        {
            Assert.AreEqual(typeof(PromotedMember), proxyBridge.GetMember(store5Owner).Value.GetType());

            Task<ResponseT<Guid>> task1 = Task.Run(() => {
                return proxyBridge.AddItemToStore(store5Owner, storeID5, "bamba","food", 5, 3);
            });
            
            Task<Response> task2 = Task.Run(() => {
                return proxyBridge.RemovePermission(store5Founder, storeID5, "storeOwnerMail2@gmail.com", "owner permissions");
            });

            task1.Wait();
            task2.Wait();

            // Wait for all clients to complete
            Task.WaitAll();
            // 2 situations: 1. The owner add the item and then removed
            //               2. The owner removed so he can't add the item  

            bool situation1 = !task1.Result.ErrorOccured && !task2.Result.ErrorOccured;
            bool situation2 = task1.Result.ErrorOccured && !task2.Result.ErrorOccured;
            Assert.IsTrue(situation1 || situation2);
            if (situation1)
               Assert.IsNotNull(proxyBridge.GetStore(storeID5).Value.GetItemById(task1.Result.Value));
            else
                Assert.ThrowsException<Exception>(()=>proxyBridge.GetStore(storeID5).Value.GetItemById(task1.Result.Value));
            // in both of the situation he need to removed
            Assert.AreNotEqual(typeof(PromotedMember), proxyBridge.GetMember(store5Owner).Value.GetType());
        }
        #endregion
        
        #region Appointing a new store manager 4.6

        [TestMethod]
        public void AppointingNewStoreManager_Good()
        {
            Task<Response> task = Task.Run(() => {
                return proxyBridge.AppointStoreManager(store5Owner, storeID5, "logmail1@gmail.com");
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //error not occured 
        }

        [TestMethod]
        public void AppointingNewStoreManager_Bad()
        {
            Task<Response> task = Task.Run(() => {
                return proxyBridge.AppointStoreManager(store5Founder, storeID5, "storeManagerMail2@gmail.com");
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error should occur 
        }

        [TestMethod]
        public void AppointingNewStoreManagerNoPermission_Bad()
        {

            Task<Response> task = Task.Run(() => {
                return proxyBridge.AppointStoreManager(store5Manager, storeID5, "logmail1@gmail.com");
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error should occur 
        }
 

        [TestMethod]
        public void AppointingNewStoreManagerBy2StoreOwners_Concurrent_Bad()
        {
            Task<Response> task1 = Task.Run(() => {
                return proxyBridge.AppointStoreManager(store5Founder, storeID5, "logmail1@gmail.com");
            });


            Task<Response> task2 = Task.Run(() => {
                return proxyBridge.AppointStoreManager(store5Owner, storeID5, "logmail1@gmail.com");
            });

            task1.Wait();
            task2.Wait();
            bool error1occured = task1.Result.ErrorOccured;
            bool error2occured = task2.Result.ErrorOccured;
            Assert.IsTrue(error1occured || error2occured); //at lest one should fail
            Assert.IsTrue(!(error1occured && error2occured)); //at least one should succeed
        }

        #endregion

        #region changing a store manager permission 4.7

        #region owner permissions

        [TestMethod]
        public void AddingOwnerPermissions_Good()
        {
            
            ResponseT<Guid> response1 = proxyBridge.AddItemToStore(store5Manager, storeID5, "testItem", "testCat", 50.0, 5);
            Assert.IsTrue(response1.ErrorOccured); //no permissions yet

            Response response2 = proxyBridge.AddStoreManagerPermissions(store5Founder, storeID5, "storeManagerMail2@gmail.com", "owner permissions");
            Assert.IsFalse(response2.ErrorOccured); //error not occured 

            ResponseT<Guid> response3 = proxyBridge.AddItemToStore(store5Manager, storeID5, "testItem", "testCat", 50.0, 5);
            Assert.IsFalse(response3.ErrorOccured); //now he has permissions
            
        }

        [TestMethod]
        public void AddingOwnerPermissionsTwice_Bad()
        {
            Response response1 = proxyBridge.AddStoreManagerPermissions(store5Founder, storeID5, "storeManagerMail2@gmail.com", "owner permissions");
            Assert.IsFalse(response1.ErrorOccured); //added permission 

            Response response2 = proxyBridge.AddStoreManagerPermissions(store5Owner, storeID5, "storeManagerMail2@gmail.com", "owner permissions");
            Assert.IsTrue(response2.ErrorOccured); //added again, should be error
        }

        [TestMethod]
        public void AddingOwnerPermissionsApointerIsNotOwner_Bad()
        {
            Response response1 = proxyBridge.AddStoreManagerPermissions(loggedInUserID1, storeID5, "storeManagerMail2@gmail.com", "owner permissions");
            Assert.IsTrue(response1.ErrorOccured); 
        }

        [TestMethod]
        public void AddingOwnerPermissionsApointeeIsNotManager_Bad()
        {
            Response response1 = proxyBridge.AddStoreManagerPermissions(store5Founder, storeID5, "logmail1@gmail.com", "owner permissions");
            Assert.IsTrue(response1.ErrorOccured);
        }

        [TestMethod]
        public void AddingOwnerPermissionsApointerOfApointer_Bad()
        {
            
            proxyBridge.AppointStoreOwner(store5Owner, storeID5, "logmail1@gmail.com"); //loggedInUser1 is now also owner, and storeOwner apointed him
            proxyBridge.AppointStoreManager(loggedInUserID1, storeID5, "logmail2@gmail.com"); //loggedInUser2 is now manager, and loggedInUser1 appointed him

            // store5Owner => loggedInUser1 => loggedInUser2


            Response response1 = proxyBridge.AddStoreManagerPermissions(store5Owner, storeID5, "logmail2@gmail.com", "owner permissions");
            Assert.IsTrue(response1.ErrorOccured); //should fail because store5Owner is not the appointer of loggedInUser2
        }

        [TestMethod]
        public void AddingOwnerPermissionsTwiceConcurrent_Bad()
        {
            Task<Response> task1 = Task.Run(() => {
                return proxyBridge.AddStoreManagerPermissions(store5Founder, storeID5, "storeManagerMail2@gmail.com", "owner permissions");
            });


            Task<Response> task2 = Task.Run(() => {
                return proxyBridge.AddStoreManagerPermissions(store5Owner, storeID5, "storeManagerMail2@gmail.com", "owner permissions");
            });

            task1.Wait();
            task2.Wait();
            bool error1occured = task1.Result.ErrorOccured;
            bool error2occured = task2.Result.ErrorOccured;
            Assert.IsTrue(error1occured || error2occured); //at lest one should fail
            Assert.IsTrue(!(error1occured && error2occured)); //at least one should succeed
        }



        #endregion

        #region add new owner permissions

        [TestMethod]
        public void AddingNewOwnerPermissions_Good()
        {

            ResponseT<Guid> response1 = proxyBridge.AddItemToStore(store5Manager, storeID5, "testItem", "testCat", 50.0, 5);
            Assert.IsTrue(response1.ErrorOccured); //no permissions yet

            Response response2 = proxyBridge.AddStoreManagerPermissions(store5Founder, storeID5, "storeManagerMail2@gmail.com", "owner permissions");
            Assert.IsFalse(response2.ErrorOccured); //error not occured 

            ResponseT<Guid> response3 = proxyBridge.AddItemToStore(store5Manager, storeID5, "testItem", "testCat", 50.0, 5);
            Assert.IsFalse(response3.ErrorOccured); //now he has permissions

        }

        #endregion

        #region product management permissions

        [TestMethod]
        public void AddingProductManagementPermissions_Good()
        {

            Response response1 = proxyBridge.AppointStoreOwner(store5Manager, storeID5, "logmail2@gmail.com");
            Assert.IsTrue(response1.ErrorOccured); //no permissions yet

            Response response2 = proxyBridge.AddStoreManagerPermissions(store5Founder, storeID5, "storeManagerMail2@gmail.com", "add new owner");
            Assert.IsFalse(response2.ErrorOccured); //error not occured 

            Response response3 = proxyBridge.AppointStoreOwner(store5Manager, storeID5, "logmail2@gmail.com");
            Assert.IsFalse(response3.ErrorOccured); //now he has permissions

        }

        #endregion


        #endregion
        
        #region closing a store 4.9

        [TestMethod]
        public void ClosingAStore_Good()
        {
            var response = proxyBridge.GetAllStoreInfo();
            int numOfActiveStoresBefore = response.Value.FindAll(s => s.Active).Count;

            Task<Response> task = Task.Run(() => {
                return proxyBridge.CloseStore(store5Founder, storeID5); 
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //error not occured 

            var response2 = proxyBridge.GetAllStoreInfo();
            int numOfActiveStoresAfter = response2.Value.FindAll(s => s.Active).Count;

            Assert.AreEqual(numOfActiveStoresAfter, numOfActiveStoresBefore-1);
        }
        [TestMethod]
        public void ClosingAnAlreadyClosedStore_Bad()
        {
            Task<Response> task = Task.Run(() => {
                return proxyBridge.CloseStore(store5Founder, storeID5);
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //error not occured 

            var response = proxyBridge.GetAllStoreInfo();
            int numOfActiveStoresAfterFirstClose = response.Value.FindAll(s => s.Active).Count;

            Task<Response> task2 = Task.Run(() => {
                return proxyBridge.CloseStore(store5Founder, storeID5);
            });

            task2.Wait();
            Assert.IsTrue(task2.Result.ErrorOccured); //error occured 

            var response2 = proxyBridge.GetAllStoreInfo();
            int numOfActiveStoresAfterSecondClose = response2.Value.FindAll(s => s.Active).Count;

            Assert.AreEqual(numOfActiveStoresAfterFirstClose, numOfActiveStoresAfterSecondClose);
        }

        [TestMethod]
        public void ClosingAStoreAndTryingToMakeChanges_Good()
        {
            Task<Response> task = Task.Run(() => {
                return proxyBridge.CloseStore(store5Founder, storeID5);
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //error not occured 

            Task<Response> task2 = Task.Run(() => {
                return proxyBridge.AppointStoreOwner(store5Founder, storeID5, "logmail2@gmail.com");
            });

            task2.Wait();
            Assert.IsFalse(task2.Result.ErrorOccured); //error not occured 
        }

        [TestMethod]
        public void ClosingStoreTwiceConcurrent_Bad()
        {
            var response = proxyBridge.GetAllStoreInfo();
            int numOfActiveStoresBefore = response.Value.FindAll(s => s.Active).Count;

            Task<Response> task1 = Task.Run(() => {
                return proxyBridge.CloseStore(store5Founder, storeID5);
            });

            Task<Response> task2 = Task.Run(() => {
                return proxyBridge.CloseStore(store5Founder, storeID5);
            });

            task1.Wait();
            task2.Wait();
            bool error1occured = task1.Result.ErrorOccured;
            bool error2occured = task2.Result.ErrorOccured;
            Assert.IsTrue(error1occured || error2occured); //at lest one should fail
            Assert.IsTrue(!(error1occured && error2occured)); //at least one should succeed

            var response2 = proxyBridge.GetAllStoreInfo();
            int numOfActiveStoresAfter = response2.Value.FindAll(s => s.Active).Count;

            Assert.AreEqual(numOfActiveStoresAfter, numOfActiveStoresBefore - 1);
        }

        #endregion

        #region request store employees� information 4.11

        [TestMethod]
        public void RequestStoreEmployeeInfo_Good()
        {
            Task<ResponseT<List<SMemberForStore>>> task = Task.Run(() => {
                return proxyBridge.GetEmployeeInfoInStore(store5Owner, storeID5);
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //error not occured

            int employeeCountBefore = task.Result.Value.Count;

            Task<ResponseT<List<SMemberForStore>>> task2 = Task.Run(() => {
                proxyBridge.AppointStoreManager(store5Owner, storeID5, "logmail1@gmail.com"); //added new employee
                return proxyBridge.GetEmployeeInfoInStore(store5Owner, storeID5);
            });

            task2.Wait();
            Assert.IsFalse(task2.Result.ErrorOccured); //error not occured

            int employeeCountAfter = task2.Result.Value.Count;

            Assert.AreEqual(employeeCountBefore + 1, employeeCountAfter);
        }

        [TestMethod]
        public void RequestStoreEmployeeInfoOf10Employees_Good()
        {
            // create store founder
            Guid storeFounder = proxyBridge.Enter().Value;
            proxyBridge.Register(storeFounder, "storeFounderMail1@gmail.com", "radwan", "ganem", "A#!a12345678");
            storeFounder = proxyBridge.Login(storeFounder, "storeFounderMail1@gmail.com", "A#!a12345678").Value;
            Guid storeID = proxyBridge.OpenNewStore(storeFounder, "Store 1").Value;

            // create owner 1
            Response res1 = proxyBridge.AppointStoreOwner(storeFounder, storeID, "gil@gmail.com");

            //create appoint 1 to owner 1
            Guid enter = proxyBridge.Enter().Value;
            proxyBridge.Login(enter, "gil@gmail.com", "asASD876!@");
            proxyBridge.AppointStoreOwner(memberId, storeID, "sebatian@gmail.com");

            //create appoint 2 to owner 1
            Response res2 = proxyBridge.AppointStoreOwner(memberId, storeID, "amihai@gmail.com");

            //create appoint 1 to member1
            enter = proxyBridge.Enter().Value;
            proxyBridge.Login(enter, "sebatian@gmail.com", "asASD123!@");
            proxyBridge.AppointStoreOwner(memberId2, storeID, "bar@gmail.com");

            //create appoint 2 to member1
            Guid memberId5 = proxyBridge.Enter().Value;
            proxyBridge.Register(memberId5, "member5@gmail.com", "member", "member", "A#!a12345678");
            proxyBridge.AppointStoreOwner(memberId2, storeID, "member5@gmail.com");
            memberId5 = proxyBridge.Login(memberId5, "member5@gmail.com", "A#!a12345678").Value;

            //create appoint 1 to member2
            enter = proxyBridge.Enter().Value;
            proxyBridge.Login(enter, "amihai@gmail.com", "asASD753!@");
            Guid memberId6 = proxyBridge.Enter().Value;
            proxyBridge.Register(memberId6, "member6@gmail.com", "member", "member", "A#!a12345678");
            memberId6 = proxyBridge.Login(memberId6, "member6@gmail.com", "A#!a12345678").Value;
            proxyBridge.AppointStoreOwner(memberId3, storeID, "member6@gmail.com");

            //create appoint 2 to member2
            Guid memberId7 = proxyBridge.Enter().Value;
            proxyBridge.Register(memberId7, "member7@gmail.com", "member", "member", "A#!a12345678");
            proxyBridge.AppointStoreManager(memberId3, storeID, "member7@gmail.com");

            //create appoint 1 to member6
            enter = proxyBridge.Enter().Value;
            proxyBridge.Login(enter, "member6@gmail.com", "A#!a12345678");

            Guid memberId8 = proxyBridge.Enter().Value;
            proxyBridge.Register(memberId8, "member8@gmail.com", "member", "member", "A#!a12345678");
            proxyBridge.AppointStoreOwner(memberId6, storeID, "member8@gmail.com");

            //create appoint 1 to member 5
            enter = proxyBridge.Enter().Value;
            proxyBridge.Login(enter, "member5@gmail.com", "A#!a12345678");

            Guid memberId9 = proxyBridge.Enter().Value;
            proxyBridge.Register(memberId9, "member9@gmail.com", "member", "member", "A#!a12345678");
            proxyBridge.AppointStoreManager(memberId5, storeID, "member9@gmail.com");

            List<SMemberForStore> employees = proxyBridge.GetEmployeeInfoInStore(storeFounder, storeID).Value;
            List<string> employeesEmail = new List<string>();
            // convert employees to emails list in order
            foreach (SMemberForStore member in employees)
            {
                employeesEmail.Add(member.Email);
            }
            Assert.IsTrue(employeesEmail.Contains("storeFounderMail1@gmail.com"));
            Assert.IsTrue(employeesEmail.Contains("gil@gmail.com"));
            Assert.IsTrue(employeesEmail.Contains("amihai@gmail.com"));
            Assert.IsTrue(employeesEmail.Contains("sebatian@gmail.com"));
            Assert.IsTrue(employeesEmail.Contains("bar@gmail.com"));
            Assert.IsTrue(employeesEmail.Contains("member5@gmail.com"));
            Assert.IsTrue(employeesEmail.Contains("member5@gmail.com"));
            Assert.IsTrue(employeesEmail.Contains("member6@gmail.com"));
            Assert.IsTrue(employeesEmail.Contains("member7@gmail.com"));
            Assert.IsTrue(employeesEmail.Contains("member8@gmail.com"));
            Assert.IsTrue(employeesEmail.Contains("member9@gmail.com"));
        }

        [TestMethod]
        public void RequestStoreEmployeeInfoNoPermission_Good()
        {
            Task<ResponseT<List<SMemberForStore>>> task = Task.Run(() => {
                return proxyBridge.GetEmployeeInfoInStore(store5Manager, storeID5);
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error occured

            Task<ResponseT<List<SMemberForStore>>> task2 = Task.Run(() => {
                proxyBridge.AddStoreManagerPermissions(store5Founder, storeID5, "storeManagerMail2@gmail.com", "get employees info");
                return proxyBridge.GetEmployeeInfoInStore(store5Manager, storeID5);
            });

            task2.Wait();
            Assert.IsFalse(task2.Result.ErrorOccured); //error not occured
        }
        #endregion


        #region request store purchase history 4.13

        [TestMethod]
        public void RequestStorePurchaseInfo_Good()
        {
            Task<ResponseT<List<ItemForOrder>>> task = Task.Run(() => {
                return proxyBridge.GetStorePurchases(store5Owner, storeID5);
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //error not occured
        }

        [TestMethod]
        public void RequestStorePurchaseInfoNoPermission_Good()
        {
            Task<ResponseT<List<ItemForOrder>>> task = Task.Run(() => {
                return proxyBridge.GetStorePurchases(loggedInUserID1, storeID5);
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //no error occured
            Assert.AreEqual(0, task.Result.Value.Count);
        }
        #endregion


        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
        }
    }
}