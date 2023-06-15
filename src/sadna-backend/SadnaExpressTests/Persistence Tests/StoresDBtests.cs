using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DataLayer;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.Store.Policy;
using SadnaExpress.ServiceLayer;
using SadnaExpress.ServiceLayer.SModels;
using SadnaExpressTests.Integration_Tests;

namespace SadnaExpressTests.Persistence_Tests
{
    [TestClass]
    public class StoresDBtests: TradingSystemPT
    {
        [TestInitialize]
        public override void Setup()
        {
            base.setTestMood();
            base.Setup();
            DatabaseContextFactory.TestMode = true;
        }
        
        [TestMethod()]
        public void DB_Open_Store_Success()
        {
            int numOfStores = DBHandler.Instance.GetAllStores().Count;
            trading.OpenNewStore(userID, "Jumbo_New_Store");
            
            Assert.IsTrue(DBHandler.Instance.IsStoreNameExist("Jumbo_New_Store"));
            Assert.AreEqual(DBHandler.Instance.GetAllStores().Count,numOfStores+1);
        }
        
        [TestMethod()]
        public void DB_Open_Store_Fail()
        {
            int numOfStores = DBHandler.Instance.GetAllStores().Count;
            trading.OpenNewStore(Guid.NewGuid(), "Store_That_will_not_open");
            
            Assert.IsFalse(DBHandler.Instance.IsStoreNameExist("Store_That_will_not_open"));
            Assert.AreEqual(DBHandler.Instance.GetAllStores().Count,numOfStores);
        }
        
        [TestMethod()]
        public void DB_Close_Store_Success()
        {
            int numOfStores = DBHandler.Instance.GetAllStores().Count;
            Guid storeid = trading.OpenNewStore(userID, "Jumbo_New_Store").Value;
            Assert.IsTrue(DBHandler.Instance.IsStoreNameExist("Jumbo_New_Store"));
            trading.CloseStore(userID, storeid);
            
            Assert.IsFalse(DBHandler.Instance.GetStoreById(storeid).Active);
            Assert.AreEqual(DBHandler.Instance.GetAllStores().Count, numOfStores+1);
        }
        
        [TestMethod()]
        public void DB_Close_Store_Fail()
        {
            int numOfStores = DBHandler.Instance.GetAllStores().Count;
            trading.OpenNewStore(userID, "Store_That_will_not_close");
            Assert.IsTrue(DBHandler.Instance.IsStoreNameExist("Store_That_will_not_close"));
            trading.CloseStore(userID, new Guid());
            
            Assert.IsTrue(DBHandler.Instance.IsStoreNameExist("Store_That_will_not_close"));
            Assert.AreEqual(DBHandler.Instance.GetAllStores().Count,numOfStores+1);
        }
        
        [TestMethod()]
        public void DB_Add_Item_Store_Success()
        {
            Guid itemID3 = trading.AddItemToStore(userID, storeID1, "Bike", "Vehicles", 1000, 3).Value;
            Guid itemID4 = trading.AddItemToStore(userID, storeID2, "Car", "Vehicles", 1000, 4).Value;

            Store store1 = DBHandler.Instance.GetStoreById(storeID1);
            Inventory inv1 = store1.itemsInventory;

            Store store2 = DBHandler.Instance.GetStoreById(storeID2);
            Inventory inv2 = store2.itemsInventory;

            Assert.AreEqual(3, inv1.items_quantity[store1.GetItemById(itemID3)]);
            Assert.ThrowsException<Exception>(()=>DBHandler.Instance.GetStoreById(storeID1).GetItemById(itemID4));
            Assert.AreEqual(4, inv2.items_quantity[store2.GetItemById(itemID4)]);
        }
        
        [TestMethod()]
        public void DB_Add_Item_Store_Fail()
        {
            Guid itemID3 = trading.AddItemToStore(userID, storeID1, "Bike", "Vehicles", 1000, -1).Value;
            Assert.ThrowsException<Exception>(() => DBHandler.Instance.GetStoreById(storeID1).GetItemById(itemID3));
        }
        
        [TestMethod()]
        public void DB_Remove_Item_Store_Success()
        {
            Guid itemID3 = trading.AddItemToStore(userID, storeID1, "Bike", "Vehicles", 1000, 3).Value;

            Store store1 = DBHandler.Instance.GetStoreById(storeID1);
            Inventory inv1 = store1.itemsInventory;

            Assert.IsNotNull(store1.GetItemById(itemID3));
            Assert.AreEqual(3, inv1.items_quantity[store1.GetItemById(itemID3)]);

            trading.RemoveItemFromStore(userID, storeID1, itemID3);
            Assert.ThrowsException<Exception>(()=> DBHandler.Instance.GetStoreById(storeID1).GetItemById(itemID3));
        }
        
        [TestMethod()]
        public void DB_Remove_Item_Store_Fail()
        {
            Guid itemID3 = trading.AddItemToStore(userID, storeID1, "Bike", "Vehicles", 1000, 3).Value;

            Store store1 = DBHandler.Instance.GetStoreById(storeID1);
            Inventory inv1 = store1.itemsInventory;

            Assert.IsNotNull(store1.GetItemById(itemID3));
            Assert.AreEqual(3, inv1.items_quantity[store1.GetItemById(itemID3)]);

            trading.RemoveItemFromStore(userID, storeID2, itemID3);
            Assert.IsNotNull(DBHandler.Instance.GetStoreById(storeID1).GetItemById(itemID3));
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
            
            Assert.IsTrue(res.ErrorOccured);
            Assert.IsFalse(res1.ErrorOccured);
            Assert.AreNotEqual(2, trading.GetItemReviews(itemID1).Value.Count);
            Assert.AreEqual(1, trading.GetItemReviews(itemID1).Value.Count);
        }
        
        [TestMethod()]
        public void DB_Conditions_Add_Success()
        {
            DBHandler.Instance.CleanDB();
            Condition cond1 = trading.GetStore(storeID1).Value.AddCondition("Item", "ipad 32", "min quantity", 2, DateTime.MaxValue);
            Assert.AreNotEqual(DBHandler.Instance.GetCond(cond1.ID,storeID1),-1);
        }
        
        [TestMethod()]
        public void DB_Conditions_Remove_Success()
        {
            DBHandler.Instance.CleanDB();
            Condition cond1 = trading.GetStore(storeID1).Value.AddCondition("Item", "ipad 32", "min quantity", 2, DateTime.MaxValue);
            DBHandler.Instance.RemoveCond(cond1.ID, trading.GetStore(storeID1).Value);
            Assert.AreEqual(DBHandler.Instance.GetCond(cond1.ID,storeID1),-1);
        }
        
        [TestMethod()]
        public void DB_Conditions_Remove_Fail()
        {
            DBHandler.Instance.CleanDB();
            Condition cond1 = trading.GetStore(storeID1).Value.AddCondition("Item", "ipad 32", "min quantity", 2, DateTime.MaxValue);
            DBHandler.Instance.RemoveCond(cond1.ID, trading.GetStore(storeID1).Value);
            Assert.AreEqual(-1, DBHandler.Instance.GetCond(cond1.ID,storeID1));
        }

        [TestMethod()]
        public void DB_Purchase_Success()
        {
            trading.SetPaymentService(new Mocks.Mock_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_SupplierService());

            trading.AddItemToCart(buyerMemberID, storeID1, itemID1, 1);

            int numBefore = trading.GetStore(storeID1).Value.GetItemByQuantity(itemID1);
            SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
            SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent", "38 Tacher st.", "Richmond", "England", "4284200");

            trading.PurchaseCart(buyerMemberID, transactionDetails, transactionDetailsSupply);

            Assert.AreEqual(DBHandler.Instance.GetAllOrders().Count, 1);
            Assert.AreEqual(0, trading.GetDetailsOnCart(buyerMemberID).Value.Baskets.Count); // the shopping basket get empty
            Assert.AreEqual(numBefore - 3, trading.GetStore(storeID1).Value.GetItemByQuantity(itemID1)); //the quantity updated
        }

        [TestMethod()]
        public void DB_Purchase_Fail()
        {
            trading.SetPaymentService(new Mocks.Mock_Bad_PaymentService());
            trading.SetSupplierService(new Mocks.Mock_SupplierService());

            trading.AddItemToCart(buyerMemberID, storeID1, itemID1, 1);

            int numBefore = trading.GetStore(storeID1).Value.GetItemByQuantity(itemID1);
            int numofbasket = trading.GetDetailsOnCart(buyerMemberID).Value.Baskets.Count;
            SPaymentDetails transactionDetails = new SPaymentDetails("1122334455667788", "12", "27", "Tal Galmor", "444", "123456789");
            SSupplyDetails transactionDetailsSupply = new SSupplyDetails("Roy Kent", "38 Tacher st.", "Richmond", "England", "4284200");

            trading.PurchaseCart(buyerMemberID, transactionDetails, transactionDetailsSupply);

            Assert.AreEqual(DBHandler.Instance.GetAllOrders().Count, 0);
            Assert.AreEqual(numofbasket, trading.GetDetailsOnCart(buyerMemberID).Value.Baskets.Count); // the shopping basket still full
            Assert.AreEqual(numBefore, trading.GetStore(storeID1).Value.GetItemByQuantity(itemID1)); //the quantity not updated
        }

    }
}
