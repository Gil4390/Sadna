﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using SadnaExpressTests.Acceptance_Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpressTests.Integration_Tests
{
    [TestClass]
    public class SystemActivityIT: TradingSystemIT
    {
        private UserUsageData userUsageData;
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            userUsageData = UserUsageData.Instance;
        }

        [TestMethod()]
        public void GuestEnterVisitSuccess()
        {
            //Arrange & Act
            int guestCount = userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[0];
            Guid guestId = trading.Enter().Value;
           
            //Assert
            Assert.IsTrue(userUsageData.UsersVisits.Count(item=>item.UserID == guestId)==1);
            Assert.IsTrue(guestCount+1 == userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[0]);
        }

        [TestMethod()]
        public void GuestEnterAndThenLogInVisitSuccess()
        {
            //Arrange & Act
            int guestCount = userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[0];
            int memberCount = userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[1];
            Guid guestId = trading.Enter().Value;
            trading.Register(guestId, "bruno@gmail.com", "Bruno", "Biton", "asASD876!@");
            Guid loginId = trading.Login(guestId, "bruno@gmail.com", "asASD876!@").Value;
            //Assert
            Assert.IsTrue(userUsageData.UsersVisits.Count(item => item.UserID == guestId) == 0 && UserUsageData.Instance.UsersVisits.Count(item => item.UserID == loginId) == 1);
            Assert.IsTrue(guestCount == userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[0]);
            Assert.IsTrue(memberCount + 1 == userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[1]);
        }


        [TestMethod()]
        public void MemberEnterAndLogoutLoginExitTwiceVisitSuccess()
        {
            //Arrange & Act
            int guestCount = userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[0];
            int memberCount = userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[1];
            Guid guestId = trading.Enter().Value;
            trading.Register(guestId, "bruno@gmail.com", "Bruno", "Biton", "asASD876!@");
            Guid loginId = trading.Login(guestId, "bruno@gmail.com", "asASD876!@").Value;

            trading.Exit(loginId);
            Guid enterAgain = trading.Enter().Value;
            Guid loginIdAgain = trading.Login(enterAgain, "bruno@gmail.com", "asASD876!@").Value;

            //Assert
            Assert.IsTrue(userUsageData.UsersVisits.Count(item => item.UserID == guestId) == 0 && userUsageData.UsersVisits.Count(item => item.UserID == enterAgain) == 0 && UserUsageData.Instance.UsersVisits.Count(item => item.UserID == loginIdAgain) == 1);
            Assert.IsTrue(guestCount == userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[0]);
            Assert.IsTrue(memberCount + 1 == userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[1]);
        }


        [TestMethod()]
        public void MemberEnterAndLogoutLoginTwiceVisitSuccess()
        {
            //Arrange & Act
            int guestCount = userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[0];
            int memberCount = userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[1];
            Guid guestId = trading.Enter().Value;
            trading.Register(guestId, "bruno@gmail.com", "Bruno", "Biton", "asASD876!@");
            Guid loginId = trading.Login(guestId, "bruno@gmail.com", "asASD876!@").Value;

            Guid enterAgain = trading.Logout(loginId).Value;
            Guid loginIdAgain = trading.Login(enterAgain, "bruno@gmail.com", "asASD876!@").Value;

            //Assert
            Assert.IsTrue(userUsageData.UsersVisits.Count(item => item.UserID == guestId) == 0 && userUsageData.UsersVisits.Count(item => item.UserID == enterAgain) == 0 && UserUsageData.Instance.UsersVisits.Count(item => item.UserID == loginIdAgain) == 1);
            Assert.IsTrue(guestCount == userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[0]);
            Assert.IsTrue(memberCount + 1 == userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[1]);
        }

        [TestMethod()]
        public void StoreManagerVisitSuccess()
        {
            //Arrange
            Guid guestId = trading.Enter().Value;
            trading.Register(guestId, "bruno@gmail.com", "Bruno", "Biton", "asASD876!@"); //register bruno

            trading.AppointStoreManager(userID, storeID1, "bruno@gmail.com"); //bruno is store manager

            //Act
            Guid loginId = trading.Login(guestId, "bruno@gmail.com", "asASD876!@").Value; //bruno login

            //Assert
            Assert.IsTrue(userUsageData.UsersVisits.Count(item => item.UserID == guestId) == 0); //no guest visit 
            Visit visit= userUsageData.UsersVisits.FirstOrDefault(item => item.UserID == loginId);
            Assert.IsTrue(visit!=null); //visit of bruno exist
            Assert.IsTrue(visit.Role=="store manager");
        }

        [TestMethod()]
        public void StoreOwnerVisitSuccess()
        {
            //Arrange
            Guid guestId = trading.Enter().Value;
            trading.Register(guestId, "bruno@gmail.com", "Bruno", "Biton", "asASD876!@"); //register bruno

            trading.AppointStoreOwner(userID, storeID1, "bruno@gmail.com"); //bruno is store manager

            //Act
            Guid loginId = trading.Login(guestId, "bruno@gmail.com", "asASD876!@").Value; //bruno login

            //Assert
            Assert.IsTrue(userUsageData.UsersVisits.Count(item => item.UserID == guestId) == 0); //no guest visit 
            Visit visit = userUsageData.UsersVisits.FirstOrDefault(item => item.UserID == loginId);
            Assert.IsTrue(visit != null); //visit of bruno exist
            Assert.IsTrue(visit.Role == "founder or owner");
        }


        [TestMethod()]
        public void StoreOwnerAndManagerVisitSuccess()
        {
            //Arrange
            Guid guestId = trading.Enter().Value;
            trading.Register(guestId, "bruno@gmail.com", "Bruno", "Biton", "asASD876!@"); //register bruno

            trading.AppointStoreOwner(userID, storeID1, "bruno@gmail.com"); //bruno is store manager
            trading.AppointStoreManager(userID, storeID2, "bruno@gmail.com"); //bruno is store manager

            //Act
            Guid loginId = trading.Login(guestId, "bruno@gmail.com", "asASD876!@").Value; //bruno login

            //Assert
            Assert.IsTrue(userUsageData.UsersVisits.Count(item => item.UserID == guestId) == 0); //no guest visit 
            Visit visit = userUsageData.UsersVisits.FirstOrDefault(item => item.UserID == loginId);
            Assert.IsTrue(visit != null); //visit of bruno exist
            Assert.IsTrue(visit.Role == "founder or owner");
        }

        [TestMethod()]
        public void SystemManagerLogInVisitSuccess()
        {
            //Arrange 
            trading.Logout(buyerMemberID);
            Guid guestId = trading.Enter().Value;
           
            //Act
            buyerMemberID = trading.Login(guestId, "dor@gmail.com", "asASD876!@").Value;

            //Assert
            Assert.IsTrue(userUsageData.UsersVisits.Count(item => item.UserID == guestId) == 0); //no guest visit 
            Visit visit = userUsageData.UsersVisits.FirstOrDefault(item => (item.UserID == buyerMemberID) & (item.Role== "system manager"));
            Assert.IsTrue(visit != null); //visit of RotemSela exist
        }


    }
}
