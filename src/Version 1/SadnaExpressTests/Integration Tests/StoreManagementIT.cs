using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;

using SadnaExpress.ServiceLayer;
using SadnaExpressTests.Unit_Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpressTests.Integration_Tests
{
    [TestClass]
    public class StoreManagementIT: TradingSystemIT
    {
        
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
        }

        /// <summary>
        /// Tests the successful opening of a new store by a user.
        /// Asserts that no error occurred during the process and that the new store was added to the list of stores.
        /// <summary>
        [TestMethod()]
        public void UserOpenStore_HappyTest()
        {
            //Arrange


            //Act
            ResponseT<Guid> res=trading.OpenNewStore(userID, "Jumbo");

            //Assert
            Assert.IsFalse(res.ErrorOccured);
            Assert.IsTrue(trading.GetAllStoreInfo().Value.FindAll((store) => store.StoreName == "Jumbo").Count == 1);
            Assert.IsTrue(trading.GetStore(res.Value).Value.Active);
        }

        /// <summary>
        /// Tests the case where a user tries to open a store with a name that already exists.
        /// Asserts that an error occurred during the process and that the existing store was not overwritten.
        /// <summary>
        [TestMethod()]
        public void UserOpenStoreNameExist_HappyTest()
        {
            //Arrange


            //Act
            ResponseT<Guid> res = trading.OpenNewStore(userID, "hello"); //already exist

            //Assert
            Assert.IsTrue(res.ErrorOccured);
            Assert.IsTrue(trading.GetAllStoreInfo().Value.FindAll((store) => store.StoreName == "hello").Count == 1);
        }

        /// <summary>
        /// Tests the UserCloseStore method when provided with valid input.
        /// Asserts that the store was successfully closed.
        /// <summary>
        [TestMethod()]
        public void UserCloseStore_BadTest()
        {
            //Arrange


            //Act
            Response res = trading.CloseStore(userID, storeID1); 

            //Assert
            Assert.IsFalse(res.ErrorOccured);
            Assert.IsTrue(trading.GetAllStoreInfo().Value.FindAll((store) => store.StoreID == storeID1).Count == 1);//store exists
            Assert.IsFalse(trading.GetStore(storeID1).Value.Active);
        }

        /// <summary>
        /// Tests the UserCloseStore method when provided with valid input.
        /// Asserts that the store items are not availble to search.
        /// <summary>
        [TestMethod()]
        public void UserCloseStoreItemsNotAppearInSearchByCategory_BadTest()
        {
            //Arrange


            //Act
            ResponseT<List<Item>> res1 = trading.GetItemsByKeysWord(userID, "ipad");

            Response res = trading.CloseStore(userID, storeID1);

            ResponseT<List<Item>> res2= trading.GetItemsByKeysWord(userID, "ipad");

            //Assert
            Assert.IsFalse(res.ErrorOccured);
            Assert.IsFalse(res1.ErrorOccured);
            Assert.IsFalse(res2.ErrorOccured);

            Assert.IsTrue(res1.Value.Count==2);
            Assert.IsTrue(res2.Value.Count == 1);
        }

        /// <summary>
        /// Tests the UserReopenStore method when provided with valid input.
        /// Asserts that the store was successfully reopened.
        /// <summary>
        [TestMethod()]
        public void UserReOpenStore_HappyTest()
        {
            //Arrange
            trading.GetStore(storeID1).Value.Active = false;

            //Act
            Response res = trading.ReopenStore(userID, storeID1);

            //Assert
            Assert.IsFalse(res.ErrorOccured);
            Assert.IsTrue(trading.GetStore(storeID1).Value.Active);//store active
           
        }

        /// <summary>
        /// Tests the WriteItemReview method when provided with valid input
        /// Asserts that the review was successfully added to the item.
        /// <summary>
        [TestMethod()]
        public void UserAddItemReview_HappyTest()
        {
            //Arrange
            Mocks.Mock_Orders mock_Orders = new Mocks.Mock_Orders();
            mock_Orders.AddOrderToUser(userID, new Order(new List<ItemForOrder> { new ItemForOrder(trading.GetItemByID(storeID1,itemID1).Value, storeID1) }));
            trading.SetTSOrders(mock_Orders);

            //Act
            Response res = trading.WriteItemReview(userID, storeID1, itemID1,"very nice product!! I like :)");

            //Assert
            Assert.IsFalse(res.ErrorOccured);
            Assert.AreEqual(1, trading.GetItemReviews(storeID1, itemID1).Value.Count);//store exists and item
        }

        /// <summary>
        /// Tests the WriteItemReview method when provided with invalid userID
        /// Asserts that the review wasn't added to the item.
        /// <summary>
        [TestMethod()]
        public void UserAddItemReviewNotPurchase_BadTest()
        {
            //Arrange
           

            //Act
            Response res = trading.WriteItemReview(userID, storeID1, itemID1, "very nice product!! I like :)");

            //Assert
            Assert.IsTrue(res.ErrorOccured);
            foreach (Review review in trading.GetItemReviews(storeID1, itemID1).Value)
            {
                Assert.AreNotEqual(review.ReviewerID, userID);
            }
        }

        /// <summary>
        /// Tests the WriteItemReview method when provided with valid input
        /// Asserts that both reviewes was successfully added to the item.
        /// <summary>
        [TestMethod()]
        public void UserAddFewItemsToReview_HappyTest()
        {
            //Arrange
            Mocks.Mock_Orders mock_Orders = new Mocks.Mock_Orders();
            mock_Orders.AddOrderToUser(userID, new Order(new List<ItemForOrder> { new ItemForOrder(trading.GetItemByID(storeID1, itemID1).Value, storeID1) }));
            trading.SetTSOrders(mock_Orders);

            //Act
            Response res = trading.WriteItemReview(userID, storeID1, itemID1, "very nice product!! I like :)");
            Response res1 = trading.WriteItemReview(userID, storeID1, itemID1, "nice product !! ");

            //Assert
            Assert.IsFalse(res.ErrorOccured);
            Assert.IsFalse(res1.ErrorOccured);
            Assert.AreEqual(2, trading.GetItemReviews(storeID1, itemID1).Value.Count);//store exists
        }

        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
        }
    }
}
