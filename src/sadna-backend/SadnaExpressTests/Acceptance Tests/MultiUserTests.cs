using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DataLayer;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using SadnaExpress.ServiceLayer.SModels;

namespace SadnaExpressTests.Acceptance_Tests
{
    [TestClass]
    public class MultiUserTests : TradingSystemAT
    {
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
        }

        #region Many users want to add items to cart and show the cart
        [TestMethod]
        [TestCategory("Concurrency")]
        public void ManyUsersAddingItems()
        {
            Guid id1 = Guid.Empty;
            Guid id2 = Guid.Empty;
            Guid id3 = Guid.Empty;
            Guid id4 = Guid.Empty;
            Guid id5 = Guid.Empty;
            Guid id6 = Guid.Empty;
            Guid id7 = Guid.Empty;
            Guid id8 = Guid.Empty;
            Guid id9 = Guid.Empty;
            Task<ResponseT<ShoppingCart>>[] clientTasks = new Task<ResponseT<ShoppingCart>>[] {
                Task.Run(() =>
                {
                    id1=proxyBridge.Enter().Value;
                    proxyBridge.Register(id1, "Ted@amazon.io","Ted", "Lasso", "123AaC!@#");
                    id1 = proxyBridge.Login(id1, "Ted@amazon.io", "123AaC!@#").Value;
                    Thread.Sleep(1000);
                    return proxyBridge.GetDetailsOnCart(id1);
                }),
                Task.Run(() =>
                {
                    id2=proxyBridge.Enter().Value;
                    proxyBridge.Register(id2, "Roy@amazon.io","Roy", "Kent", "123AaC!@#");
                    Thread.Sleep(500);
                    id2 = proxyBridge.Login(id2, "Roy@amazon.io", "123AaC!@#").Value;
                    Thread.Sleep(500);
                    return proxyBridge.GetDetailsOnCart(id2);
                }),
                Task.Run(() =>
                {
                    id3=proxyBridge.Enter().Value;
                    proxyBridge.Register(id3, "Tartt@amazon.io","Jamie", "Tartt", "123AaC!@#");
                    Thread.Sleep(500);
                    id3 = proxyBridge.Login(id3, "Tartt@amazon.io", "123AaC!@#").Value;
                    proxyBridge.AddItemToCart(id3, storeid1, itemid11, 1);
                    Thread.Sleep(500);
                    return proxyBridge.GetDetailsOnCart(id3);
                }),
                Task.Run(() =>
                {
                    id4=proxyBridge.Enter().Value;
                    proxyBridge.Register(id4, "Tartt@amazon.io","Jamie", "Tartt", "123AaC!@#");
                    Thread.Sleep(10000);
                    id4 = proxyBridge.Login(id4, "Tartt@amazon.io", "123AaC!@#").Value;
                    proxyBridge.AddItemToCart(id4, storeid1, itemid11, 1);
                    Thread.Sleep(10);
                    return proxyBridge.GetDetailsOnCart(id4);
                }),
                Task.Run(() =>
                {
                    id5=proxyBridge.Enter().Value;
                    proxyBridge.Register(id5, "Obysania@amazon.io","Sami", "Obysania", "123AaC!@#");
                    Thread.Sleep(7);
                    id5 = proxyBridge.Login(id5, "Obysania@amazon.io", "123AaC!@#").Value;
                    proxyBridge.GetDetailsOnCart(id5);
                    Thread.Sleep(777);
                    proxyBridge.AddItemToCart(id5, storeid1, itemid11, 1);
                    proxyBridge.AddItemToCart(id5, storeid2, itemid2, 3);
                    Thread.Sleep(1);
                    return proxyBridge.GetDetailsOnCart(id5);
                }),
                Task.Run(() =>
                {
                    id6=proxyBridge.Enter().Value;
                    proxyBridge.Register(id6, "Mass@amazon.io","Dan", "Mass", "123AaC!@#");
                    Thread.Sleep(100);
                    id6 = proxyBridge.Login(id6, "Mass@amazon.io", "123AaC!@#").Value;
                    proxyBridge.AddItemToCart(id6, storeid1, itemid11, 1);
                    Thread.Sleep(100);
                    return proxyBridge.GetDetailsOnCart(id6);
                }),
                Task.Run(() =>
                {
                    Thread.Sleep(100);
                    id7=proxyBridge.Enter().Value;
                    proxyBridge.Register(id7, "Mass@amazon.io","Dan", "Mass", "123AaC!@#");
                    Thread.Sleep(200);
                    id7 = proxyBridge.Login(id7, "Mass@amazon.io", "123AaC!@#").Value;
                    proxyBridge.AddItemToCart(id7, storeid1, itemid11, 1);
                    Thread.Sleep(1000);
                    return proxyBridge.GetDetailsOnCart(id7);
                }),
                Task.Run(() =>
                {
                    Thread.Sleep(100);
                    id8=proxyBridge.Enter().Value;
                    Thread.Sleep(200);
                    proxyBridge.AddItemToCart(id8, storeid1, itemid11, 1);
                    Thread.Sleep(2100);
                    return proxyBridge.GetDetailsOnCart(id8);
                }),
                Task.Run(() =>
                {
                    Thread.Sleep(100);
                    id9=proxyBridge.Enter().Value;
                    Thread.Sleep(300);
                    proxyBridge.AddItemToCart(id9, storeid1, itemid11, 3);
                    Thread.Sleep(100);
                    return proxyBridge.GetDetailsOnCart(id9);
                }),
            };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);
            Assert.IsFalse(clientTasks[0].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id1).Value.Baskets.Count == 0);

            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id2).Value.Baskets.Count == 0);
            Assert.IsTrue(clientTasks[2].Result.ErrorOccured || clientTasks[3].Result.ErrorOccured); //no error occurred

            Assert.IsFalse(clientTasks[4].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id5).Value.Baskets.Count == 2);
            Assert.IsTrue(clientTasks[5].Result.ErrorOccured || clientTasks[6].Result.ErrorOccured); //no error occurred

            Assert.IsFalse(clientTasks[7].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id8).Value.Baskets.Count == 1);
            
            Assert.IsFalse(clientTasks[8].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id9).Value.Baskets.Count == 1);
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id9).Value.GetItemQuantityInCart(storeid1, itemid11) == 3);
        }

        #endregion
        
        #region Many users try to purchase items
        [TestMethod]
        [TestCategory("Concurrency")]
        public void ManyUsersPurchasingItems()
        {
            Guid userId1 = Guid.Empty;
            Guid userId2 = Guid.Empty;
            Guid userId3 = Guid.Empty;
            Guid userId4 = Guid.Empty;
            Guid userId5 = Guid.Empty;
            Guid userId6 = Guid.Empty;
            Guid userId7 = Guid.Empty;
            Guid userId8 = Guid.Empty;
            Guid userId9 = Guid.Empty;
            Guid userId10 = Guid.Empty;

            Guid loggedId1 = Guid.Empty;
            Guid loggedId2 = Guid.Empty;
            Guid loggedId3 = Guid.Empty;
            Guid loggedId4 = Guid.Empty;
            Guid loggedId5 = Guid.Empty;
            Guid loggedId6 = Guid.Empty;

            Guid storeId1 = Guid.Empty;
            Guid storeId2 = Guid.Empty;

            Guid itemId1 = Guid.Empty;
            Guid itemId2 = Guid.Empty;

            Task<Response>[] clientTasks = new Task<Response>[] {
                Task.Run( () => {
                    userId1 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId1, "radwan1111@gmail.com", "radwan", "ganem", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId1 = proxyBridge.Login(userId1, "radwan1111@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);
                    storeId1 = proxyBridge.OpenNewStore(loggedId1, "FruitsStore").Value;

                    itemId1 = proxyBridge.AddItemToStore(loggedId1, storeId1, "apple", "fruits", 5.99, 1).Value;
                    return proxyBridge.EditItem(loggedId1, storeId1, itemId1, "lemon", "fruits", 5.99, 0);
                }),
                Task.Run( () => {
                    Thread.Sleep(200);
                    userId2 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId2, "david1111@gmail.com", "david", "beckham", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId2 = proxyBridge.Login(userId2, "david1111@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);

                    Thread.Sleep(20);
                    return proxyBridge.AddItemToCart(loggedId2, storeId1, itemId1, 2);

                }),
                Task.Run( () => {
                    Thread.Sleep(200);
                    userId3 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId3, "john1111@gmail.com", "john", "wick", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId3 = proxyBridge.Login(userId3, "john1111@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);

                    Thread.Sleep(200);
                    Response resp = proxyBridge.AddItemToCart(loggedId3, storeId1, itemId1, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(loggedId3, transactionDetails, transactionDetailsSupply);
                }),
                Task.Run( () => {
                    Thread.Sleep(210);
                    userId4 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId4, "alice1111@gmail.com", "alice", "ford", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId4 = proxyBridge.Login(userId4, "alice1111@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(100);
                    Response resp = proxyBridge.AddItemToCart(loggedId4, storeId1, itemId1, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(loggedId4, transactionDetails, transactionDetailsSupply);
                }),
                Task.Run( () => {
                    Thread.Sleep(220);
                    userId5 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId5, "ragnar1111@gmail.com", "ragnar", "lothbrok", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId5 = proxyBridge.Login(userId5, "ragnar1111@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);

                    Thread.Sleep(300);
                    Response resp = proxyBridge.AddItemToCart(loggedId5, storeId1, itemId1, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(loggedId5, transactionDetails, transactionDetailsSupply);

                }),
                Task.Run( () => {
                    userId6 = proxyBridge.Enter().Value;
                    proxyBridge.Register(userId6, "another1111@gmail.com", "another", "oneone", "123AaC!@#");
                     Thread.Sleep(600);
                    loggedId6 = proxyBridge.Login(userId6, "another1111@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);
                    storeId2 = proxyBridge.OpenNewStore(loggedId6, "VegtablesStore").Value;

                    itemId2 = proxyBridge.AddItemToStore(loggedId6, storeId2, "tomato", "vegtables", 3.99, 2).Value;
                    return proxyBridge.EditItem(loggedId6, storeId2, itemId2, "potato","vegtables", 3.99, 0);
                }),
                Task.Run( () => {
                    Thread.Sleep(5000);
                    userId7 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    Response resp = proxyBridge.AddItemToCart(userId7, storeId2, itemId2, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    Thread.Sleep(20);
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(userId7, transactionDetails, transactionDetailsSupply);

                }),
                Task.Run( () => {
                    Thread.Sleep(5000);
                    userId8 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    Response resp = proxyBridge.AddItemToCart(userId8, storeId2, itemId2, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    Thread.Sleep(20);
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(userId8, transactionDetails, transactionDetailsSupply);
                }),
                Task.Run( () => {
                    Thread.Sleep(5000);
                    userId9 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    Response resp = proxyBridge.AddItemToCart(userId9, storeId2, itemId2, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    Thread.Sleep(20);
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(userId9, transactionDetails, transactionDetailsSupply);
                }),
                Task.Run( () => {
                    Thread.Sleep(5000);
                    userId10 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    Response resp = proxyBridge.AddItemToCart(userId10, storeId2, itemId2, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    Thread.Sleep(20);
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(userId10, transactionDetails, transactionDetailsSupply);

                })
            };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            // explanation:
            // 1 user register and then login then open store with item with amount 1
            // then 3 user attempt to add it to cart and then purchase it and 1 user attempt to add item with amount 2 to cart
            // another 1 user register and then login then open store with item with amount 2
            // then 4 guests attempt to purchase, each added 1 item to cart

            Assert.IsFalse(clientTasks[0].Result.ErrorOccured);//no error occurred

            Assert.IsTrue(clientTasks[1].Result.ErrorOccured);// error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(loggedId2).Value.Baskets.Count == 0); // failed to add item to cart


            // only one purhcase successed, 2 failed
            int cntFailed = 0;
            int cntSuccess = 0;
            for (int i=2; i<5; i++)
            {
                if (clientTasks[i].Result.ErrorOccured)
                    cntFailed++;
                else
                    cntSuccess++;
            }
            Assert.IsTrue(cntSuccess.Equals(1));
            Assert.IsTrue(cntFailed.Equals(2));
            Assert.IsTrue(proxyBridge.GetStore(storeId1).Value.itemsInventory.items_quantity[proxyBridge.GetStore(storeId1).Value.GetItemById(itemId1)].Equals(0));

            Assert.IsFalse(clientTasks[5].Result.ErrorOccured);//no error occurred

            // only two purhcases successed, 2 failed
            cntFailed = 0;
            cntSuccess = 0;
            for (int i = 6; i < 10; i++)
            {
                if (clientTasks[i].Result.ErrorOccured)
                    cntFailed++;
                else
                    cntSuccess++;
            }
            Assert.IsTrue(cntFailed.Equals(2));
            Assert.IsTrue(cntSuccess.Equals(2));

            Assert.IsTrue(proxyBridge.GetStore(storeId2).Value.itemsInventory.items_quantity[proxyBridge.GetStore(storeId2).Value.GetItemById(itemId2)].Equals(0));
        }
        #endregion 
        
        #region Many users try purchase last item
        [TestMethod]
        [TestCategory("Concurrency")]
        public void ManyUsersTryPurchaseLastItem()
        {
            // stores = new ConcurrentDictionary<Guid, Store>();
            // Store store1 = new Store("Zara");
            // storeid1 = store1.StoreID;
            // itemid1 = store1.AddItem("Tshirt", "clothes", 99.8, 40);
            // itemid22 = store1.AddItem("Ipad", "electronic", 99.8, 2);
            // itemid11 = store1.AddItem("Dress", "clothes", 70, 45);
            // Store store2 = new Store("Fox");
            // storeid2 = store2.StoreID;
            // itemid2 = store2.AddItem("Pants", "clothes", 150, 200);
            // store2.AddItem("Towel", "Home", 40, 450);
            // store2.AddItem("Teddy bear toy", "children toys", 65, 120);
            // itemNoStock = store2.AddItem("mouse", "animals", 65, 0);
            
            Guid id1 = Guid.Empty;
            Guid id2 = Guid.Empty;
            Guid id3 = Guid.Empty;
            Guid id4 = Guid.Empty;
            Guid id5 = Guid.Empty;
            Guid id6 = Guid.Empty;
            Guid id7 = Guid.Empty;
            Guid id8 = Guid.Empty;
            Guid id9 = Guid.Empty;
            Guid id10 = Guid.Empty;
            Guid storeID = Guid.Empty;
            Guid newitemID = Guid.Empty;

            Task<ResponseT<List<ItemForOrder>>> task1 = Task.Run(() =>
            {
                id1 = proxyBridge.Enter().Value;
                proxyBridge.Register(id1, "Ted@amazon.io", "Ted", "Lasso", "123AaC!@#");
                id1 = proxyBridge.Login(id1, "Ted@amazon.io", "123AaC!@#").Value;
                Thread.Sleep(1000);
                proxyBridge.AddItemToCart(id1, storeid1, itemid1, 39);
                Thread.Sleep(10000);
                proxyBridge.AddItemToCart(id1, storeid1, itemid1, 1);
                SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                return proxyBridge.PurchaseCart(id1, transactionDetails, transactionDetailsSupply);
            });
            Task<ResponseT<List<ItemForOrder>>> task2 = Task.Run(() =>
            {
                id2 = proxyBridge.Enter().Value;
                proxyBridge.Register(id2, "Roy@amazon.io", "Roy", "Kent", "123AaC!@#");
                Thread.Sleep(500);
                id2 = proxyBridge.Login(id2, "Roy@amazon.io", "123AaC!@#").Value;
                Thread.Sleep(1000);
                proxyBridge.AddItemToCart(id2, storeid1, itemid1, 9);
                Thread.Sleep(1000);
                proxyBridge.AddItemToCart(id2, storeid1, itemid1, 9);
                SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                return proxyBridge.PurchaseCart(id2, transactionDetails, transactionDetailsSupply);
            });
            Task<Response> task3 = Task.Run(() =>
            {
                id3 = proxyBridge.Enter().Value;
                proxyBridge.Register(id3, "Tartt@amazon.io", "Jamie", "Tartt", "123AaC!@#");
                Thread.Sleep(500);
                id3 = proxyBridge.Login(id3, "Tartt@amazon.io", "123AaC!@#").Value;
                proxyBridge.AddItemToCart(id3, storeid1, itemid22, 1);
                Thread.Sleep(2000);
                return proxyBridge.AddItemToCart(id3, storeid1, itemid22, 1);
            });
            Task<Response> task4 = Task.Run(() =>
            {
                id4 = proxyBridge.Enter().Value;
                proxyBridge.Register(id4, "Tartt@amazon.io", "Jamie", "Tartt", "123AaC!@#");
                Thread.Sleep(10000);
                id4 = proxyBridge.Login(id4, "Tartt@amazon.io", "123AaC!@#").Value;
                proxyBridge.AddItemToCart(id4, storeid1, itemid11, 1);
                Thread.Sleep(10);
                return proxyBridge.AddItemToCart(id4, storeid1, itemid22, 1);
            });
            Task<Response> task5 = Task.Run(() =>
            {
                id5 = proxyBridge.Enter().Value;
                proxyBridge.Register(id5, "Obysania@amazon.io", "Sami", "Obysania", "123AaC!@#");
                Thread.Sleep(7);
                id5 = proxyBridge.Login(id5, "Obysania@amazon.io", "123AaC!@#").Value;
                proxyBridge.GetDetailsOnCart(id5);
                Thread.Sleep(777);
                proxyBridge.AddItemToCart(id5, storeid1, itemid11, 1);
                proxyBridge.AddItemToCart(id5, storeid2, itemid11, 3);
                Thread.Sleep(1);
                return proxyBridge.AddItemToCart(id5, storeid1, itemid11, 1);
            });
            Task<Response> task6 = Task.Run(() =>
            {
                id6 = proxyBridge.Enter().Value;
                proxyBridge.Register(id6, "Mass@amazon.io", "Dan", "Mass", "123AaC!@#");
                id6 = proxyBridge.Login(id3, "Mass@amazon.io", "123AaC!@#").Value;

                Thread.Sleep(100);
                storeID = proxyBridge.OpenNewStore(id6, "StoreWillNotShouldOpen").Value;
                return proxyBridge.AddItemToCart(id6, storeID, itemid11, 1);
            });
            Task<Response> task7 = Task.Run(() =>
            {
                Thread.Sleep(100);
                id7 = proxyBridge.Enter().Value;
                proxyBridge.Register(id7, "JOnny@amazon.io", "Dan", "Mass", "123AaC!@#");
                Thread.Sleep(200);
                id7 = proxyBridge.Login(id7, "JOnny@amazon.io", "123AaC!@#").Value;
                storeID = proxyBridge.OpenNewStore(id7, "StoreWillShouldOpen").Value;
                newitemID = proxyBridge.AddItemToStore(id7, storeID, "Cat", "Animal", 100.0, 1).Value;
                return proxyBridge.Exit(id7);
            });
            Task<Response> task8 = Task.Run(() =>
            {
                Thread.Sleep(100);
                id8 = proxyBridge.Enter().Value;
                proxyBridge.Register(id8, "oMAHA1@amazon.io", "Dan", "Mass", "123AaC!@#");
                Thread.Sleep(200);
                id8 = proxyBridge.Login(id8, "oMAHA1@amazon.io", "123AaC!@#").Value;
                Store s = proxyBridge.GetStore(storeID).Value;
                proxyBridge.AddItemToCart(id8, storeID, newitemID, 1);
                SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                proxyBridge.PurchaseCart(id8, transactionDetails, transactionDetailsSupply);
                return proxyBridge.Exit(id8);
            });
            Task<Response> task9 = Task.Run(() =>
            {
                Thread.Sleep(100);
                id9 = proxyBridge.Enter().Value;
                proxyBridge.Register(id9, "oMAHA2@amazon.io", "Dan", "Mass", "123AaC!@#");
                Thread.Sleep(200);
                id9 = proxyBridge.Login(id9, "oMAHA2@amazon.io", "123AaC!@#").Value;
                Store s = proxyBridge.GetStore(storeID).Value;
                proxyBridge.AddItemToCart(id9, storeID, newitemID, 1);
                SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                proxyBridge.PurchaseCart(id9, transactionDetails, transactionDetailsSupply);
                return proxyBridge.Exit(id9);
            });

            // Wait for all clients to complete
            Task.WaitAll();

            Assert.IsTrue(task1.Result.ErrorOccured || task2.Result.ErrorOccured);//no error occurred

            Assert.IsFalse(task1.Result.ErrorOccured && task2.Result.ErrorOccured);//no error occurred

            Assert.IsTrue(task3.Result.ErrorOccured || task4.Result.ErrorOccured); //no error occurred
            
            Assert.IsFalse(task5.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id5).Value.Baskets.Count == 1);
            
            Assert.IsTrue(task6.Result.ErrorOccured); //no error occurred\   

            if (!task7.Result.ErrorOccured)
            {
                Assert.IsNotNull(proxyBridge.GetStore(storeID).Value);
                Assert.IsFalse(task8.Result.ErrorOccured && task9.Result.ErrorOccured);//no error occurred
                Assert.IsFalse(proxyBridge.GetStore(storeid1).Value.itemsInventory.ItemExist(newitemID));

            }
            else
            {
                Assert.IsFalse(task7.Result.ErrorOccured && task8.Result.ErrorOccured &&
                               task9.Result.ErrorOccured);
            }
        }
        #endregion
        
        #region Two users try to purchase item that store owner deleting
        [TestMethod]
        [TestCategory("Concurrency")]
        public void StoreOwnerDeletingItemUserPurchase()
        {
            Guid id1 = Guid.Empty;
            Guid id2 = Guid.Empty;
            Guid id3 = Guid.Empty;
            Guid id4 = Guid.Empty;
            Guid id5 = Guid.Empty;
            Guid id6 = Guid.Empty;
            Guid id7 = Guid.Empty;
            Guid id8 = Guid.Empty;
            Guid id9 = Guid.Empty;
            Guid storeID = Guid.Empty;
            Guid newitemID = Guid.Empty;
            Task<ResponseT<List<ItemForOrder>>> task1 = Task.Run(() =>
            {
                id1 = proxyBridge.Enter().Value;
                proxyBridge.Register(id1, "Ted@amazon.io", "Ted", "Lasso", "123AaC!@#");
                id1 = proxyBridge.Login(id1, "Ted@amazon.io", "123AaC!@#").Value;
                Thread.Sleep(1000);
                proxyBridge.AddItemToCart(id1, storeid1, itemid1, 39);
                Thread.Sleep(10000);
                proxyBridge.AddItemToCart(id1, storeid1, itemid1, 1);
                SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                return proxyBridge.PurchaseCart(id1, transactionDetails, transactionDetailsSupply);
            });
            
            Task<ResponseT<List<ItemForOrder>>> task2 = Task.Run(() =>
            {
                id2 = proxyBridge.Enter().Value;
                proxyBridge.Register(id2, "Roy@amazon.io", "Roy", "Kent", "123AaC!@#");
                Thread.Sleep(500);
                id2 = proxyBridge.Login(id2, "Roy@amazon.io", "123AaC!@#").Value;
                Thread.Sleep(1000);
                proxyBridge.AddItemToCart(id2, storeid1, itemid1, 9);
                Thread.Sleep(1000);
                proxyBridge.AddItemToCart(id2, storeid1, itemid1, 9);
                SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                return proxyBridge.PurchaseCart(id2, transactionDetails, transactionDetailsSupply);
            });
            Task<Response> task3 = Task.Run(() =>
            {
                id3 = proxyBridge.Enter().Value;
                proxyBridge.Register(id3, "Tartt@amazon.io", "Jamie", "Tartt", "123AaC!@#");
                Thread.Sleep(500);
                id3 = proxyBridge.Login(id3, "Tartt@amazon.io", "123AaC!@#").Value;
                proxyBridge.AddItemToCart(id3, storeid1, itemid22, 1);
                Thread.Sleep(2000);
                return proxyBridge.AddItemToCart(id3, storeid1, itemid22, 1);
            });
                Task<Response> task4 = Task.Run(() =>
            {
                id4 = proxyBridge.Enter().Value;
                proxyBridge.Register(id4, "Tartt@amazon.io", "Jamie", "Tartt", "123AaC!@#");
                Thread.Sleep(10000);
                id4 = proxyBridge.Login(id4, "Tartt@amazon.io", "123AaC!@#").Value;
                proxyBridge.AddItemToCart(id4, storeid1, itemid11, 1);
                Thread.Sleep(10);
                return proxyBridge.AddItemToCart(id4, storeid1, itemid22, 1);
            });
                Task<Response> task5 = Task.Run(() =>
            {
                id5 = proxyBridge.Enter().Value;
                proxyBridge.Register(id5, "Obysania@amazon.io", "Sami", "Obysania", "123AaC!@#");
                Thread.Sleep(7);
                id5 = proxyBridge.Login(id5, "Obysania@amazon.io", "123AaC!@#").Value;
                proxyBridge.GetDetailsOnCart(id5);
                Thread.Sleep(777);
                proxyBridge.AddItemToCart(id5, storeid1, itemid11, 1);
                proxyBridge.AddItemToCart(id5, storeid2, itemid11, 3);
                Thread.Sleep(1);
                return proxyBridge.AddItemToCart(id5, storeid1, itemid11, 1);
            });
                Task<Response> task6 = Task.Run(() =>
            {
                id6 = proxyBridge.Enter().Value;
                proxyBridge.Register(id6, "Mass@amazon.io", "Dan", "Mass", "123AaC!@#");
                id6 = proxyBridge.Login(id3, "Mass@amazon.io", "123AaC!@#").Value;
                Thread.Sleep(100);
                storeID = proxyBridge.OpenNewStore(id6, "StoreWillNotShouldOpen").Value;
                return proxyBridge.AddItemToCart(id6, storeID, itemid11, 1);
            });
                Task<Response> task7 = Task.Run(() =>
            {
                Thread.Sleep(100);
                id7 = proxyBridge.Enter().Value;
                Thread.Sleep(200);
                id7 = proxyBridge.Login(id7, "AsiAzar@gmail.com", "A#!a12345678").Value;
                proxyBridge.RemoveItemFromStore(id7, storeid1, itemid22);
                return proxyBridge.Exit(id7);
            });
                Task<Response> task8 = Task.Run(() =>
            {
                Thread.Sleep(100);
                id8 = proxyBridge.Enter().Value;
                proxyBridge.Register(id8, "oMAHA1@amazon.io", "Dan", "Mass", "123AaC!@#");
                Thread.Sleep(200);
                id8 = proxyBridge.Login(id8, "oMAHA1@amazon.io", "123AaC!@#").Value;
                proxyBridge.AddItemToCart(id8, storeid1, itemid22, 1);
                SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                proxyBridge.PurchaseCart(id8, transactionDetails, transactionDetailsSupply);
                return proxyBridge.Exit(id8);
            });
                Task<Response> task9 = Task.Run(() =>
            {
                Thread.Sleep(100);
                id9 = proxyBridge.Enter().Value;
                proxyBridge.Register(id9, "oMAHA2@amazon.io", "Dan", "Mass", "123AaC!@#");
                Thread.Sleep(200);
                id9 = proxyBridge.Login(id9, "oMAHA2@amazon.io", "123AaC!@#").Value;
                Store s = proxyBridge.GetStore(storeID).Value;
                proxyBridge.AddItemToCart(id9, storeid1, itemid22, 1);
                SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                proxyBridge.PurchaseCart(id9, transactionDetails, transactionDetailsSupply);
                return proxyBridge.Exit(id9);
            });
        

            // Wait for all clients to complete
            Task.WaitAll();

            Assert.IsTrue(task1.Result.ErrorOccured || task2.Result.ErrorOccured);//no error occurred

            Assert.IsFalse(task1.Result.ErrorOccured && task2.Result.ErrorOccured);//no error occurred

            Assert.IsTrue(task3.Result.ErrorOccured || task4.Result.ErrorOccured); //no error occurred
            
            Assert.IsFalse(task5.Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetDetailsOnCart(id5).Value.Baskets.Count == 1);
            
            Assert.IsTrue(task6.Result.ErrorOccured); //no error occurred\   
            
            if (!task7.Result.ErrorOccured)
            {
                Assert.IsFalse(proxyBridge.GetStore(storeid1).Value.itemsInventory.ItemExist(itemid22));
            }
            else
            {
                Assert.IsFalse(task7.Result.ErrorOccured && task8.Result.ErrorOccured &&
                               task9.Result.ErrorOccured);
            }
        }
        #endregion

        #region Many users try to purchase items at the same time store managers changing items price
        [TestMethod]
        [TestCategory("Concurrency")]
        public void ManyUsersPurchasingItemsWhileManagerEditingPrice()
        {
            Guid userId1 = Guid.Empty;
            Guid userId2 = Guid.Empty;
            Guid userId3 = Guid.Empty;
            Guid userId4 = Guid.Empty;
            Guid userId5 = Guid.Empty;
            Guid userId6 = Guid.Empty;
            Guid userId7 = Guid.Empty;
            Guid userId8 = Guid.Empty;
            Guid userId9 = Guid.Empty;
            Guid userId10 = Guid.Empty;

            Guid loggedId1 = Guid.Empty;
            Guid loggedId2 = Guid.Empty;
            Guid loggedId3 = Guid.Empty;
            Guid loggedId4 = Guid.Empty;
            Guid loggedId5 = Guid.Empty;
            Guid loggedId6 = Guid.Empty;

            Guid storeId1 = Guid.Empty;
            Guid storeId2 = Guid.Empty;

            Guid itemId1 = Guid.Empty;
            Guid itemId2 = Guid.Empty;

            Task<Response>[] clientTasks = new Task<Response>[] {
                Task.Run( () => {
                    userId1 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId1, "radwan4444@gmail.com", "radwan", "ganem", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId1 = proxyBridge.Login(userId1, "radwan4444@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);
                    storeId1 = proxyBridge.OpenNewStore(loggedId1, "FruitsStore").Value;

                    itemId1 = proxyBridge.AddItemToStore(loggedId1, storeId1, "apple", "fruits", 10, 1).Value;
                    return proxyBridge.EditItem(loggedId1, storeId1, itemId1, "apple", "fruits", 40, 0);
                }),
                Task.Run( () => {
                    Thread.Sleep(1000);
                    userId2 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId2, "david3333@gmail.com", "david", "beckham", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId2 = proxyBridge.Login(userId2, "david3333@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);

                    Thread.Sleep(20);
                    return proxyBridge.AddItemToCart(loggedId2, storeId1, itemId1, 2);

                }),
                Task.Run( () => {
                    Thread.Sleep(2000);
                    userId3 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId3, "john3333@gmail.com", "john", "wick", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId3 = proxyBridge.Login(userId3, "john3333@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);

                    Thread.Sleep(300);
                    Response resp = proxyBridge.AddItemToCart(loggedId3, storeId1, itemId1, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(loggedId3, transactionDetails, transactionDetailsSupply);
                }),
                Task.Run( () => {
                    Thread.Sleep(2000);
                    userId4 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId4, "alice3333@gmail.com", "alice", "ford", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId4 = proxyBridge.Login(userId4, "alice3333@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(300);
                    Response resp = proxyBridge.AddItemToCart(loggedId4, storeId1, itemId1, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(loggedId4, transactionDetails, transactionDetailsSupply);
                }),
                Task.Run( () => {
                    Thread.Sleep(2000);
                    userId5 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId5, "ragnar3333@gmail.com", "ragnar", "lothbrok", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId5 = proxyBridge.Login(userId5, "ragnar3333@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);

                    Thread.Sleep(300);
                    Response resp = proxyBridge.AddItemToCart(loggedId5, storeId1, itemId1, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(loggedId5, transactionDetails, transactionDetailsSupply);

                }),
                Task.Run( () => {
                    userId6 = proxyBridge.Enter().Value;
                    proxyBridge.Register(userId6, "another3333@gmail.com", "another", "oneone", "123AaC!@#");
                     Thread.Sleep(600);
                    loggedId6 = proxyBridge.Login(userId6, "another3333@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);
                    storeId2 = proxyBridge.OpenNewStore(loggedId6, "VegtablesStore").Value;

                    itemId2 = proxyBridge.AddItemToStore(loggedId6, storeId2, "tomato", "vegtables", 3.99, 2).Value;
                    return proxyBridge.EditItem(loggedId6, storeId2, itemId2, "tomato", "vegtables", 10, 0);
                }),
                Task.Run( () => {
                    Thread.Sleep(1000);
                    userId7 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    Response resp = proxyBridge.AddItemToCart(userId7, storeId2, itemId2, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    Assert.IsTrue(proxyBridge.GetItemByID(storeId2, itemId2).Value.Price.Equals(10));
                    Thread.Sleep(20);
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(userId7, transactionDetails, transactionDetailsSupply);

                }),
                Task.Run( () => {
                    Thread.Sleep(1000);
                    userId8 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    Response resp = proxyBridge.AddItemToCart(userId8, storeId2, itemId2, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    Assert.IsTrue(proxyBridge.GetItemByID(storeId2, itemId2).Value.Price.Equals(10));
                    Thread.Sleep(20);
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(userId8, transactionDetails, transactionDetailsSupply);
                }),
                Task.Run( () => {
                    Thread.Sleep(1000);
                    userId9 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    Response resp = proxyBridge.AddItemToCart(userId9, storeId2, itemId2, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    Assert.IsTrue(proxyBridge.GetItemByID(storeId2, itemId2).Value.Price.Equals(10));
                    Thread.Sleep(20);
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(userId9, transactionDetails, transactionDetailsSupply);
                }),
                Task.Run( () => {
                    Thread.Sleep(1000);
                    userId10 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    Response resp = proxyBridge.AddItemToCart(userId10, storeId2, itemId2, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    Assert.IsTrue(proxyBridge.GetItemByID(storeId2, itemId2).Value.Price.Equals(10));
                    Thread.Sleep(20);
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(userId10, transactionDetails, transactionDetailsSupply);

                })
            };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            // explanation:
            // 1 user register and then login then open store with item with amount 1
            // then 3 user attempt to add it to cart and then purchase it and 1 user attempt to add item with amount 2 to cart
            // while the store manager removes the item
            // another 1 user register and then login then open store with item with amount 2
            // then 4 guests attempt to purchase, each added 1 item to cart
            // while the store manager removes the item

            if (clientTasks[0].Result.ErrorOccured)
            {
                // first user failed to register and open store and add item
                Assert.IsTrue(clientTasks[0].Result.ErrorOccured); //error occurred
            }
            else
            {
                Assert.IsFalse(clientTasks[0].Result.ErrorOccured);//no error occurred

                Assert.IsTrue(clientTasks[1].Result.ErrorOccured);// error occurred
                Assert.IsTrue(proxyBridge.GetDetailsOnCart(loggedId2).Value.Baskets.Count == 0); // successed to add item to cart


                // all 3 purchase attempts successed while store manager adding the item quantity
                int cntFailed = 0;
                int cntSuccess = 0;
                for (int i = 2; i < 5; i++)
                {
                    if (clientTasks[i].Result.ErrorOccured)
                        cntFailed++;
                    else
                        cntSuccess++;
                }
                Assert.IsTrue(cntSuccess.Equals(1));
                Assert.IsTrue(cntFailed.Equals(2));
                // items has 0 quantity after the purchases
                Assert.IsTrue(proxyBridge.GetStore(storeId1).Value.itemsInventory.items_quantity[proxyBridge.GetStore(storeId1).Value.GetItemById(itemId1)].Equals(0));

                // price changed from 10 to 40
                Assert.IsTrue(proxyBridge.GetStore(storeId1).Value.GetItemById(itemId1).Price.Equals(40));

                Assert.IsFalse(clientTasks[5].Result.ErrorOccured);//no error occurred

                //  3 purchases successed , 1 failed 
                cntFailed = 0;
                cntSuccess = 0;
                for (int i = 6; i < 10; i++)
                {
                    if (clientTasks[i].Result.ErrorOccured)
                        cntFailed++;
                    else
                        cntSuccess++;
                }
                Assert.IsTrue(cntFailed.Equals(2));
                Assert.IsTrue(cntSuccess.Equals(2));

                // remaining quantity of item after store manager added and all purchases is 0
                Assert.IsTrue(proxyBridge.GetStore(storeId2).Value.itemsInventory.items_quantity[proxyBridge.GetStore(storeId2).Value.GetItemById(itemId2)].Equals(0));
                Assert.IsTrue(proxyBridge.GetStore(storeId2).Value.GetItemById(itemId2).Price.Equals(10.0));

                
            }
        }
        #endregion

        #region  many users try remove many store owners 

        [TestMethod]
        [TestCategory("Concurrency")]
        public void ManyUsersRemoveManyStoreOwner()
        {
            //Arrange
            // create store founder
            Guid storeFounder = proxyBridge.Enter().Value;
            proxyBridge.Register(storeFounder, "storeFounderMail@gmail.com", "radwan", "ganem", "A#!a12345678");
            storeFounder = proxyBridge.Login(storeFounder, "storeFounderMail@gmail.com", "A#!a12345678").Value;
            Guid storeID = proxyBridge.OpenNewStore(storeFounder, "Store 1").Value;
            
            // create owner 1
            Response res1 =proxyBridge.AppointStoreOwner(storeFounder, storeID, "gil@gmail.com");
            
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
            memberId7 = proxyBridge.Login(memberId7, "member7@gmail.com", "A#!a12345678").Value;
            proxyBridge.AppointStoreOwner(memberId3, storeID, "member7@gmail.com");
            
            //create appoint 1 to member6
            enter = proxyBridge.Enter().Value;
            proxyBridge.Login(enter, "member6@gmail.com", "A#!a12345678");
            
            Guid memberId8 = proxyBridge.Enter().Value;
            proxyBridge.Register(memberId8, "member8@gmail.com", "member", "member", "A#!a12345678");
            memberId8 = proxyBridge.Login(memberId8, "member8@gmail.com", "A#!a12345678").Value;
            proxyBridge.AppointStoreOwner(memberId6, storeID, "member8@gmail.com");
            
            //create appoint 1 to member 5
            enter = proxyBridge.Enter().Value;
            proxyBridge.Login(enter, "member5@gmail.com", "A#!a12345678");
            
            Guid memberId9 = proxyBridge.Enter().Value;
            proxyBridge.Register(memberId9, "member9@gmail.com", "member", "member", "A#!a12345678");
            memberId9 = proxyBridge.Login(memberId9, "member9@gmail.com", "A#!a12345678").Value;
            proxyBridge.AppointStoreOwner(memberId5, storeID, "member9@gmail.com");

            //Act
            proxyBridge.RemovePermission(storeFounder, storeID, "gil@gmail.com", "owner permissions");
            proxyBridge.RemovePermission(memberId, storeID, "sebatian@gmail.com", "owner permissions");
            proxyBridge.RemovePermission(memberId, storeID, "amihai@gmail.com", "owner permissions");
            proxyBridge.RemovePermission(memberId2, storeID, "bar@gmail.com", "owner permissions");
            proxyBridge.RemovePermission(memberId2, storeID, "member5@gmail.com", "owner permissions");
            proxyBridge.RemovePermission(memberId3, storeID, "member6@gmail.com", "owner permissions");
            proxyBridge.RemovePermission(memberId3, storeID, "member7@gmail.com", "owner permissions");
            proxyBridge.RemovePermission(memberId6, storeID, "member8@gmail.com", "owner permissions");
            proxyBridge.RemovePermission(memberId5, storeID, "member9@gmail.com", "owner permissions");
            //Assert
            //check that only the founder has left
            Assert.AreEqual(3, proxyBridge.GetEmployeeInfoInStore(storeFounder, storeID).Value.Count);
           // Assert.IsTrue( proxyBridge.GetEmployeeInfoInStore(storeFounder, storeID).Value.Contains((SMember)proxyBridge.GetMember(storeFounder).Value));
            //check that the other didnt have permissions to the store
            Assert.IsFalse(proxyBridge.GetMember(memberId).Value.hasPermissions(storeID, new List<string>{"owner permissions"}));
            Assert.IsFalse(proxyBridge.GetMember(memberId2).Value.hasPermissions(storeID, new List<string>{"owner permissions"}));
            Assert.IsFalse(proxyBridge.GetMember(memberId3).Value.hasPermissions(storeID, new List<string>{"owner permissions"}));
            Assert.IsFalse(proxyBridge.GetMember(memberId4).Value.hasPermissions(storeID, new List<string>{"owner permissions"}));
            Assert.IsFalse(proxyBridge.GetMember(memberId5).Value.hasPermissions(storeID, new List<string>{"owner permissions"}));
            Assert.IsFalse(proxyBridge.GetMember(memberId6).Value.hasPermissions(storeID, new List<string>{"owner permissions"}));
            Assert.IsFalse(proxyBridge.GetMember(memberId7).Value.hasPermissions(storeID, new List<string>{"owner permissions"}));
            Assert.IsFalse(proxyBridge.GetMember(memberId8).Value.hasPermissions(storeID, new List<string>{"owner permissions"}));
            Assert.IsFalse(proxyBridge.GetMember(memberId9).Value.hasPermissions(storeID, new List<string>{"owner permissions"}));
        }

        #endregion
        //test for next version 
        #region Many users try to purchase items at the same time store managers removes the items
        [TestCategory("Concurrency")]
        public void ManyUsersPurchasingItemsWhileManagerDeletingThem()
        {
            Guid userId1 = Guid.Empty;
            Guid userId2 = Guid.Empty;
            Guid userId3 = Guid.Empty;
            Guid userId4 = Guid.Empty;
            Guid userId5 = Guid.Empty;
            Guid userId6 = Guid.Empty;
            Guid userId7 = Guid.Empty;
            Guid userId8 = Guid.Empty;
            Guid userId9 = Guid.Empty;
            Guid userId10 = Guid.Empty;

            Guid loggedId1 = Guid.Empty;
            Guid loggedId2 = Guid.Empty;
            Guid loggedId3 = Guid.Empty;
            Guid loggedId4 = Guid.Empty;
            Guid loggedId5 = Guid.Empty;
            Guid loggedId6 = Guid.Empty;

            Guid storeId1 = Guid.Empty;
            Guid storeId2 = Guid.Empty;

            Guid itemId1 = Guid.Empty;
            Guid itemId2 = Guid.Empty;

            Task<Response>[] clientTasks = new Task<Response>[] {
                Task.Run( () => {
                    userId1 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId1, "radwan2222@gmail.com", "radwann", "ganemm", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId1 = proxyBridge.Login(userId1, "radwan2222@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);
                    storeId1 = proxyBridge.OpenNewStore(loggedId1, "FruitsStore").Value;

                    itemId1 = proxyBridge.AddItemToStore(loggedId1, storeId1, "apple", "fruits", 5.99, 1).Value;

                    Thread.Sleep(100);
                    return proxyBridge.RemoveItemFromStore(loggedId1, storeId1, itemId1);

                }),
                Task.Run( () => {
                    Thread.Sleep(210);
                    userId2 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId2, "david2222@gmail.com", "david", "beckham", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId2 = proxyBridge.Login(userId2, "david2222@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);

                    Thread.Sleep(20);
                    return proxyBridge.AddItemToCart(loggedId2, storeId1, itemId1, 2);

                }),
                Task.Run( () => {
                    Thread.Sleep(210);
                    userId3 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId3, "john2222@gmail.com", "john", "wick", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId3 = proxyBridge.Login(userId3, "john2222@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);

                    Thread.Sleep(200);
                    Response resp = proxyBridge.AddItemToCart(loggedId3, storeId1, itemId1, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(loggedId3, transactionDetails, transactionDetailsSupply);
                }),
                Task.Run( () => {
                    Thread.Sleep(210);
                    userId4 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId4, "alice2222@gmail.com", "alice", "ford", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId4 = proxyBridge.Login(userId4, "alice2222@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(100);
                    Response resp =proxyBridge.AddItemToCart(loggedId4, storeId1, itemId1, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(loggedId4, transactionDetails, transactionDetailsSupply);
                }),
                Task.Run( () => {
                    Thread.Sleep(220);
                    userId5 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId5, "ragnar2222@gmail.com", "ragnar", "lothbrok", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId5 = proxyBridge.Login(userId5, "ragnar2222@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);

                    Thread.Sleep(300);
                    Response resp = proxyBridge.AddItemToCart(loggedId5, storeId1, itemId1, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(loggedId5, transactionDetails, transactionDetailsSupply);

                }),
                Task.Run( () => {
                    userId6 = proxyBridge.Enter().Value;
                    proxyBridge.Register(userId6, "another2222@gmail.com", "another", "oneone", "123AaC!@#");
                     Thread.Sleep(600);
                    loggedId6 = proxyBridge.Login(userId6, "another2222@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);
                    storeId2 = proxyBridge.OpenNewStore(loggedId6, "VegtablesStore").Value;
                    Thread.Sleep(20);
                    itemId2 = proxyBridge.AddItemToStore(loggedId6, storeId2, "tomato", "vegtables", 3.99, 2).Value;
                    Thread.Sleep(800);
                    return proxyBridge.RemoveItemFromStore(loggedId6, storeId2, itemId2);
                }),
                Task.Run( () => {
                    Thread.Sleep(1000);
                    userId7 = proxyBridge.Enter().Value;
                    Thread.Sleep(3000);
                    Response resp = proxyBridge.AddItemToCart(userId7, storeId2, itemId2, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    Thread.Sleep(20);
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(userId7, transactionDetails, transactionDetailsSupply);

                }),
                Task.Run( () => {
                    Thread.Sleep(1000);
                    userId8 = proxyBridge.Enter().Value;
                    Thread.Sleep(200);
                    Response resp =proxyBridge.AddItemToCart(userId8, storeId2, itemId2, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    Thread.Sleep(20);
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(userId8, transactionDetails, transactionDetailsSupply);
                }),
                Task.Run( () => {
                    Thread.Sleep(1000);
                    userId9 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    Thread.Sleep(3000);
                    Response resp = proxyBridge.AddItemToCart(userId9, storeId2, itemId2, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    Thread.Sleep(20);
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(userId9, transactionDetails, transactionDetailsSupply);
                }),
                Task.Run( () => {
                    Thread.Sleep(1000);
                    userId10 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    Thread.Sleep(3000);
                    Response resp = proxyBridge.AddItemToCart(userId10, storeId2, itemId2, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    Thread.Sleep(20);
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(userId10, transactionDetails, transactionDetailsSupply);

                })
            };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            // explanation:
            // 1 user register and then login then open store with item with amount 1
            // then 3 user attempt to add it to cart and then purchase it and 1 user attempt to add item with amount 2 to cart
            // while the store manager removes the item
            // another 1 user register and then login then open store with item with amount 2
            // then 4 guests attempt to purchase, each added 1 item to cart
            // while the store manager removes the item

            if (clientTasks[0].Result.ErrorOccured)
            {
                // first user failed to register and open store and add item
                Assert.IsTrue(clientTasks[0].Result.ErrorOccured); //error occurred
            }
            else
            {
                Assert.IsFalse(clientTasks[0].Result.ErrorOccured);//no error occurred

                Assert.IsTrue(clientTasks[1].Result.ErrorOccured);// error occurred
                Assert.IsTrue(proxyBridge.GetDetailsOnCart(loggedId2).Value.Baskets.Count == 0); // failed to add item to cart


                // all 3 purchase attempts fail while store manager removes the item
                int cntFailed = 0;
                int cntSuccess = 0;
                for (int i = 2; i < 5; i++)
                {
                    if (clientTasks[i].Result.ErrorOccured)
                        cntFailed++;
                    else
                        cntSuccess++;
                }
                Assert.IsTrue(cntSuccess.Equals(0));
                Assert.IsTrue(cntFailed.Equals(3));
                // throws exception because items doesnt exist after store manager deleted it
                Assert.ThrowsException<Exception>(() => proxyBridge.GetStore(storeId1).Value.itemsInventory.items_quantity[proxyBridge.GetStore(storeId1).Value.GetItemById(itemId1)].Equals(0));

                Assert.IsFalse(clientTasks[5].Result.ErrorOccured);//no error occurred

                // only 1 purchase successed , 3 purchases failed, one user succefully added item to cart and  
                // purchased it before store manager removes the item
                cntFailed = 0;
                cntSuccess = 0;
                for (int i = 6; i < 10; i++)
                {
                    if (clientTasks[i].Result.ErrorOccured)
                        cntFailed++;
                    else
                        cntSuccess++;
                }
                Assert.IsTrue(cntFailed.Equals(3));
                Assert.IsTrue(cntSuccess.Equals(1));

                // throws exception because items doesnt exist after store manager deleted it
                Assert.ThrowsException<Exception>(() => proxyBridge.GetStore(storeId2).Value.itemsInventory.items_quantity[proxyBridge.GetStore(storeId2).Value.GetItemById(itemId2)].Equals(0));

            }
        }
        #endregion
        
        #region Many users try to purchase items at the same time store managers adding items quantity
        [TestMethod]
        [TestCategory("Concurrency")]
        public void ManyUsersPurchasingItemsWhileManagerEditingQuantity()
        {
            Guid userId1 = Guid.Empty;
            Guid userId2 = Guid.Empty;
            Guid userId3 = Guid.Empty;
            Guid userId4 = Guid.Empty;
            Guid userId5 = Guid.Empty;
            Guid userId6 = Guid.Empty;
            Guid userId7 = Guid.Empty;
            Guid userId8 = Guid.Empty;
            Guid userId9 = Guid.Empty;
            Guid userId10 = Guid.Empty;

            Guid loggedId1 = Guid.Empty;
            Guid loggedId2 = Guid.Empty;
            Guid loggedId3 = Guid.Empty;
            Guid loggedId4 = Guid.Empty;
            Guid loggedId5 = Guid.Empty;
            Guid loggedId6 = Guid.Empty;

            Guid storeId1 = Guid.Empty;
            Guid storeId2 = Guid.Empty;

            Guid itemId1 = Guid.Empty;
            Guid itemId2 = Guid.Empty;

            Task<Response>[] clientTasks = new Task<Response>[] {
                Task.Run( () => {
                    userId1 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId1, "radwan3333@gmail.com", "radwan", "ganem", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId1 = proxyBridge.Login(userId1, "radwan3333@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);
                    storeId1 = proxyBridge.OpenNewStore(loggedId1, "FruitsStore").Value;

                    itemId1 = proxyBridge.AddItemToStore(loggedId1, storeId1, "apple", "fruits", 5.99, 1).Value;
                    return proxyBridge.EditItem(loggedId1, storeId1, itemId1,"apple", "fruits", 5.99, 9);
                }),
                Task.Run( () => {
                    Thread.Sleep(1000);
                    userId2 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId2, "david3333@gmail.com", "david", "beckham", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId2 = proxyBridge.Login(userId2, "david3333@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);

                    Thread.Sleep(20);
                    return proxyBridge.AddItemToCart(loggedId2, storeId1, itemId1, 2);

                }),
                Task.Run( () => {
                    Thread.Sleep(2000);
                    userId3 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId3, "john3333@gmail.com", "john", "wick", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId3 = proxyBridge.Login(userId3, "john3333@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);

                    Thread.Sleep(300);
                    Response resp = proxyBridge.AddItemToCart(loggedId3, storeId1, itemId1, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(loggedId3, transactionDetails, transactionDetailsSupply);
                }),
                Task.Run( () => {
                    Thread.Sleep(2000);
                    userId4 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId4, "alice3333@gmail.com", "alice", "ford", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId4 = proxyBridge.Login(userId4, "alice3333@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(300);
                    Response resp = proxyBridge.AddItemToCart(loggedId4, storeId1, itemId1, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(loggedId4, transactionDetails, transactionDetailsSupply);
                }),
                Task.Run( () => {
                    Thread.Sleep(2000);
                    userId5 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    proxyBridge.Register(userId5, "ragnar3333@gmail.com", "ragnar", "lothbrok", "123AaC!@#");
                    Thread.Sleep(20);
                    loggedId5 = proxyBridge.Login(userId5, "ragnar3333@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);

                    Thread.Sleep(300);
                    Response resp = proxyBridge.AddItemToCart(loggedId5, storeId1, itemId1, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(loggedId5, transactionDetails, transactionDetailsSupply);

                }),
                Task.Run( () => {
                    userId6 = proxyBridge.Enter().Value;
                    proxyBridge.Register(userId6, "another3333@gmail.com", "another", "oneone", "123AaC!@#");
                     Thread.Sleep(600);
                    loggedId6 = proxyBridge.Login(userId6, "another3333@gmail.com", "123AaC!@#").Value;
                    Thread.Sleep(20);
                    storeId2 = proxyBridge.OpenNewStore(loggedId6, "VegtablesStore").Value;

                    itemId2 = proxyBridge.AddItemToStore(loggedId6, storeId2, "tomato", "vegtables", 3.99, 2).Value;
                    return proxyBridge.EditItem(loggedId6, storeId2, itemId2,"tomato", "vegtables", 3.99, 1);
                }),
                Task.Run( () => {
                    Thread.Sleep(1000);
                    userId7 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    Response resp = proxyBridge.AddItemToCart(userId7, storeId2, itemId2, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    Thread.Sleep(20);
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(userId7, transactionDetails, transactionDetailsSupply);

                }),
                Task.Run( () => {
                    Thread.Sleep(1000);
                    userId8 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    Response resp = proxyBridge.AddItemToCart(userId8, storeId2, itemId2, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    Thread.Sleep(20);
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(userId8, transactionDetails, transactionDetailsSupply);
                }),
                Task.Run( () => {
                    Thread.Sleep(1000);
                    userId9 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    Response resp = proxyBridge.AddItemToCart(userId9, storeId2, itemId2, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    Thread.Sleep(20);
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(userId9, transactionDetails, transactionDetailsSupply);
                }),
                Task.Run( () => {
                    Thread.Sleep(1000);
                    userId10 = proxyBridge.Enter().Value;
                    Thread.Sleep(20);
                    Response resp = proxyBridge.AddItemToCart(userId10, storeId2, itemId2, 1);
                    if(resp.ErrorOccured)
                        return resp;
                    Thread.Sleep(20);
                    SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
                    SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent","38 Tacher st.","Richmond","England","4284200");

                    return proxyBridge.PurchaseCart(userId10, transactionDetails, transactionDetailsSupply);

                })
            };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            // explanation:
            // 1 user register and then login then open store with item with amount 1
            // then 3 user attempt to add it to cart and then purchase it and 1 user attempt to add item with amount 2 to cart
            // while the store manager removes the item
            // another 1 user register and then login then open store with item with amount 2
            // then 4 guests attempt to purchase, each added 1 item to cart
            // while the store manager removes the item

            if (clientTasks[0].Result.ErrorOccured)
            {
                // first user failed to register and open store and add item
                Assert.IsTrue(clientTasks[0].Result.ErrorOccured); //error occurred
            }
            else
            {
                Assert.IsFalse(clientTasks[0].Result.ErrorOccured);//no error occurred

                Assert.IsFalse(clientTasks[1].Result.ErrorOccured);// no error occurred
                Assert.IsFalse(proxyBridge.GetDetailsOnCart(loggedId2).Value.Baskets.Count == 0); // successed to add item to cart


                // all 3 purchase attempts successed while store manager adding the item quantity
                int cntFailed = 0;
                int cntSuccess = 0;
                for (int i = 2; i < 5; i++)
                {
                    if (clientTasks[i].Result.ErrorOccured)
                        cntFailed++;
                    else
                        cntSuccess++;
                }
                Assert.IsTrue(cntSuccess.Equals(3));
                Assert.IsTrue(cntFailed.Equals(0));
                // items has more quantity beacuase store manager added
                Assert.IsFalse(proxyBridge.GetStore(storeId1).Value.itemsInventory.items_quantity[proxyBridge.GetStore(storeId1).Value.GetItemById(itemId1)].Equals(0));

                Assert.IsFalse(clientTasks[5].Result.ErrorOccured);//no error occurred

                //  3 purchases successed , 1 failed 
                cntFailed = 0;
                cntSuccess = 0;
                for (int i = 6; i < 10; i++)
                {
                    if (clientTasks[i].Result.ErrorOccured)
                        cntFailed++;
                    else
                        cntSuccess++;
                }
                Assert.IsTrue(cntFailed.Equals(1));
                Assert.IsTrue(cntSuccess.Equals(3));

                // remaining quantity of item after store manager added and all purchases is 0
                Assert.IsTrue(proxyBridge.GetStore(storeId2).Value.itemsInventory.items_quantity[proxyBridge.GetStore(storeId2).Value.GetItemById(itemId2)].Equals(0));

            }
        }
        #endregion


        [TestCleanup]
        public override void CleanUp()
        {
            DBHandler.Instance.CleanDB(); 
        }
    }

}