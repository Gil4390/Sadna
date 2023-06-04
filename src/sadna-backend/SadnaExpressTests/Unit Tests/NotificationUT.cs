﻿using System;
using System.Collections.Generic;
 using System.Linq;
 using SadnaExpress.DomainLayer;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.API.SignalR;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.DataLayer;

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
        private Guid userID3;
        private Member member3;

        #region SetUp
        [TestInitialize]
        public void SetUp()
        {
            DatabaseContextFactory.TestMode = true;
            DBHandler.Instance.CleanDB();
            NotificationNotifier.GetInstance().TestMood = true;
            UserFacade userFacade = new UserFacade();
            notificationSystem.userFacade = userFacade;
            storeID = Guid.NewGuid();
            userID1 = Guid.NewGuid();
            userID2 = Guid.NewGuid();
            userID3 = Guid.NewGuid();

            member1 = new Member(userID1, "Dina@gmail.com", "Dina", "Agapov", "dinY1787%$%");
            member2 = new Member(userID2, "Lili@gmail.com", "lili", "lili", "liliY1787%$%");
            member3 = new Member(userID3, "Ayelet@gmail.com", "ayelet", "koz", "liliY1787%$%");
            userFacade.members.TryAdd(userID1, member1);
            userFacade.members.TryAdd(userID2, member2);
            userFacade.members.TryAdd(userID3, member3);
        }
        #endregion
        
        #region Tests

        [TestMethod]
        public void RegisterObserverSuccess()
        {
            notificationSystem.RegisterObserver(storeID,member1);
            int pre = notificationSystem.NotificationOfficials[storeID].Count;
            notificationSystem.RegisterObserver(storeID,member2);
            Assert.IsTrue( notificationSystem.NotificationOfficials[storeID].Contains(member1));
        }
        
        
        [TestMethod]
        public void RemoveObserverSuccess()
        {
            notificationSystem.RegisterObserver(storeID,member1);
            notificationSystem.RegisterObserver(storeID,member2);;
            notificationSystem.RemoveObserver(storeID,member1);
            Assert.IsTrue(!notificationSystem.NotificationOfficials[storeID].Contains(member1));
        }
        
        
        [TestMethod]
        public void NotifyWhenTheMemberIsNotLoginSuccess()
        {
            notificationSystem.RegisterObserver(storeID,member1);
            notificationSystem.NotifyObservers(storeID,"memeber2 did something", member2.UserId);
            int size = member1.AwaitingNotification.Count() - 1;
            Assert.AreEqual("memeber2 did something" , member1.AwaitingNotification[size].Message);
        }
        
        
        [TestMethod]
        public void NotifyWhenTheMemberIsLoginSuccess()
        {
            notificationSystem.RegisterObserver(storeID,member1);
            member1.LoggedIn = true;
            notificationSystem.NotifyObservers(storeID,"memeber1 did something", member2.UserId);
            int size = member1.AwaitingNotification.Count() - 1;
            Assert.AreEqual("memeber1 did something" , member1.AwaitingNotification[size].Message);
        }
        
        
        [TestMethod]
        public void NotificationWasNotRead()
        {
            notificationSystem.RegisterObserver(storeID,member1);
            member1.LoggedIn = true;
            notificationSystem.NotifyObservers(storeID,"memeber1 did something", member2.UserId);
            int size = unreadMessages(member1.AwaitingNotification).Count() - 1;
            Assert.AreEqual("memeber1 did something" , unreadMessages(member1.AwaitingNotification)[size].Message);
        }
        
        
        [TestMethod]
        public void NotificationMarkedAsRead()
        {
            int pre = unreadMessages(member1.AwaitingNotification).Count();
            notificationSystem.RegisterObserver(storeID,member1);
            member1.LoggedIn = true;
            notificationSystem.NotifyObservers(storeID,"memeber1 did something", member2.UserId);
            member1.AwaitingNotification[0].Read = true;
            Assert.AreEqual(pre , unreadMessages(member1.AwaitingNotification).Count);
        }
        
        
        [TestMethod]
        public void NotificationToAllMembers()
        {
            notificationSystem.RegisterObserver(storeID,member1);
            notificationSystem.RegisterObserver(storeID,member3);
            
            notificationSystem.NotifyObservers(storeID,"memeber2 did something", member2.UserId);
            notificationSystem.NotifyObservers(storeID,"memeber2 did something", member2.UserId);
            int size1 = unreadMessages(member1.AwaitingNotification).Count() - 1;
            int size2 = unreadMessages(member3.AwaitingNotification).Count() - 1;
            Assert.AreEqual("memeber2 did something" , unreadMessages(member1.AwaitingNotification)[size1].Message);
            Assert.AreEqual("memeber2 did something" , unreadMessages(member3.AwaitingNotification)[size2].Message);
        }
        
        [TestMethod]
        public void OnlyOneMemberMarkAsRead()
        {
            notificationSystem.RegisterObserver(storeID,member1);
            notificationSystem.RegisterObserver(storeID,member3);
            
            notificationSystem.NotifyObservers(storeID,"memeber2 did something", member2.UserId);
            int size1 = unreadMessages(member1.AwaitingNotification).Count() - 1;
            int size2 = unreadMessages(member3.AwaitingNotification).Count() - 1;
            Notification message1 = unreadMessages(member1.AwaitingNotification)[size1];
            Notification message2 = unreadMessages(member3.AwaitingNotification)[size2];
            unreadMessages(member1.AwaitingNotification)[size1].Read = true;

            Assert.IsFalse(unreadMessages(member1.AwaitingNotification).Contains(message1));
            Assert.IsTrue(unreadMessages(member3.AwaitingNotification).Contains(message2));
            
        }
        
        [TestMethod]
        public void RegisterTwiceTheSameMember()
        {
            int pre = notificationSystem.NotificationOfficials.Count;
            notificationSystem.RegisterObserver(storeID,member1);
            notificationSystem.RegisterObserver(storeID,member1);

            Assert.AreEqual( notificationSystem.NotificationOfficials.Count , pre + 1)  ;
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