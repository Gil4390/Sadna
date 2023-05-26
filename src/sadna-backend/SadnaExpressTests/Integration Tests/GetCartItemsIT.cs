using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store.Policy;
using SadnaExpress.ServiceLayer.Obj;

namespace SadnaExpressTests.Integration_Tests
{
    
    [TestClass]
    public class GetCartItemsIT: TradingSystemIT
    {
        protected Guid itemID3;
        protected Guid itemID4;
        
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            itemID3 = trading.AddItemToStore(userID, storeID1, "bamba shosh", "Food", 10, 3).Value;
            itemID4 = trading.AddItemToStore(userID, storeID2, "bisli", "Food", 10, 1).Value;
            trading.AddItemToCart(buyerID, storeID1, itemID3, 2);
            trading.AddItemToCart(buyerID, storeID2, itemID4, 1);
        }
        
        [TestMethod()]
        public void UserGetCartWithSimpleDiscountTest()
        {
            //Arrange
            DiscountPolicy policy1 =trading.CreateSimplePolicy(userID,storeID1, "Itemipad 32", 10,
                DateTime.Now, new DateTime(2024, 05, 22)).Value;
            trading.AddPolicy(userID,storeID1, policy1.ID);
            //Act
            List<SItem> items = trading.GetCartItems(buyerID).Value;
            //Assert
            Assert.AreEqual(4, items.Count);
            bool found = false;
            foreach (SItem sItem in items)
            {
                if (sItem.ItemId.Equals(itemID1.ToString()))
                {
                    Assert.AreEqual(3600, sItem.PriceDiscount);
                    found = true;
                }
            }
            Assert.IsTrue(found);
        }
        
        [TestMethod()]
        public void UserGetCartWithDiscountComplexTest()
        {
            //Arrange
            DiscountPolicy policy1 =trading.CreateSimplePolicy(userID,storeID1, "Itemipad 32", 10,
                DateTime.Now, new DateTime(2024, 05, 22)).Value;
            DiscountPolicy policy2 =trading.CreateSimplePolicy(userID,storeID1, "Store", 20,
                DateTime.Now, new DateTime(2024, 05, 22)).Value;
            DiscountPolicy addPolicy = trading.CreateComplexPolicy(userID,storeID1, "add", policy1.ID, policy2.ID).Value;
            trading.AddPolicy(userID,storeID1, addPolicy.ID);
            //Act
            List<SItem> items = trading.GetCartItems(buyerID).Value;
            //Assert
            bool found1 = false;
            bool found2 = false;
            foreach (SItem sItem in items)
            {
                if (sItem.ItemId.Equals(itemID1.ToString()))
                {
                    Assert.AreEqual(2800, sItem.PriceDiscount);
                    found1 = true;
                }
                if (sItem.ItemId.Equals(itemID3.ToString()))
                {
                    Assert.AreEqual(8, sItem.PriceDiscount);
                    found2 = true;
                }
            }
            Assert.IsTrue(found1 && found2);
        }

        [TestMethod()]
        public void UserGetItemWithDiscountComplexTest()
        {
            //Arrange
            DiscountPolicy policy1 =trading.CreateSimplePolicy(userID,storeID1, "Itemipad 32", 10,
                DateTime.Now, new DateTime(2024, 05, 22)).Value;
            DiscountPolicy policy2 =trading.CreateSimplePolicy(userID,storeID1, "Store", 20,
                DateTime.Now, new DateTime(2024, 05, 22)).Value;
            DiscountPolicy addPolicy = trading.CreateComplexPolicy(userID,storeID1, "add", policy1.ID, policy2.ID).Value;
            trading.AddPolicy(userID,storeID1, addPolicy.ID);
            //Act
            List<SItem> items = trading.GetItemsForClient(buyerID, "ipad").Value;
            //Assert
            bool found1 = false;
            bool found2 = false;
            foreach (SItem sItem in items)
            {
                if (sItem.ItemId.Equals(itemID1.ToString()))
                {
                    Assert.AreEqual(2800, sItem.PriceDiscount);
                    found1 = true;
                }
                if (sItem.ItemId.Equals(itemID2.ToString()))
                {
                    Assert.AreEqual(-1, sItem.PriceDiscount);
                    found2 = true;
                }
            }
            Assert.IsTrue(found1 && found2);
        }

        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
        }        
    }
}