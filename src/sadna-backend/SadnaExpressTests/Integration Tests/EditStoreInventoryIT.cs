using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer;
using SadnaExpressTests.Acceptance_Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaExpress.ServiceLayer.Obj;
using SadnaExpress.DataLayer;

namespace SadnaExpressTests.Integration_Tests
{
    [TestClass]
    public class EditStoreInventoryIT: TradingSystemIT
    {
        Guid guestID;
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            //create guest
            guestID = trading.Enter().Value;
        }

        /// <summary>
        /// Tests the UserAddItemToStore method with valid input parameters.
        /// </summary>
        [TestMethod()]
        public void UserAddItemToStore_HappyTest()
        {
            //Arrange


            //Act
            ResponseT<Guid> res = trading.AddItemToStore(userID, storeID1, "head phones", "electronicy", 175, 56);

            //Assert
            Assert.IsFalse(res.ErrorOccured);
            Assert.IsTrue(trading.GetStore(storeID1).Value.itemsInventory.ItemExist(res.Value));
          
        }

        /// <summary>
        /// Tests the UserAddItemToStore and the search functionality with valid input parameters.
        /// </summary>
        [TestMethod()]
        public void UserAddItemToStoreAndFindBySearch_HappyTest()
        {
            //Arrange


            //Act
            ResponseT<Guid> res = trading.AddItemToStore(userID, storeID1,"head phones", "electronicy", 175, 56);
            ResponseT<List<SItem>> res1 = trading.GetItemsForClient(guestID, "",category:"electronicy");
            ResponseT<List<SItem>> res2 = trading.GetItemsForClient(guestID, "head phones");
            ResponseT<List<SItem>> res3 = trading.GetItemsForClient(guestID, "head");

            //Assert
            Assert.IsFalse(res.ErrorOccured);
            Assert.IsFalse(res1.ErrorOccured);
            Assert.IsFalse(res2.ErrorOccured);
            Assert.IsFalse(res3.ErrorOccured);
            Assert.IsTrue(res1.Value.Count == 1);
            Assert.IsTrue(res2.Value.Count == 1);
            Assert.IsTrue(res3.Value.Count == 1);
        }

        /// <summary>
        /// Tests the UserRemoveItemFromStore method with valid input parameters.
        /// </summary>
        [TestMethod()]
        public void UserRemoveItemFromStore_HappyTest()
        {
            //Arrange


            //Act
            Response res = trading.RemoveItemFromStore(userID, storeID1, itemID1);

            //Assert
            Assert.IsFalse(res.ErrorOccured);
            Assert.IsFalse(trading.GetStore(storeID1).Value.itemsInventory.ItemExist(itemID1));

        }

        /// <summary>
        /// Tests the UserRemoveItemFromStore method with invalid user ID.
        /// </summary>
        [TestMethod()]
        public void UserRemoveItemFromStoreNoPremissions_BadTest()
        {
            //Arrange


            //Act
            Response res = trading.RemoveItemFromStore(Guid.NewGuid(), storeID1, itemID1);

            //Assert
            Assert.IsTrue(res.ErrorOccured);
            Assert.IsTrue(trading.GetStore(storeID1).Value.itemsInventory.ItemExist(itemID1));

        }

        /// <summary>
        /// Tests the UserEditItemsInStore method with valid input parameters.
        /// </summary>
        [TestMethod()]
        public void UserEditItemsInStore_HappyTest()
        {
            //Arrange


            //Act
            Response res = trading.EditItemQuantity(userID, storeID1, itemID1,187);

            //Assert
            Assert.IsFalse(res.ErrorOccured);
            Assert.IsTrue(trading.GetStore(storeID1).Value.itemsInventory.ItemExist(itemID1));
            Assert.IsTrue(trading.GetStore(storeID1).Value.itemsInventory.GetItemByQuantity(itemID1)==190);
        }

        /// <summary>
        /// Tests the UserEditItemsInStore method with ivalid userID.
        /// </summary>
        [TestMethod()]
        public void UserEditItemsInStoreNoPremissions_BadTest()
        {
            //Arrange
            Guid id = trading.Enter().Value;

            //Act
            Response res = trading.EditItemQuantity(id, storeID1, itemID1, 187);

            //Assert
            Assert.IsTrue(res.ErrorOccured);
            Assert.IsTrue(trading.GetStore(storeID1).Value.itemsInventory.ItemExist(itemID1));
            Assert.IsTrue(trading.GetStore(storeID1).Value.itemsInventory.GetItemByQuantity(itemID1) == 3);
        }

        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
            DBHandler.Instance.TestMode();
        }
    }
}
