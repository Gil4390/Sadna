using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DataLayer;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.Store.Policy;
using SadnaExpress.ServiceLayer;
using SadnaExpressTests.Integration_Tests;

namespace SadnaExpressTests.Persistence_Tests
{
    [TestClass]
    public class StoresDBtests: TradingSystemIT
    {
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            DatabaseContextFactory.TestMode = true;
        }
        
        [TestMethod()]
        public void DB_Open_Store_Success()
        {
            int numOfStores = DBHandler.Instance.GetAllStores().Count;
            trading.OpenNewStore(userID, "Jumbo_New_Store");
            
            Assert.IsTrue(DBHandler.Instance.IsStoreNameExist("Jumbo_New_Store"));
            Assert.Equals(DBHandler.Instance.GetAllStores().Count,numOfStores+1);
        }
        
        [TestMethod()]
        public void DB_Open_Store_Fail()
        {
            int numOfStores = DBHandler.Instance.GetAllStores().Count;
            trading.OpenNewStore(new Guid(), "Store_That_will_not_open");
            
            Assert.IsFalse(DBHandler.Instance.IsStoreNameExist("Store_That_will_not_open"));
            Assert.Equals(DBHandler.Instance.GetAllStores().Count,numOfStores);
        }
        
        [TestMethod()]
        public void DB_Close_Store_Success()
        {
            int numOfStores = DBHandler.Instance.GetAllStores().Count;
            Guid storeid = trading.OpenNewStore(userID, "Jumbo_New_Store").Value;
            Assert.IsTrue(DBHandler.Instance.IsStoreNameExist("Jumbo_New_Store"));
            trading.CloseStore(userID, storeid);
            
            Assert.IsFalse(DBHandler.Instance.IsStoreNameExist("Jumbo_New_Store"));
            Assert.Equals(DBHandler.Instance.GetAllStores().Count,numOfStores);
        }
        
        [TestMethod()]
        public void DB_Close_Store_Fail()
        {
            int numOfStores = DBHandler.Instance.GetAllStores().Count;
            trading.OpenNewStore(userID, "Store_That_will_not_close");
            Assert.IsTrue(DBHandler.Instance.IsStoreNameExist("Store_That_will_not_close"));
            trading.CloseStore(userID, new Guid());
            
            Assert.IsTrue(DBHandler.Instance.IsStoreNameExist("Store_That_will_not_close"));
            Assert.Equals(DBHandler.Instance.GetAllStores().Count,numOfStores+1);
        }
        
        [TestMethod()]
        public void DB_Add_Item_Store_Success()
        {
            Guid itemID3 = trading.AddItemToStore(userID, storeID1, "Bike", "Vehicles", 1000, 3).Value;
            Guid itemID4 = trading.AddItemToStore(userID, storeID2, "Car", "Vehicles", 1000, 4).Value;
            
            Assert.IsTrue(DBHandler.Instance.GetStoreById(storeID1).itemsInventory.items_quantity[DBHandler.Instance.GetStoreById(storeID1).GetItemById(itemID3)].Equals(3));
            Assert.IsFalse(DBHandler.Instance.GetStoreById(storeID1).itemsInventory.items_quantity[DBHandler.Instance.GetStoreById(storeID1).GetItemById(itemID4)].Equals(3));
            Assert.IsTrue(DBHandler.Instance.GetStoreById(storeID2).itemsInventory.items_quantity[DBHandler.Instance.GetStoreById(storeID1).GetItemById(itemID4)].Equals(4));
        }
        
        [TestMethod()]
        public void DB_Add_Item_Store_Fail()
        {
            Guid itemID3 = trading.AddItemToStore(userID, storeID1, "Bike", "Vehicles", 1000, -1).Value;
            Assert.IsNull(DBHandler.Instance.GetStoreById(storeID1).GetItemById(itemID3));
        }
        
        [TestMethod()]
        public void DB_Remove_Item_Store_Success()
        {
            Guid itemID3 = trading.AddItemToStore(userID, storeID1, "Bike", "Vehicles", 1000, 3).Value;
            Assert.IsNotNull(DBHandler.Instance.GetStoreById(storeID1).GetItemById(itemID3));
            Assert.IsTrue(DBHandler.Instance.GetStoreById(storeID1).itemsInventory.items_quantity[DBHandler.Instance.GetStoreById(storeID1).GetItemById(itemID3)].Equals(3));

            trading.RemoveItemFromStore(userID, storeID1, itemID3);
            Assert.IsNull(DBHandler.Instance.GetStoreById(storeID1).GetItemById(itemID3));
        }
        
        [TestMethod()]
        public void DB_Remove_Item_Store_Fail()
        {
            Guid itemID3 = trading.AddItemToStore(userID, storeID1, "Bike", "Vehicles", 1000, 3).Value;
            Assert.IsNotNull(DBHandler.Instance.GetStoreById(storeID1).GetItemById(itemID3));
            Assert.IsTrue(DBHandler.Instance.GetStoreById(storeID1).itemsInventory.items_quantity[DBHandler.Instance.GetStoreById(storeID1).GetItemById(itemID3)].Equals(3));

            trading.RemoveItemFromStore(userID, storeID1, new Guid());
            Assert.IsTrue(DBHandler.Instance.GetStoreById(storeID1).itemsInventory.items_quantity[DBHandler.Instance.GetStoreById(storeID1).GetItemById(itemID3)].Equals(3));

            trading.RemoveItemFromStore(userID, storeID2, itemID3);
            Assert.IsTrue(DBHandler.Instance.GetStoreById(storeID1).itemsInventory.items_quantity[DBHandler.Instance.GetStoreById(storeID1).GetItemById(itemID3)].Equals(3));
        }
        
        [TestMethod()]
        public void DB_Add_Review_Success()
        {
            Mocks.Mock_Orders mock_Orders = new Mocks.Mock_Orders();
            mock_Orders.AddOrderToUser(userID, new Order(new List<ItemForOrder> { new ItemForOrder(trading.GetItemByID(storeID1, itemID1).Value, storeID1, "RotemSela@gmail.com" ,"Zara") }, userID));
            trading.SetTSOrders(mock_Orders);

            //Act
            Response res = trading.WriteItemReview(userID,  itemID1, "very nice product!! I like :) #########");
            Response res1 = trading.WriteItemReview(userID,  itemID1, "nice product !! &&*&*&");
            

            List<Guid> reviewsIds = DBHandler.Instance.GetTSReviewsIds();
            int reviewsCount = 0;
            foreach (Guid reviewId in reviewsIds)
            {
                if (DBHandler.Instance.GetReviewById(reviewId).ReviewText ==
                    "very nice product!! I like :) #########" ||
                    DBHandler.Instance.GetReviewById(reviewId).ReviewText == "nice product !! &&*&*&")
                    reviewsCount++;
            }
            
            Assert.IsFalse(res.ErrorOccured);
            Assert.IsFalse(res1.ErrorOccured);
            Assert.AreEqual(2, trading.GetItemReviews(itemID1).Value.Count);
        }
        
        [TestMethod()]
        public void DB_Add_Review_Fail()
        {
            Mocks.Mock_Orders mock_Orders = new Mocks.Mock_Orders();
            mock_Orders.AddOrderToUser(userID, new Order(new List<ItemForOrder> { new ItemForOrder(trading.GetItemByID(storeID1, itemID1).Value, storeID1, "RotemSela@gmail.com" ,"Zara") }, userID));
            trading.SetTSOrders(mock_Orders);

            //Act
            Response res = trading.WriteItemReview(new Guid(),  itemID1, "very nice product!! I like :) #########");
            Response res1 = trading.WriteItemReview(userID,  itemID2, "nice product !! &&*&*&");
            

            List<Guid> reviewsIds = DBHandler.Instance.GetTSReviewsIds();
            int reviewsCount = 0;
            foreach (Guid reviewId in reviewsIds)
            {
                if (DBHandler.Instance.GetReviewById(reviewId).ReviewText ==
                    "very nice product!! I like :) #########" ||
                    DBHandler.Instance.GetReviewById(reviewId).ReviewText == "nice product !! &&*&*&")
                    reviewsCount++;
            }
            
            Assert.IsFalse(res.ErrorOccured);
            Assert.IsFalse(res1.ErrorOccured);
            Assert.AreNotEqual(2, trading.GetItemReviews(itemID1).Value.Count);
            Assert.AreEqual(0, trading.GetItemReviews(itemID1).Value.Count);
        }
        
        [TestMethod()]
        public void DB_Conditions_Add_Success()
        {
            DBHandler.Instance.CleanDB();
            Condition cond1 = trading.GetStore(storeID1).Value.AddCondition("Item","Bisli", "min quantity", 2, DateTime.MaxValue);
            Assert.AreNotEqual(DBHandler.Instance.GetCond(cond1.ID,storeID1),-1);
        }
        
        [TestMethod()]
        public void DB_Conditions_Add_Fail()
        {
            DBHandler.Instance.CleanDB();
            Condition cond1 = trading.GetStore(storeID1).Value.AddCondition("Category","Animal", "min quality", 2, DateTime.MaxValue);
            Assert.AreEqual(DBHandler.Instance.GetCond(cond1.ID,storeID1),-1);
        }
        
        [TestMethod()]
        public void DB_Conditions_Remove_Success()
        {
            DBHandler.Instance.CleanDB();
            Condition cond1 = trading.GetStore(storeID1).Value.AddCondition("Item","Bisli", "min quantity", 2, DateTime.MaxValue);
            DBHandler.Instance.RemoveCond(cond1.ID,storeID1);
            Assert.AreEqual(DBHandler.Instance.GetCond(cond1.ID,storeID1),-1);
        }
        
        [TestMethod()]
        public void DB_Conditions_Remove_Fail()
        {
            DBHandler.Instance.CleanDB();
            Condition cond1 = trading.GetStore(storeID1).Value.AddCondition("Item","Bisli", "min quantity", 2, DateTime.MaxValue);
            DBHandler.Instance.RemoveCond(cond1.ID,storeID2);
            Assert.AreEqual(DBHandler.Instance.GetCond(cond1.ID,storeID1),cond1.ID);
        }
        
    }
}
