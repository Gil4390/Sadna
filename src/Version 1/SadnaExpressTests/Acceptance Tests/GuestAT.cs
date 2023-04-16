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
    public class GuestAT : TradingSystemAT
    {
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
        }

        #region Guest Getting information about stores in the market and the products in the stores 2.1
        #endregion

        #region Guest search products by general search or filters 2.2
        [TestMethod]
        public void GuestSearchProductsByCategoryHome_HappyTest()
        {
            Guid id = new Guid();
            Task<ResponseT<List<Item>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.GetItemsByCategory(id,"Home");
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(task.Result.Value.Count==1);
        }

        [TestMethod]
        public void GuestSearchProductsByCategoryClothes_HappyTest()
        {
            Guid id = new Guid();
            Task<ResponseT<List<Item>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.GetItemsByCategory(id, "clothes");
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(task.Result.Value.Count == 3);
        }

        [TestMethod]
        public void GuestSearchProductsByCategoryClothesMixMaxPrice_HappyTest()
        {
            Guid id = new Guid();
            Task<ResponseT<List<Item>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.GetItemsByCategory(id, "clothes",90);
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(task.Result.Value.Count == 2);
        }

        [TestMethod]
        public void GuestSearchProductsByCategoryThatDoesntExist_BadTest()
        {
            Guid id = new Guid();
            Task<ResponseT<List<Item>>> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.GetItemsByCategory(id, "shay");
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(task.Result.Value.Count == 0);
        }

        [TestMethod]
        public void GuestSearchProductsByNameTowel_HappyTest()
        {
            Guid id = new Guid();
            Task<ResponseT<List<Item>>> task = Task.Run(() => {
                return GetItemsByName("Towel");
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(task.Result.Value.Count == 1);
        }

        private ResponseT<List<Item>> GetItemsByName(string itemName, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1)
        {
            Guid id = proxyBridge.Enter().Value;
            return proxyBridge.GetItemsByName(id, itemName, minPrice, maxPrice, ratingItem, category, ratingStore);
        }
        private ResponseT<List<Item>> GetItemsByCategory(string category, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, int ratingStore = -1)
        {
            Guid id = proxyBridge.Enter().Value;
            return proxyBridge.GetItemsByCategory(id, category, minPrice, maxPrice, ratingItem, ratingStore);
        }

        private ResponseT<List<Item>> GetItemsByKeysWord(string keyWords, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1)
        {
            Guid id = proxyBridge.Enter().Value;
            return proxyBridge.GetItemsByKeysWord(id, keyWords, minPrice, maxPrice, ratingItem, category, ratingStore);
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void MultipleClientsSearchForItems1_HappyTest()
        {
            Task<ResponseT<List<Item>>>[] clientTasks = new Task<ResponseT<List<Item>>>[] {
                Task.Run(() => GetItemsByName("Towel")),
                Task.Run(() => GetItemsByCategory("clothes")),
                Task.Run(() =>  GetItemsByCategory("clothes",90))
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
            Task<ResponseT<List<Item>>>[] clientTasks = new Task<ResponseT<List<Item>>>[] {
                Task.Run(() => GetItemsByName("Towel")),
                Task.Run(() => GetItemsByCategory("Home")),
                Task.Run(() =>  GetItemsByCategory("clothes",90)),
                Task.Run(() =>  GetItemsByKeysWord("to"))
             };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsFalse(clientTasks[0].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[0].Result.Value.Count == 1);

            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[1].Result.Value.Count == 1);

            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[2].Result.Value.Count == 2);

            Assert.IsFalse(clientTasks[3].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[3].Result.Value.Count == 2);
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.Baskets.Count==1);
           
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.Baskets.Count == 2);

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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.Baskets.Count == 1);
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.Baskets.Count == 0);
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.Baskets.Count == 0);
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id1).Value.Baskets.Count == 1);

            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id2).Value.Baskets.Count == 1);

            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id3).Value.Baskets.Count == 1);

            Assert.IsFalse(clientTasks[3].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id4).Value.Baskets.Count == 1);
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id1).ErrorOccured);

            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id2).Value.Baskets.Count == 1);

            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id3).Value.Baskets.Count == 1);

            Assert.IsFalse(clientTasks[3].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id4).Value.Baskets.Count == 1);
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id1).ErrorOccured);

            Assert.IsTrue(clientTasks[1].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id2).Value.Baskets.Count == 0);

            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id3).Value.Baskets.Count == 1);

            Assert.IsFalse(clientTasks[3].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id4).Value.Baskets.Count == 1);
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.Baskets.Count == 0);
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.Baskets.Count == 1);
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.GetItemQuantityInCart(storeid1,itemid1)==3);
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.Baskets.Count == 1);
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.Baskets.Count == 0);
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.Baskets.Count == 1);
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.GetItemQuantityInCart(storeid1, itemid1) == 1);
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.Baskets.Count == 0);
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.Baskets.Count == 0);
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.Baskets.Count == 1);
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.GetItemQuantityInCart(storeid1, itemid1) == 1);
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.Baskets.Count == 1);
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id).Value.GetItemQuantityInCart(storeid1, itemid1) == 1);
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void MultipleClientsEditShoppingCart_HappyTest()
        {
            Guid id1 = Guid.Empty;
            Guid id2 = Guid.Empty;
            Guid id3 = Guid.Empty;
            Guid id4 = Guid.Empty;
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id1).Value.Baskets.Count == 0);

            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id2).Value.Baskets.Count == 1);

            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id3).Value.Baskets.Count == 0);
        }
        #endregion

        #region Guest making a purchase of the shopping cart 2.5
        #endregion

        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
        }
    }
}