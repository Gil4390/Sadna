using SadnaExpress.API.SignalR;
using SadnaExpress.DomainLayer.User;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.Store.Policy;
using SadnaExpress.DomainLayer;
using SadnaExpress.DataLayer;

namespace SadnaExpressTests.Unit_Tests
{
    [TestClass()]
    public class BidsUT
    {
        private Guid store;
        private Guid item1;
        private Guid item2;
        private Guid item3;
        private Member member;
        private Bid bid;
        private PromotedMember founder;

        #region SetUp

        [TestInitialize]
        public void SetUp()
        {
            DatabaseContextFactory.TestMode = true;
            DBHandler.Instance.CleanDB();
            store = Guid.NewGuid();
            item1 = Guid.NewGuid();
            item2 = Guid.NewGuid();
            item3 = Guid.NewGuid();
            Guid userID = Guid.NewGuid();
            member = new Member(userID, "tal@gmail.com", "Tal", "Galmor", "w3ka!Tal");
            UserFacade userFacade = new UserFacade();
            userFacade.members.TryAdd(userID, member);

            NotificationNotifier.GetInstance().TestMood = true;
            NotificationSystem.Instance.userFacade = userFacade;

            founder = new PromotedMember(Guid.NewGuid(), "AsiAzar@gmail.com", "Asi", "Azar",
                ("A#!a12345678"));
            userFacade.members.TryAdd(founder.UserId, founder);
            founder.createFounder(Guid.NewGuid());
            bid = member.PlaceBid(store, item2, "Apple", 5, new List<PromotedMember> { founder });
        }

        #endregion

        #region tests

        [TestMethod]
        public void PlaceBidFounderNotLoginSuccess()
        {
            //Arrange
            User user1 = new User();
            //Act
            user1.PlaceBid(store, item1, "Banana", 8, new List<PromotedMember> { founder });
            //Assert
            Assert.AreEqual(1, user1.GetBidsOfUser().Count);
            Assert.AreEqual(8, user1.GetBidsOfUser()[item1].Key);
            Assert.AreEqual(2, founder.AwaitingNotification.Count);
        }

        [TestMethod]
        public void ReactToBidSuccess()
        {
            //Act
            founder.ReactToBid(store, bid.BidId, "approved");
            //Assert
            Assert.AreEqual(1, member.GetBidsOfUser().Count);
            Assert.AreEqual(1, member.AwaitingNotification.Count);
        }
        
        [TestMethod]
        public void GetBidsInStoreSuccess()
        {
            //Act
            List<Bid> bids = founder.GetBidsInStore(store);
            //Assert
            Assert.AreEqual(1, bids.Count);
            Assert.AreEqual(5, bids[0].Price);
            Assert.AreEqual(false, bids[0].Approved());
        }
        
        [TestMethod]
        public void GetBidsInStoreFail()
        {
            //Act
            Assert.ThrowsException<Exception>(()=> founder.GetBidsInStore(Guid.NewGuid()));
        }
        
        [TestMethod]
        public void GetBidsAfterApprovedSuccess()
        {
            //Arrange
            founder.ReactToBid(store, bid.BidId, "approved");
            //Act
            List<Bid> bids = founder.GetBidsInStore(store);
            //Assert
            Assert.IsTrue(bids[0].Approved());
            Assert.AreEqual(1, member.AwaitingNotification.Count);
            Assert.AreEqual("Your offer on Apple accepted! The price changed to 5", member.AwaitingNotification[0].Message);
        }
        
        [TestMethod]
        public void GetBidsAfterOfferNewPriceSuccess()
        {
            //Arrange
            founder.ReactToBid(store, bid.BidId, "6");
            //Act
            List<Bid> bids = founder.GetBidsInStore(store);
            //Assert
            Assert.IsTrue(bids[0].Approved());
            Assert.AreEqual(1, member.AwaitingNotification.Count);
            Assert.AreEqual("Your offer on Apple wasn't approved. You get counter offer of this amount 6", member.AwaitingNotification[0].Message);
        }
        
        [TestMethod]
        public void GetBidsAfterOfferNewDoublePriceSuccess()
        {
            //Arrange
            founder.ReactToBid(store, bid.BidId, "6.5");
            //Act
            List<Bid> bids = founder.GetBidsInStore(store);
            //Assert
            Assert.IsTrue(bids[0].Approved());
            Assert.AreEqual(1, member.AwaitingNotification.Count);
            Assert.AreEqual("Your offer on Apple wasn't approved. You get counter offer of this amount 6.5", member.AwaitingNotification[0].Message);
        }
        
        [TestMethod]
        public void GetBidsAfterDenySuccess()
        {
            //Arrange
            founder.ReactToBid(store, bid.BidId, "denied");
            //Act
            List<Bid> bids = founder.GetBidsInStore(store);
            //Assert
            Assert.AreEqual(0, bids.Count);
            Assert.AreEqual("Your offer on Apple denied!", member.AwaitingNotification[0].Message);
        }
        #endregion

        #region Clean Up

        [TestCleanup]
        public void CleanUp()
        {
            member.Bids = new List<Bid>();
        }

        #endregion

    }
}
