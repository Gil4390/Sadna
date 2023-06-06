using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DataLayer;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.SModels;
using SadnaExpressTests.Integration_Tests;

namespace SadnaExpressTests.Persistence_Tests
{
    [TestClass]
    public class UsersDBtests: TradingSystemIT
    {
        [TestInitialize]
        public override void Setup()
        {
            base.setTestMood();    
            base.Setup();
            DatabaseContextFactory.TestMode = true;
        }

        [TestMethod()]
        public void DB_Register_empty_Success()
        {
            Guid id = trading.Enter().Value;
            trading.Register(id, "omer@weka.io","omer", "shikma", "143AaC!@#");
            Guid memID = trading.Login(id, "omer@weka.io", "143AaC!@#").Value;
            Assert.IsTrue(DBHandler.Instance.memberExistsById(memID));
        }
        
        [TestMethod()]
        public void DB_Register_non_empty_Success()
        {
            int numOfMembers = DBHandler.Instance.GetAllMembers().Count;
            
            Guid id = trading.Enter().Value;
            trading.Register(id, "omer@weka.io","omer", "shikma", "143AaC!@#");
            Assert.AreEqual(numOfMembers + 1, DBHandler.Instance.GetAllMembers().Count);
            Guid memID = trading.Login(id, "omer@weka.io", "143AaC!@#").Value;
            Assert.IsTrue(DBHandler.Instance.memberExistsById(memID));
        }
        
        
        [TestMethod()]
        public void DB_Register_invalid_ID_Fail()
        {
            DBHandler.Instance.CleanDB();
            Guid id = new Guid();

            trading.Register(id, "omer@weka.io", "omer", "shikma", "143AaC!@#");

            Assert.IsFalse(DBHandler.Instance.memberExistsById(id));
        }
        
        
        [TestMethod()]
        public void DB_Remove_non_empty_Success()
        {
            Guid tempid = trading.Enter().Value;
            int numOfMembers = DBHandler.Instance.GetAllMembers().Count;
            
            Guid id = trading.Enter().Value;
            trading.Register(id, "omer@weka.io","omer", "shikma", "143AaC!@#");
            Guid loggedid2 = trading.Login(id, "omer@weka.io", "143AaC!@#").Value;
            trading.RemoveUserMembership(buyerMemberID, "omer@weka.io");
            Assert.AreEqual(numOfMembers , DBHandler.Instance.GetAllMembers().Count);
            Assert.IsFalse(DBHandler.Instance.memberExistsById(loggedid2));
        }

        [TestMethod()]
        public void DB_Promote_Success()
        {
            Guid id = trading.Enter().Value;
            trading.Register(id, "omer@weka.io", "omer", "shikma", "143AaC!@#");
            Guid tempid = trading.Enter().Value;
            trading.AppointStoreOwner(userID, storeID1, "omer@weka.io");
            Member cur = DBHandler.Instance.GetMemberFromDBByEmail("omer@weka.io");
            Assert.IsNotNull(cur);
            Assert.IsTrue(cur.hasPermissions(storeID1, 
                new List<string> { "owner permissions" }));
        }

        
        [TestMethod()]
        public void DB_Promote_Fail()
        {
            Guid id = trading.Enter().Value;
            trading.Register(id, "omer@weka.io", "omer", "shikma", "143AaC!@#");
            Guid tempid = trading.Enter().Value;
            trading.AddStoreManagerPermissions(userID, storeID1, "omer@weka.io", "Blah Blah");
            Member cur = DBHandler.Instance.GetMemberFromDBByEmail("omer@weka.io");
            Assert.IsNotNull(cur);
            Assert.IsFalse(cur.hasPermissions(storeID1, 
                new List<string> { "owner permissions" }));
        }

        [TestMethod]
        public void SystemManagerRequestMembersInformation_HappyTest()
        {
            List<SMember> members = trading.GetMembers(buyerMemberID).Value;

            Assert.AreEqual(2, members.Count); 
        }

    }
}