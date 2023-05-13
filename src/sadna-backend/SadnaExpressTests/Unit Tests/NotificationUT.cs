using System;
using System.Collections.Generic;
using SadnaExpress.DomainLayer;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.API.SignalR;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpressTests.Unit_Tests
{
    [TestClass()]
    public class NotificationUT
    {
        private NotificationSystem notificationSystem = NotificationSystem.Instance;
        private Guid storeID;
        private Guid userID1;
        private Member member1;
        private Guid userID2;
        private Member member2;

        #region SetUp
        [TestInitialize]
        public void SetUp()
        {
            NotificationNotifier.GetInstance().TestMood = true;
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
            notificationSystem.RegisterObserver(storeID,member1);
            int pre = notificationSystem.StoreOwners[storeID].Count;
            notificationSystem.RegisterObserver(storeID,member2);
            Assert.AreEqual(pre + 1 , notificationSystem.StoreOwners[storeID].Count);
        }
        
        
        [TestMethod]
        public void RemoveObserverSuccess()
        {
            notificationSystem.RegisterObserver(storeID,member1);
            notificationSystem.RegisterObserver(storeID,member2);;
            int pre = notificationSystem.StoreOwners[storeID].Count;
            notificationSystem.RemoveObserver(storeID,member1);
            Assert.AreEqual(pre - 1 , notificationSystem.StoreOwners[storeID].Count);
        }
        
        
        [TestMethod]
        public void NotifyWhenTheMemberIsNotLoginSuccess()
        {
            int pre = member1.AwaitingNotification.Count;
            notificationSystem.RegisterObserver(storeID,member1);
            notificationSystem.NotifyObservers(storeID,"memeber2 did something", member2.UserId);
            Assert.AreEqual(pre + 1 , member1.AwaitingNotification.Count);
        }
        
        
        [TestMethod]
        public void NotifyWhenTheMemberIsLoginSuccess()
        {
            int pre = member1.AwaitingNotification.Count;
            notificationSystem.RegisterObserver(storeID,member1);
            member1.LoggedIn = true;
            notificationSystem.NotifyObservers(storeID,"memeber1 did something", member2.UserId);
            Assert.AreEqual(pre + 1 , member1.AwaitingNotification.Count);
        }
        
        
        [TestMethod]
        public void NotificationWasNotRead()
        {
            notificationSystem.RegisterObserver(storeID,member1);
            member1.LoggedIn = true;
            notificationSystem.NotifyObservers(storeID,"memeber1 did something", member2.UserId);
            Assert.AreEqual(1 , unreadMessages(member1.AwaitingNotification).Count);
        }
        
        
        [TestMethod]
        public void NotificationMarkedAsRead()
        {
            notificationSystem.RegisterObserver(storeID,member1);
            member1.LoggedIn = true;
            notificationSystem.NotifyObservers(storeID,"memeber1 did something", member2.UserId);
            member1.AwaitingNotification[0].Read = true;
            Assert.AreEqual(0 , unreadMessages(member1.AwaitingNotification).Count);
        }
        
        
        [TestMethod]
        public void NotificationToAllMembers()
        {
            notificationSystem.RegisterObserver(storeID,member1);
            notificationSystem.RegisterObserver(storeID,member2);

            member1.LoggedIn = true;
            notificationSystem.NotifyObservers(storeID,"memeber1 did something", member2.UserId);
            notificationSystem.NotifyObservers(storeID,"memeber1 did something", member2.UserId);

            Assert.AreEqual(2 , unreadMessages(member1.AwaitingNotification).Count + unreadMessages(member2.AwaitingNotification).Count);
        }
        
        [TestMethod]
        public void OnlyOneMemberMarkAsRead()
        {
            notificationSystem.RegisterObserver(storeID,member1);
            notificationSystem.RegisterObserver(storeID,member2);
            member1.LoggedIn = true;
            notificationSystem.NotifyObservers(storeID,"memeber1 did something", member2.UserId);
            notificationSystem.NotifyObservers(storeID,"memeber1 did something", member2.UserId);
            unreadMessages(member1.AwaitingNotification)[0].Read = true;
            Assert.AreEqual(1 , unreadMessages(member1.AwaitingNotification).Count + unreadMessages(member2.AwaitingNotification).Count);
        }
        
        [TestMethod]
        public void RegisterTwiceTheSameMember()
        {
            int pre = notificationSystem.StoreOwners.Count;
            notificationSystem.RegisterObserver(storeID,member1);
            notificationSystem.RegisterObserver(storeID,member1);

            Assert.AreEqual( notificationSystem.StoreOwners.Count , pre + 1)  ;
        }
  
        public List<Notification> unreadMessages(List<Notification> notifications)
        {
            List<Notification> notificationsUnRead = new List<Notification>();
            foreach (Notification notification in notifications)
            {
                if(!notification.Read)
                    notificationsUnRead.Add(notification);
            }

            return notificationsUnRead;
        }
        #endregion 
    }
    
}