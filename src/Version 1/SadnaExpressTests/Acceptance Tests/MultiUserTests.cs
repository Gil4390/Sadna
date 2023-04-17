using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer;

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
            Guid id10 = Guid.Empty;
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
                    proxyBridge.GetUserShoppingCart(id5);
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
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id1).Value.Baskets.Count == 0);

            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id2).Value.Baskets.Count == 0);

            Assert.IsTrue(clientTasks[2].Result.ErrorOccured || clientTasks[3].Result.ErrorOccured); //no error occurred
            
            Assert.IsFalse(clientTasks[4].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id5).Value.Baskets.Count == 2);
            
            Assert.IsTrue(clientTasks[5].Result.ErrorOccured || clientTasks[6].Result.ErrorOccured); //no error occurred
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id6).Value.Baskets.Count == 1 || proxyBridge.GetUserShoppingCart(id7).Value.Baskets.Count == 1); //no error occurred
            
            Assert.IsFalse(clientTasks[7].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id8).Value.Baskets.Count == 1);
            
            Assert.IsFalse(clientTasks[8].Result.ErrorOccured);//no error occurred
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id9).Value.Baskets.Count == 1);
            Assert.IsTrue(proxyBridge.GetUserShoppingCart(id9).Value.GetItemQuantityInCart(storeid1,itemid11) == 3);
        }
        
        #endregion
        
    }
    
}