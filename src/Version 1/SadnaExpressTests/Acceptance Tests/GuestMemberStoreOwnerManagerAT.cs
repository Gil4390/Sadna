using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;

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

            int count = proxyBridge.GetItemsByName(store5Owner, "chips").Value.Count;
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

            int count = proxyBridge.GetItemsByName(tempid, "bamba").Value.Count;
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

            int count = proxyBridge.GetItemsByName(tempid, "bamba").Value.Count;
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

            int count = proxyBridge.GetItemsByName(tempid, "bamba").Value.Count;
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

            int count = proxyBridge.GetItemsByName(userid, "chips").Value.Count;
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

            int count = proxyBridge.GetItemsByName(store5Founder, "bisli").Value.Count;
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

            int count = proxyBridge.GetItemsByName(store5Founder, "bisli").Value.Count;
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
            Assert.IsTrue(error1occured || error2occured); //at lest one should fail
            Assert.IsTrue(!(error1occured && error2occured)); //at least one should succeed

            int count = proxyBridge.GetItemsByName(userid, "bisli").Value.Count;
            Assert.AreEqual(0, count); //item was removed
        }

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
            Assert.IsTrue(error1occured || error2occured); //at lest one should fail
            Assert.IsTrue(!(error1occured && error2occured)); //at least one should succeed

            int count = proxyBridge.GetItemsByName(userid, "bisli").Value.Count;
            Assert.AreEqual(0, count); //item was removed
            int count2 = proxyBridge.GetItemsByName(userid, "doritos").Value.Count;
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

            int count = proxyBridge.GetItemsByName(store5Founder, "bisli bbq").Value.Count;
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

            int count = proxyBridge.GetItemsByCategory(store5Founder, "snacks").Value.Count;
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

            int count = proxyBridge.GetItemsByName(store5Founder, "bisli").Value.Count;
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
                                                       

            int count1 = proxyBridge.GetItemsByName(store5Founder, "aaa").Value.Count;
            int count2 = proxyBridge.GetItemsByName(store5Founder, "bbb").Value.Count;

            Assert.AreEqual(1, count1+count2);

        }
        #endregion

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

            int count = proxyBridge.GetItemsByName(store5Owner, "testItem").Value.Count;
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
            int count = proxyBridge.GetItemsByName(store5Owner, "testItem").Value.Count;
            Assert.AreEqual(1, count); //item was added because the user has permissions
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


        #region request store employees’ information 4.11

        [TestMethod]
        public void RequestStoreEmployeeInfo_Good()
        {
            Task<ResponseT<List<PromotedMember>>> task = Task.Run(() => {
                return proxyBridge.GetEmployeeInfoInStore(store5Owner, storeID5);
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //error not occured

            int employeeCountBefore = task.Result.Value.Count;

            Task<ResponseT<List<PromotedMember>>> task2 = Task.Run(() => {
                proxyBridge.AppointStoreManager(store5Owner, storeID5, "logmail1@gmail.com"); //added new employee
                return proxyBridge.GetEmployeeInfoInStore(store5Owner, storeID5);
            });

            task2.Wait();
            Assert.IsFalse(task2.Result.ErrorOccured); //error not occured

            int employeeCountAfter = task2.Result.Value.Count;

            Assert.AreEqual(employeeCountBefore + 1, employeeCountAfter);
        }

        [TestMethod]
        public void RequestStoreEmployeeInfoNoPermission_Good()
        {
            Task<ResponseT<List<PromotedMember>>> task = Task.Run(() => {
                return proxyBridge.GetEmployeeInfoInStore(store5Manager, storeID5);
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error occured

            Task<ResponseT<List<PromotedMember>>> task2 = Task.Run(() => {
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
            Task<ResponseT<List<Order>>> task = Task.Run(() => {
                return proxyBridge.GetStorePurchases(store5Owner, storeID5);
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //error not occured
        }

        [TestMethod]
        public void RequestStorePurchseInfoNoPermission_Good()
        {
            Task<ResponseT<List<Order>>> task = Task.Run(() => {
                return proxyBridge.GetStorePurchases(loggedInUserID1, storeID5);
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error occured
        }
        #endregion


        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
        }
    }
}