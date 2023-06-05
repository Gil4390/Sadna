using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DataLayer;
using SadnaExpress.DomainLayer.User;
using SadnaExpressTests.Integration_Tests;

namespace SadnaExpressTests.Persistence_Tests
{
    [TestClass]
    public class UsersDBtests: TradingSystemIT
    {
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            DatabaseContextFactory.TestMode = true;
        }

        [TestMethod()]
        public void DB_Register_empty_Success()
        {
            Guid id = trading.Enter().Value;
            trading.Register(id, "omer@weka.io","omer", "shikma", "143AaC!@#");
            Assert.IsTrue(DBHandler.Instance.memberExistsById(id));
        }
        
        [TestMethod()]
        public void DB_Register_non_empty_Success()
        {
            int numOfMembers = DBHandler.Instance.GetAllMembers().Count;
            
            Guid id = trading.Enter().Value;
            trading.Register(id, "omer@weka.io","omer", "shikma", "143AaC!@#");
            Assert.Equals(numOfMembers + 1, DBHandler.Instance.GetAllMembers().Count);
            Assert.IsTrue(DBHandler.Instance.memberExistsById(id));
        }
        
        
        [TestMethod()]
        public void DB_Register_invalid_ID_Fail()
        {
            DBHandler.Instance.CleanDB();
            Guid unknown = new Guid();
            trading.Register(unknown, "tal.galmor@weka.io","tal", "galmor", "123AaC!@#");
            Assert.IsTrue(DBHandler.Instance.memberExistsById(unknown));
        }
        
        [TestMethod()]
        public void DB_Remove_empty_Success()
        {
            Guid id = trading.Enter().Value; 
            trading.Register(id, "omer@weka.io","omer", "shikma", "143AaC!@#");
            Guid loggedid = trading.Login(id, "omer@weka.io", "143AaC!@#").Value;
            trading.RemoveUserMembership(loggedid, "omer@weka.io");
            Assert.IsFalse(DBHandler.Instance.memberExistsById(id));
            Assert.Equals(1, DBHandler.Instance.GetAllMembers().Count);

        }
        
        [TestMethod()]
        public void DB_Remove_non_empty_Success()
        {
            Guid tempid = trading.Enter().Value;
            Guid loggedid = trading.Login(tempid, "RotemSela@gmail.com", "AS87654askj").Value;
            int numOfMembers = DBHandler.Instance.GetAllMembers().Count;
            
            Guid id = trading.Enter().Value;
            trading.Register(id, "omer@weka.io","omer", "shikma", "143AaC!@#");
            trading.RemoveUserMembership(loggedid, "omer@weka.io");
            Assert.Equals(numOfMembers , DBHandler.Instance.GetAllMembers().Count);
            Assert.IsFalse(DBHandler.Instance.memberExistsById(id));
        }

        [TestMethod()]
        public void DB_Promote_Success()
        {
            Guid id = trading.Enter().Value;
            trading.Register(id, "omer@weka.io", "omer", "shikma", "143AaC!@#");
            Guid tempid = trading.Enter().Value;
            Guid managerID = trading.Login(tempid, "RotemSela@gmail.com", "AS87654askj").Value;
            trading.AddStoreManagerPermissions(managerID, storeID1, "omer@weka.io", "owner permissions");
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
            Guid managerID = trading.Login(tempid, "RotemSela@gmail.com", "AS87654askj").Value;
            trading.AddStoreManagerPermissions(managerID, storeID1, "omer@weka.io", "Blah Blah");
            Member cur = DBHandler.Instance.GetMemberFromDBByEmail("omer@weka.io");
            Assert.IsNotNull(cur);
            Assert.IsFalse(cur.hasPermissions(storeID1, 
                new List<string> { "owner permissions" }));
        }

    }
}