using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using SadnaExpress.ServiceLayer.Obj;
using SadnaExpress.ServiceLayer.SModels;

namespace SadnaExpressTests.Acceptance_Tests
{
    [TestClass]
    public class GuestAT : TradingSystemAT
    {
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
        }

        #region Guest Getting information about stores in the market and the products in the stores 2.1
                [TestMethod]
        public void StoreInfoUpdated1_HappyTest()
        {
            int count = 0;
            Task<ResponseT<List<Store>>> task = Task.Run(() =>
            {
                count = proxyBridge.GetAllStoreInfo().Value.Count;
                Store newStore = new Store("Pull&Bear");
                 stores.TryAdd(newStore.StoreID,newStore);
                 return proxyBridge.GetAllStoreInfo();
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //no error occurred
            Assert.IsTrue(task.Result.Value.Count == count + 1 );
        }
        [TestMethod]
        public void StoreInfoUpdated2_HappyTest()
        {
            int count = 0;
            Task<ResponseT<List<Store>>> task = Task.Run(() =>
            {
                count = proxyBridge.GetAllStoreInfo().Value.Count;
                Store newStore = new Store("Pull&Bear");
                stores.TryAdd(newStore.StoreID,newStore);
                stores.TryRemove(newStore.StoreID,out newStore);

                return proxyBridge.GetAllStoreInfo();
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //no error occurred
            Assert.IsTrue(task.Result.Value.Count == count);
        }
        
        [TestMethod]
        public void MultipleClientsGetInfo_HappyTest()
        {
            Store newStore = new Store("Pull&Bear");
            stores.TryAdd(newStore.StoreID,newStore);

            int count = proxyBridge.GetAllStoreInfo().Value.Count;

            Task<ResponseT<List<Store>>>[] clientTasks = new Task<ResponseT<List<Store>>>[] {
                Task.Run(() => {
                    return proxyBridge.GetAllStoreInfo();
                }),
                Task.Run(() => {
                    return proxyBridge.GetAllStoreInfo();
                }),
            };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsFalse(clientTasks[0].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetAllStoreInfo().Value.Count == count);

            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);//error occurred
            Assert.IsTrue(proxyBridge.GetAllStoreInfo().Value.Count == count);
            
        }
        
        
        #endregion

        #region Guest search products by general search or filters 2.2
        [TestMethod]
        public void GuestSearchProductsByCategoryHome_HappyTest()
        {
            Guid id = Guid.NewGuid();
            Task<ResponseT<List<SItem>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.GetItemsForClient(id,"",category:"Home");
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(task.Result.Value.Count==1);
        }

        [TestMethod]
        public void GuestSearchProductsByCategoryClothes_HappyTest()
        {
            Guid id = new Guid();
            Task<ResponseT<List<SItem>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.GetItemsForClient(id, "", category:"clothes");
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(task.Result.Value.Count == 3);
        }

        [TestMethod]
        public void GuestSearchProductsByCategoryClothesMixMaxPrice_HappyTest()
        {
            Guid id = new Guid();
            Task<ResponseT<List<SItem>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.GetItemsForClient(id, "",90,category:"clothes");
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(task.Result.Value.Count == 2);
        }

        [TestMethod]
        public void GuestSearchProductsByCategoryThatDoesntExist_BadTest()
        {
            Guid id = new Guid();
            Task<ResponseT<List<SItem>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.GetItemsForClient(id, "shay");
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(task.Result.Value.Count == 0);
        }

        [TestMethod]
        public void GuestSearchProductsByNameTowel_HappyTest()
        {
            Guid id = new Guid();
            Task<ResponseT<List<SItem>>> task = Task.Run(() => {
                return proxyBridge.GetItemsForClient(userid,"Towel");
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(task.Result.Value.Count == 1);
        }

        [TestMethod]
        public void GuestSearchProductsByNameTowelButStoreIsClosed_HappyTest()
        {
            //Arrange
            proxyBridge.GetStore(storeid2).Value.Active = false;
            Guid id = new Guid();
            Task<ResponseT<List<SItem>>> task = Task.Run(() => {
                return proxyBridge.GetItemsForClient(userid,"Towel");
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(task.Result.Value.Count == 0);
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void MultipleClientsSearchForItems1_HappyTest()
        {
            //Arrange
            Task<ResponseT<List<SItem>>>[] clientTasks = new Task<ResponseT<List<SItem>>>[] {
                Task.Run(() => proxyBridge.GetItemsForClient(userid ,"Towel")),
                Task.Run(() => proxyBridge.GetItemsForClient(userid ,"", category:"clothes")),
                Task.Run(() =>  proxyBridge.GetItemsForClient(userid ,"",90,category:"clothes"))
             };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsFalse(clientTasks[0].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[0].Result.Value.Count==1);

            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[1].Result.Value.Count == 3);

            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[2].Result.Value.Count == 2);
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void MultipleClientsSearchForItems2_HappyTest()
        {
            Task<ResponseT<List<SItem>>>[] clientTasks = new Task<ResponseT<List<SItem>>>[] {
                Task.Run(() => proxyBridge.GetItemsForClient(userid,"Towel")),
                Task.Run(() => proxyBridge.GetItemsForClient(userid,"",category:"Home")),
                Task.Run(() =>  proxyBridge.GetItemsForClient(userid,"",90,category:"clothes")),
                Task.Run(() =>  proxyBridge.GetItemsForClient(userid,"to"))
             };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsFalse(clientTasks[0].Result.ErrorOccured);
            Assert.AreEqual(1,clientTasks[0].Result.Value.Count);

            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[1].Result.Value.Count == 1);

            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[2].Result.Value.Count == 2);

            Assert.IsFalse(clientTasks[3].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[3].Result.Value.Count == 2);
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void MultipleClientsSearchForItemsWhenStoreBecomeClose_HappyTest()
        {
            //Arrange
            proxyBridge.GetMember(storeOwnerid).Value.LoggedIn = true;

            Task<ResponseT<List<SItem>>>[] clientTasks = new Task<ResponseT<List<SItem>>>[] {
                Task.Run(() => proxyBridge.GetItemsForClient(userid,"Towel")),
                Task.Run(() => proxyBridge.GetItemsForClient(userid,"",category:"clothes")),
                Task.Run(() =>  proxyBridge.GetItemsForClient(userid,"rt"))
             };

            Task<Response> task = Task.Run(() => {
                return proxyBridge.CloseStore(storeOwnerid, storeid1);
            });

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);
            task.Wait();

            Assert.IsFalse(clientTasks[0].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[0].Result.Value.Count == 1);

            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[1].Result.Value.Count == 1|| clientTasks[1].Result.Value.Count == 3);

            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[2].Result.Value.Count == 0 || clientTasks[2].Result.Value.Count == 1);

            Assert.IsFalse(task.Result.ErrorOccured);
        }
        #endregion

        #region Guest saving item in the shopping cart for some store 2.3
        [TestMethod]
        public void GuestSave1ItemInShoppingCart_HappyTest()
        {
            Guid id = new Guid();
            Task<Response> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.AddItemToCart(id, storeid1,itemid1,1);
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.Baskets.Count==1);
           
        }

        [TestMethod]
        public void GuestSaveItemsInShoppingCartFromDiffStore_HappyTest()
        {
            Guid id = new Guid();
            Task<Response> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid1, itemid1, 1);
                return proxyBridge.AddItemToCart(id, storeid2, itemid2, 1);
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.Baskets.Count == 2);

        }

        [TestMethod]
        public void GuestSave2ItemsInShoppingCartFromSameStore_HappyTest()
        {
            Guid id = Guid.Empty;
            Task<Response> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid1, itemid1, 1);
                return proxyBridge.AddItemToCart(id, storeid1, itemid11, 1);
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.Baskets.Count == 1);
        }

        [TestMethod]
        public void GuestSaveItemsInShoppingCartItemDoesNotExist_HappyTest()
        {
            Guid id = Guid.Empty;
            Task<Response> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.AddItemToCart(id, storeid1, Guid.NewGuid(), 1);
            });
            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured);//error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.Baskets.Count == 0);
        }

        [TestMethod]
        public void GuestSaveItemsInShoppingCartStoreDoesNotExist_BadTest()
        {
            Guid id = Guid.Empty;
            Task<Response> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.AddItemToCart(id, Guid.NewGuid(), itemid1, 1);
            });
            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured);//error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.Baskets.Count == 0);
        }

        private Response AddItemToCart(Guid id,Guid storeid, Guid itemid, int itemAmount)
        {
            return proxyBridge.AddItemToCart(id, storeid, itemid, itemAmount);
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void MultipleClientsAddSameItemToShoppingCart_HappyTest()
        {
            Guid id1 = Guid.Empty;
            Guid id2 = Guid.Empty;
            Guid id3 = Guid.Empty;
            Guid id4 = Guid.Empty;
            Task<Response>[] clientTasks = new Task<Response>[] {
                Task.Run(() => {id1=proxyBridge.Enter().Value; return AddItemToCart(id1,storeid1,itemid1,1); }),
                Task.Run(() => {id2=proxyBridge.Enter().Value; return AddItemToCart(id2,storeid1,itemid1,1); }),
                Task.Run(() => {id3=proxyBridge.Enter().Value; return AddItemToCart(id3,storeid1,itemid1,1); }),
                Task.Run(() => {id4=proxyBridge.Enter().Value; return AddItemToCart(id4,storeid1,itemid1,1); }),
             };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsFalse(clientTasks[0].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id1).Value.Baskets.Count == 1);

            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id2).Value.Baskets.Count == 1);

            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id3).Value.Baskets.Count == 1);

            Assert.IsFalse(clientTasks[3].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id4).Value.Baskets.Count == 1);
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void MultipleClientsAddSameItemToShoppingCartOneNotEnter_HappyTest()
        {
            Guid id1 = Guid.Empty;
            Guid id2 = Guid.Empty;
            Guid id3 = Guid.Empty;
            Guid id4 = Guid.Empty;
            Task<Response>[] clientTasks = new Task<Response>[] {
                Task.Run(() => { return AddItemToCart(id1,storeid1,itemid1,1); }),
                Task.Run(() => {id2=proxyBridge.Enter().Value; return AddItemToCart(id2,storeid1,itemid1,1); }),
                Task.Run(() => {id3=proxyBridge.Enter().Value; return AddItemToCart(id3,storeid1,itemid1,1); }),
                Task.Run(() => {id4=proxyBridge.Enter().Value; return AddItemToCart(id4,storeid1,itemid1,1); }),
             };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsTrue(clientTasks[0].Result.ErrorOccured);//error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id1).ErrorOccured);

            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id2).Value.Baskets.Count == 1);

            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id3).Value.Baskets.Count == 1);

            Assert.IsFalse(clientTasks[3].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id4).Value.Baskets.Count == 1);
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void MultipleClientsAddSameItemToShoppingCartWhileIsRemovedByManager_HappyTest()
        {
            //Arrange
            proxyBridge.GetMember(storeOwnerid).Value.LoggedIn = true;

            Guid id1 = Guid.Empty;
            Guid id2 = Guid.Empty;
            
            Task<Response>[] clientTasks = new Task<Response>[] {
                Task.Run(() => {id1=proxyBridge.Enter().Value; return AddItemToCart(id1,storeid1,itemid1,2); }),
                Task.Run(() => {id2=proxyBridge.Enter().Value; return AddItemToCart(id2,storeid1,itemid1,5); }),
             };

            Task<Response> task = Task.Run(() => {
                return proxyBridge.RemoveItemFromStore(storeOwnerid, storeid1, itemid1);
            });

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);
            task.Wait();

            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void MultipleClientsAddSameItemToCartOneNotEnterAndOneChooseItemNotInStock_HappyTest()
        {
            Guid id1 = Guid.Empty;
            Guid id2 = Guid.Empty;
            Guid id3 = Guid.Empty;
            Guid id4 = Guid.Empty;
            Task<Response>[] clientTasks = new Task<Response>[] {
                Task.Run(() => { return AddItemToCart(id1,storeid1,itemid1,1); }),
                Task.Run(() => {id2=proxyBridge.Enter().Value; return AddItemToCart(id2,storeid2,itemNoStock,1); }),
                Task.Run(() => {id3=proxyBridge.Enter().Value; return AddItemToCart(id3,storeid1,itemid1,1); }),
                Task.Run(() => {id4=proxyBridge.Enter().Value; return AddItemToCart(id4,storeid1,itemid1,1); }),
             };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsTrue(clientTasks[0].Result.ErrorOccured);//error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id1).ErrorOccured);

            Assert.IsTrue(clientTasks[1].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id2).Value.Baskets.Count == 0);

            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id3).Value.Baskets.Count == 1);

            Assert.IsFalse(clientTasks[3].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id4).Value.Baskets.Count == 1);
        }
        #endregion

        #region Guest checking the content of the shopping cart and making changes 2.4
        [TestMethod]
        public void GuestEditShoppingCartAddAndRemove_HappyTest()
        {
            Guid id = new Guid();
            Task<Response> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid1, itemid1, 1);
                return proxyBridge.RemoveItemFromCart(id, storeid1, itemid1);
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.Baskets.Count == 0);
        }

        [TestMethod]
        public void GuestEditShoppingCartAddAndEditQuantity_HappyTest()
        {
            Guid id = new Guid();
            Task<Response> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid1, itemid1, 1);
                return proxyBridge.EditItemFromCart(id, storeid1, itemid1, 3);
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.Baskets.Count == 1);
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.GetItemQuantityInCart(storeid1,itemid1)==3);
        }

        [TestMethod]
        public void GuestEditShoppingCartAddAndEditQuantityNoChange_HappyTest()
        {
            Guid id = new Guid();
            Task<Response> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid1, itemid1, 1);
                return proxyBridge.EditItemFromCart(id, storeid1, itemid1, 1);
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.Baskets.Count == 1);
        }

        [TestMethod]
        public void GuestEditShoppingCartItemThatDoesNotExist_BadTest()
        {
            Guid id = Guid.Empty;
            Task<Response> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.EditItemFromCart(id, storeid1, itemid1, 1);
            });
            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.Baskets.Count == 0);
        }

        [TestMethod]
        public void GuestEditShoppingCartWithBadStoreId_BadTest()
        {
            Guid id = Guid.Empty;
            Task<Response> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid1, itemid1, 1);
                return proxyBridge.EditItemFromCart(id, storeid2, itemid1, 4);
            });
            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.Baskets.Count == 1);
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.GetItemQuantityInCart(storeid1, itemid1) == 1);
        }

        [TestMethod]
        public void GuestRemoveItemFromShoppingCartThatDoesNotExist_BadTest()
        {
            Guid id = Guid.Empty;
            Task<Response> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.RemoveItemFromCart(id, storeid1, itemid1);
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.Baskets.Count == 0);
        }

        [TestMethod]
        public void GuestRemoveItemFromShoppingCart_HappyTest()
        {
            Guid id = Guid.Empty;
            Task<Response> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid1, itemid1, 1);
                proxyBridge.AddItemToCart(id, storeid1, itemid1, 4);
                return proxyBridge.RemoveItemFromCart(id, storeid1, itemid1);
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.Baskets.Count == 0);
        }

        [TestMethod]
        public void GuestRemoveItemFromShoppingCartBadStoreId_BadTest()
        {
            Guid id = Guid.Empty;
            Task<Response> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid1, itemid1, 1);
                return proxyBridge.EditItemFromCart(id, Guid.NewGuid(), itemid1, 4);
            });
            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.Baskets.Count == 1);
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.GetItemQuantityInCart(storeid1, itemid1) == 1);
        }

        [TestMethod]
        public void GuestRemoveItemFromShoppingCartBadItemId_BadTest()
        {
            Guid id = Guid.Empty;
            Task<Response> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid1, itemid1, 1);
                return proxyBridge.EditItemFromCart(id, storeid1, Guid.NewGuid(), 4);
            });
            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.Baskets.Count == 1);
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id).Value.GetItemQuantityInCart(storeid1, itemid1) == 1);
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void MultipleClientsEditShoppingCart_HappyTest()
        {
            Guid id1 = Guid.Empty;
            Guid id2 = Guid.Empty;
            Guid id3 = Guid.Empty;
            Task<Response>[] clientTasks = new Task<Response>[] {
                Task.Run(() => {
                    id1=proxyBridge.Enter().Value; 
                    AddItemToCart(id1,storeid1,itemid1,1);
                    AddItemToCart(id1,storeid1,itemid1,1);
                    return proxyBridge.RemoveItemFromCart(id1,storeid1,itemid1);
                }),
                Task.Run(() => {
                    id2=proxyBridge.Enter().Value; 
                    AddItemToCart(id2,storeid1,itemid1,1);
                    AddItemToCart(id2,storeid2,itemid2,1);
                    AddItemToCart(id2,storeid1,itemid1,1);
                    return proxyBridge.EditItemFromCart(id2,storeid1,itemid1,0);
                }),
                Task.Run(() => {
                    id3=proxyBridge.Enter().Value;
                    AddItemToCart(id3,storeid1,itemid1,1);
                    proxyBridge.EditItemFromCart(id3,storeid1,itemid2,0);
                    return proxyBridge.EditItemFromCart(id3,storeid1,itemid1,0);
                })
             };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsFalse(clientTasks[0].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id1).Value.Baskets.Count == 0);

            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id2).Value.Baskets.Count == 1);

            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id3).Value.Baskets.Count == 0);
        }
        #endregion

        #region Guest making a purchase of the shopping cart 2.5
        [TestMethod]
        public void Guest1PurchaseShoppingCart_HappyTest()
        {
            Guid id = new Guid();
            Task<ResponseT<List<ItemForOrder>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid1, itemid1, 1);
                SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                return proxyBridge.PurchaseCart(id, transactionDetails, transactionDetailsSupply);
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); // no error occurred
            Assert.AreEqual(0,proxyBridge.GetDetailsOnCart(id).Value.Baskets.Count); // the shopping basket get empty
            Assert.AreEqual(39, proxyBridge.GetStore(storeid1).Value.GetItemByQuantity(itemid1)); //the quantity updated
        }
        [TestMethod]
        public void InvalidPaymentInformation_BadTest()
        {
            Guid id = new Guid();
            Task<ResponseT<List<ItemForOrder>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid1, itemid1, 1);
                proxyBridge.SetPaymentService(new Mocks.Mock_Bad_PaymentService());
                SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                return proxyBridge.PurchaseCart(id, transactionDetails, transactionDetailsSupply);
            });
            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured);//error occurred
            Assert.AreEqual(1,proxyBridge.GetDetailsOnCart(id).Value.Baskets.Count);// the shopping basket same
            Assert.AreEqual(40, proxyBridge.GetStore(storeid1).Value.GetItemByQuantity(itemid1));//the quantity same
        }
        [TestMethod]
        public void PurchaseItemFromStoreNotActive_BadTest()
        {
            Guid id = new Guid();
            Task<ResponseT<List<ItemForOrder>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                proxyBridge.AddItemToCart(id, storeid1, itemid1, 1);
                proxyBridge.GetStore(storeid1).Value.Active = false;
                SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                return proxyBridge.PurchaseCart(id, transactionDetails, transactionDetailsSupply);
            });
            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured);//error occurred
            Assert.AreEqual(1,proxyBridge.GetDetailsOnCart(id).Value.Baskets.Count);// the shopping basket same
            Assert.AreEqual(40, proxyBridge.GetStore(storeid1).Value.GetItemByQuantity(itemid1));//the quantity same
        }
        
        [TestMethod]
        [TestCategory("Concurrency")]
        public void Guest2PurchaseShoppingCartWhile_OwnerEditHappyTest()
        {
            //Arrange
            Guid id1 = new Guid();
            Guid id2 = new Guid();
            Guid id3 = proxyBridge.Enter().Value;
            proxyBridge.Login(id3, "AsiAzar@gmail.com", "A#!a12345678");
            // Create guest 1 cart
            id1=proxyBridge.Enter().Value; 
            AddItemToCart(id1,storeid1,itemid1,1);
            AddItemToCart(id1,storeid1,itemid22,1);
            // create guest 2 cart
            id2=proxyBridge.Enter().Value; 
            AddItemToCart(id2,storeid1,itemid1,1);
            AddItemToCart(id2,storeid1,itemid22,1);

            storeOwnerid = proxyBridge.Login(storeOwnerid, "AsiAzar@gmail.com", "A#!a12345678").Value;
            
            // Act
                Task<ResponseT<List<ItemForOrder>>> task1 = Task.Run(() => {
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(id1,transactionDetails, transactionDetailsSupply);
                });
                Task<ResponseT<List<ItemForOrder>>> task2 = Task.Run(() => {
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(id2,transactionDetails, transactionDetailsSupply);
                });
                Task<Response> task3 = Task.Run(() => {
                    return proxyBridge.EditItemQuantity(storeOwnerid,storeid1,itemid22,-1);
                });
            
            // Wait for all clients to complete
            Task.WaitAll();
            //Assert
            // 2 situations: 1. The 2 guests succeed to purchase the cart and the owner fail to edit the quantity
            //               2. Just one of the guest succeed to purchase the cart, the other fail the owner succed to edit the quantity                  
            bool situation1 = !task1.Result.ErrorOccured && !task2.Result.ErrorOccured && task3.Result.ErrorOccured;
            bool situation2 = ((task1.Result.ErrorOccured || task2.Result.ErrorOccured) &&
                               (!(task1.Result.ErrorOccured && task2.Result.ErrorOccured))
                               & !task3.Result.ErrorOccured);
            Assert.IsTrue(situation1 || situation2);
            Assert.AreEqual(0, proxyBridge.GetStore(storeid1).Value.GetItemByQuantity(itemid22));
            if (situation1)
                Assert.AreEqual(4, proxyBridge.GetStorePurchases(storeOwnerid, storeid1).Value.Count);
            else
                Assert.AreEqual(2, proxyBridge.GetStorePurchases(storeOwnerid, storeid1).Value.Count);
        }
        [TestMethod]
        [TestCategory("Concurrency")]
        public void Guest2PurchaseWhileThereOnlyOneItemHappyTest()
        {
            Guid id1 = new Guid();
            Guid id2 = new Guid();
            // Create guest 1 cart
            id1  =proxyBridge.Enter().Value;
            proxyBridge.AddItemToCart(id1, storeid1, itemid1, 2); //because policy
            proxyBridge.AddItemToCart(id1,storeid1,itemid23,1);
             
            // Create guest 2 cart
            id2 = proxyBridge.Enter().Value;
            proxyBridge.AddItemToCart(id2, storeid1, itemid1, 3); //because policy
            proxyBridge.AddItemToCart(id2,storeid1,itemid23,1);
            
            Task<ResponseT<List<ItemForOrder>>>[] clientTasks = new Task<ResponseT<List<ItemForOrder>>>[] {
                Task.Run(() => {
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(id1,transactionDetails, transactionDetailsSupply);
                }),
                Task.Run(() => {
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(id2,transactionDetails, transactionDetailsSupply);
                })
            };
            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            // one of the guest doesn't have quantity
            Assert.IsTrue((clientTasks[0].Result.ErrorOccured || clientTasks[1].Result.ErrorOccured)
                          &&(!(clientTasks[0].Result.ErrorOccured && clientTasks[1].Result.ErrorOccured)));
            Assert.AreEqual(0, proxyBridge.GetStore(storeid1).Value.GetItemByQuantity(itemid23));
        }
        [TestMethod]
        [TestCategory("Concurrency")]
        public void OwnerRemoveItemWhileUserPurchaseHappyTest()
        {
            Guid id1 = proxyBridge.Enter().Value;
            proxyBridge.Login(id1, "AsiAzar@gmail.com", "A#!a12345678");
            // Create guest 1 cart
            Guid id2=proxyBridge.Enter().Value;

            Task<Response> task1 = Task.Run(() =>
            {
                return proxyBridge.RemoveItemFromStore(storeOwnerid, storeid1, itemid22);
            });

            Task<ResponseT<List<ItemForOrder>>> task2 = Task.Run(() =>
            {
                AddItemToCart(id2, storeid1, itemid22, 1);
                AddItemToCart(id2, storeid1, itemid1, 1);
                SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                return proxyBridge.PurchaseCart(id2, transactionDetails, transactionDetailsSupply);
            });
            // Wait for all clients to complete
            task1.Wait();
            task2.Wait();
            Task.WaitAll();
            // 2 situations: 1. The guest succeed to purchase the cart and after it the item was removed.
            //               2. The item removed and then the guest try and get an error.                     
            bool situation1 = !task1.Result.ErrorOccured && !task2.Result.ErrorOccured;
            bool situation2 = !task1.Result.ErrorOccured && task2.Result.ErrorOccured;
            Assert.IsTrue(situation1 || situation2);
            Assert.ThrowsException<Exception>(()=>proxyBridge.GetStore(storeid1).Value.GetItemById(itemid22));
            if (situation1)
                Assert.AreEqual(39, proxyBridge.GetStore(storeid1).Value.GetItemByQuantity(itemid1));
            else
                Assert.AreEqual(40, proxyBridge.GetStore(storeid1).Value.GetItemByQuantity(itemid1));
        }
        
        #endregion

        #region Place bid to item
  
        [TestMethod]
        public void PlaceBid_Success()
        {
            //Arrange
            Guid tempid = proxyBridge.Enter().Value;
            proxyBridge.Login(tempid, "AsiAzar@gmail.com", "A#!a12345678");
            proxyBridge.AddItemToCart(userid, storeid1, itemid1, 2);
            //Act
            Response t = proxyBridge.PlaceBid(userid, itemid1, 50);
            //Assert
            Assert.IsFalse(t.ErrorOccured);
            Assert.AreEqual(1, proxyBridge.GetNotifications(storeOwnerid).Value.Count);
            Assert.AreEqual(itemid1,proxyBridge.GetBidsInStore(storeOwnerid, storeid1).Value[0].ItemID);
            Assert.AreEqual(50,proxyBridge.GetBidsInStore(storeOwnerid, storeid1).Value[0].OfferPrice);
        }
        [TestMethod]
        public void PlaceBidAndApproved_Success()
        {
            //Arrange
            Guid tempid = proxyBridge.Enter().Value;
            proxyBridge.Login(tempid, "AsiAzar@gmail.com", "A#!a12345678");
            proxyBridge.AddItemToCart(userid, storeid1, itemid1, 1);
            ResponseT<SBid> bid = proxyBridge.PlaceBid(userid, itemid1, 50);
            //Act
            Response t = proxyBridge.ReactToBid(storeOwnerid, itemid1, bid.Value.BidID, "approved");
            //Assert
            Assert.IsFalse(t.ErrorOccured);
            Assert.AreEqual(50,proxyBridge.GetItemsForClient(userid, "Tshirt").Value[0].OfferPrice);
            Assert.AreEqual(50,proxyBridge.GetCartItems(userid).Value[0].OfferPrice);
            Assert.AreEqual(1, proxyBridge.GetNotifications(storeOwnerid).Value.Count);
        }
        
        [TestMethod]
        public void PlaceBidAndThenPlaceBetter_Success()
        {
            //Arrange
            Guid tempid = proxyBridge.Enter().Value;
            proxyBridge.Login(tempid, "AsiAzar@gmail.com", "A#!a12345678");
            proxyBridge.PlaceBid(userid, itemid1, 60);
            Assert.AreEqual(60,proxyBridge.GetBidsInStore(storeOwnerid, storeid1).Value[0].OfferPrice);
            //Act
            Response t = proxyBridge.PlaceBid(userid, itemid1, 50);
            //Assert
            Assert.IsFalse(t.ErrorOccured);
            Assert.AreEqual(itemid1,proxyBridge.GetBidsInStore(storeOwnerid, storeid1).Value[0].ItemID);
            Assert.AreEqual(50,proxyBridge.GetBidsInStore(storeOwnerid, storeid1).Value[0].OfferPrice);
            Assert.AreEqual(2, proxyBridge.GetNotifications(storeOwnerid).Value.Count);
        }
        [TestMethod]
        public void PlaceBidAndThenPlaceWorse_Success()
        {
            //Arrange
            Guid tempid = proxyBridge.Enter().Value;
            proxyBridge.Login(tempid, "AsiAzar@gmail.com", "A#!a12345678");
            proxyBridge.PlaceBid(userid, itemid1, 60);
            Assert.AreEqual(60,proxyBridge.GetBidsInStore(storeOwnerid, storeid1).Value[0].OfferPrice);
            //Act
            Response t = proxyBridge.PlaceBid(userid, itemid1, 70);
            //Assert
            Assert.IsTrue(t.ErrorOccured);
            Assert.AreEqual(itemid1,proxyBridge.GetBidsInStore(storeOwnerid, storeid1).Value[0].ItemID);
            Assert.AreEqual(60,proxyBridge.GetBidsInStore(storeOwnerid, storeid1).Value[0].OfferPrice);
            Assert.AreEqual(1, proxyBridge.GetNotifications(storeOwnerid).Value.Count);
        }
        

        #endregion

        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
        }
    }
}

