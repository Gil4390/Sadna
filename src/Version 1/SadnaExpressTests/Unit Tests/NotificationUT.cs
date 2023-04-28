using System;
using SadnaExpress.DomainLayer;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpressTests.Unit_Tests
{
    [TestClass()]
    public class NotificationUT
    {
        private NotificationSystem notificationSystem;
        private Guid storeID;
        private Guid userID1;
        private Member member1;
        private Guid userID2;
        private Member member2;

        #region SetUp
        [TestInitialize]
        public void SetUp()
        {
            notificationSystem = new NotificationSystem();
            storeID = Guid.NewGuid();
            userID1 = Guid.NewGuid();
            userID2 = Guid.NewGuid();
            member1 = new Member(userID1, "Dina@gmail.com", "Dina", "Agapov", "dinY1787%$%");
            member2 = new Member(userID2, "Lili@gmail.com", "lili", "lili", "liliY1787%$%");

        }
        #endregion
        
        #region Tests

        [TestMethod]
        public void RegisterObserverSuccess()
        {
            int pre = notificationSystem.Obesevers.Count;
            notificationSystem.RegisterObserver(member1);
            Assert.AreEqual(pre + 1 , notificationSystem.Obesevers.Count);
        }
        
        [TestMethod]
        public void RemoveObserverSuccess()
        {
            notificationSystem.RegisterObserver(member1);
            notificationSystem.RegisterObserver(member2);
            int pre = notificationSystem.Obesevers.Count;
            notificationSystem.RemoveObserver(member1);
            Assert.AreEqual(pre - 1 , notificationSystem.Obesevers.Count);
        }
        [TestMethod]
        public void NotifyWhenTheMemberIsNotLoginSuccess()
        {
            int pre = member1.AwaitingNotification.Count;
            notificationSystem.RegisterObserver(member1);
            notificationSystem.NotifyObservers("memeber2 did something", member2.UserId);
            Assert.AreEqual(pre + 1 , member1.AwaitingNotification.Count);
        }
        [TestMethod]
        public void NotifyWhenTheMemberIsLoginSuccess()
        {
            int pre = member1.AwaitingNotification.Count;
            notificationSystem.RegisterObserver(member1);
            member1.LoggedIn = true;
            notificationSystem.NotifyObservers("memeber2 did something", member2.UserId);
            Assert.AreEqual(pre , member1.AwaitingNotification.Count);
        }
        
        #endregion 
    }
    
}