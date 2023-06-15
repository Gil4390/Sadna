using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DataLayer;

namespace SadnaExpressTests.Unit_Tests
{
    [TestClass()]
    public class ShoppingCartUT: TradingSystemUT
    {
        private ShoppingCart shoppingCart;
        private ShoppingBasket shoppingBasket1;
        private Guid itemID;
        private Guid storeID;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            itemID = Guid.NewGuid();
            storeID = Guid.NewGuid();
            shoppingCart = new ShoppingCart();
            shoppingBasket1 = new ShoppingBasket(storeID);
        }


        [TestMethod()]
        public void AddItemToShoppingCartSuccess()
        {
            //Act
            shoppingCart.AddItemToCart(storeID, itemID, 5);
            //Assert
            Assert.AreEqual(1, shoppingCart.Baskets.Count);
            Assert.AreEqual(5,shoppingCart.GetItemQuantityInCart(storeID, itemID));
        }

        [TestMethod()]
        public void AddMoreItemFromTheSameStoreSuccess()
        {
            //arrange
            Guid itemID2 = Guid.NewGuid();
            shoppingCart.AddItemToCart(storeID, itemID2, 2);
            //Act
            shoppingCart.AddItemToCart(storeID, itemID, 5);
            //Assert
            Assert.AreEqual(1, shoppingCart.Baskets.Count);
            Assert.AreEqual(5,shoppingCart.GetItemQuantityInCart(storeID, itemID));
            Assert.AreEqual(2,shoppingCart.GetItemQuantityInCart(storeID, itemID2));
        }
        [TestMethod()]
        public void AddMoreItemFromDiffStoreSuccess()
        {
            //arrange
            Guid newStoreID = Guid.NewGuid();
            shoppingCart.AddItemToCart(newStoreID, itemID, 2);
            //Act
            shoppingCart.AddItemToCart(storeID, itemID, 5);
            //Assert
            Assert.AreEqual(2, shoppingCart.Baskets.Count);
            Assert.AreEqual(5,shoppingCart.GetItemQuantityInCart(storeID, itemID));
            Assert.AreEqual(2,shoppingCart.GetItemQuantityInCart(newStoreID, itemID));
        }
        
        [TestMethod()]
        public void RemoveTheItemUntilBasketEmpty()
        {
            //arrange
            shoppingCart.AddItemToCart(storeID, itemID, 2);
            //Act
            shoppingCart.RemoveItemFromCart(storeID,itemID);
            //Assert
            Assert.AreEqual(0, shoppingCart.Baskets.Count);
        }
        [TestMethod()]
        public void RemoveTheItemThereStillBasket()
        {
            //arrange
            Guid itemID2 = Guid.NewGuid();
            shoppingCart.AddItemToCart(storeID, itemID, 2);
            shoppingCart.AddItemToCart(storeID, itemID2, 2);
            //Act
            shoppingCart.RemoveItemFromCart(storeID,itemID);
            //Assert
            Assert.AreEqual(1, shoppingCart.Baskets.Count);
            Assert.ThrowsException<Exception>(()=>shoppingCart.GetItemQuantityInCart(storeID, itemID));
            Assert.AreEqual(2,shoppingCart.GetItemQuantityInCart(storeID, itemID2));
        }
        [TestMethod()]
        public void RemoveBasketNotExist()
        {
            //arrange
            Guid itemID2 = Guid.NewGuid();
            shoppingCart.AddItemToCart(storeID, itemID, 2);
            shoppingCart.AddItemToCart(storeID, itemID2, 2);
            //Act
            Assert.ThrowsException<Exception>(() => shoppingCart.RemoveItemFromCart(new Guid(),itemID));
        }
        
        [TestMethod()]
        public void EditBasketNotExist()
        {
            //arrange
            Guid itemID2 = Guid.NewGuid();
            shoppingCart.AddItemToCart(storeID, itemID, 2);
            shoppingCart.AddItemToCart(storeID, itemID2, 2);
            //Act
            Assert.ThrowsException<Exception>(() => shoppingCart.EditItemFromCart(new Guid(),itemID, 5));
        }
        [TestMethod()]
        public void EditBasketItemNotExist()
        {
            //arrange
            shoppingCart.AddItemToCart(storeID, itemID, 2);
            //Act
            Assert.ThrowsException<Exception>(() => shoppingCart.EditItemFromCart(storeID,new Guid(), 5));        
        }
        
        [TestMethod()]
        public void EditBasketSuccess()
        {
            //arrange
            shoppingCart.AddItemToCart(storeID, itemID, 2);
            //Act
            shoppingCart.EditItemFromCart(storeID, itemID, 5);
            //Assert
            Assert.AreEqual(1, shoppingCart.Baskets.Count);
            Assert.AreEqual(5,shoppingCart.GetItemQuantityInCart(storeID, itemID));
        }
        

        [TestCleanup]
        public void CleanUp()
        {

        }

    }
}
