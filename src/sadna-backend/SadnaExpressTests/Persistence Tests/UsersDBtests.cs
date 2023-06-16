using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DataLayer;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.SModels;
using SadnaExpressTests.Integration_Tests;

namespace SadnaExpressTests.Persistence_Tests
{
    [TestClass]
    public class UsersDBtests: TradingSystemPT
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

        [TestMethod()]
        public void DB_CheckTheFlowOfAppointOwner_Fail()
        {
            //create user
            Guid id = trading.Enter().Value;
            trading.Register(id, "omer@weka.io", "omer", "shikma", "143AaC!@#");
            Guid tempid = trading.Enter().Value;
            //check that the user created
            Member cur = DBHandler.Instance.GetMemberFromDBByEmail("omer@weka.io");
            Assert.IsNotNull(cur);

            //appoint first store owner
            trading.AppointStoreOwner(userID, storeID1, "dor@gmail.com");
            PromotedMember cur2 = (PromotedMember)DBHandler.Instance.GetMemberFromDBByEmail("dor@gmail.com");
            Assert.IsFalse(cur.hasPermissions(storeID1, new List<string> { "owner permissions" }));
            Assert.IsTrue(cur2.hasPermissions(storeID1, new List<string> { "owner permissions" }));

            //appoint store owner2 - one false
            trading.AppointStoreOwner(userID, storeID1, "omer@weka.io");
            cur = DBHandler.Instance.GetMemberFromDBByEmail("omer@weka.io");
            cur2 = (PromotedMember)DBHandler.Instance.GetMemberFromDBById(buyerMemberID);
            Assert.AreEqual(1, cur2.PermissionsOffers[storeID1].Count);
            Assert.IsTrue(cur2.PermissionsOffers[storeID1].Contains(cur.UserId));
            Assert.IsTrue(cur.PenddingPermission[storeID1].ContainsKey(cur2.Email));
            trading.ReactToJobOffer(buyerMemberID, storeID1, cur.UserId, false);
            cur = DBHandler.Instance.GetMemberFromDBByEmail("omer@weka.io");
            cur2 = (PromotedMember)DBHandler.Instance.GetMemberFromDBById(buyerMemberID);
            Assert.IsFalse(cur.hasPermissions(storeID1, new List<string> { "owner permissions" }));
            Assert.IsFalse(cur2.PermissionsOffers[storeID1].Contains(cur.UserId));
            Assert.IsFalse(cur.PenddingPermission.ContainsKey(storeID1));

            //appoint store owner2
            trading.AppointStoreOwner(userID, storeID1, "omer@weka.io");
            trading.ReactToJobOffer(cur2.UserId, storeID1, cur.UserId, true);
            cur = DBHandler.Instance.GetMemberFromDBByEmail("omer@weka.io");
            cur2 = (PromotedMember)DBHandler.Instance.GetMemberFromDBById(buyerMemberID);
            Assert.IsTrue(cur2.hasPermissions(storeID1, new List<string> { "owner permissions" }));
            Assert.IsFalse(cur2.PermissionsOffers[storeID1].Contains(cur.UserId));
            Assert.IsFalse(cur.PenddingPermission.ContainsKey(storeID1));
        }

        [TestMethod()]
        public void DB_CheckTheFlowOfAppointOwner2Stores_Fail()
        {
            //create user
            Guid id = trading.Enter().Value;
            trading.Register(id, "omer@weka.io", "omer", "shikma", "143AaC!@#");
            Guid tempid = trading.Enter().Value;
            //check that the user created
            Member cur = DBHandler.Instance.GetMemberFromDBByEmail("omer@weka.io");
            Assert.IsNotNull(cur);

            //appoint first store owner
            trading.AppointStoreOwner(userID, storeID1, "dor@gmail.com");
            trading.AppointStoreOwner(userID, storeID2, "dor@gmail.com");
            PromotedMember cur2 = (PromotedMember)DBHandler.Instance.GetMemberFromDBByEmail("dor@gmail.com");
            Assert.IsFalse(cur.hasPermissions(storeID1, new List<string> { "owner permissions" }));
            Assert.IsTrue(cur2.hasPermissions(storeID1, new List<string> { "owner permissions" }));
            Assert.IsTrue(cur2.hasPermissions(storeID2, new List<string> { "owner permissions" }));

            //appoint store owner2 to store 1 
            trading.AppointStoreOwner(userID, storeID1, "omer@weka.io");
            trading.AppointStoreOwner(userID, storeID2, "omer@weka.io");
            cur = DBHandler.Instance.GetMemberFromDBByEmail("omer@weka.io");
            cur2 = (PromotedMember)DBHandler.Instance.GetMemberFromDBById(buyerMemberID);

            trading.ReactToJobOffer(buyerMemberID, storeID2, cur.UserId, false);
            trading.ReactToJobOffer(buyerMemberID, storeID1, cur.UserId, true);

            cur = DBHandler.Instance.GetMemberFromDBByEmail("omer@weka.io");
            cur2 = (PromotedMember)DBHandler.Instance.GetMemberFromDBById(buyerMemberID);
            Assert.IsTrue(cur.hasPermissions(storeID1, new List<string> { "owner permissions" }));
            Assert.IsFalse(cur.PenddingPermission.ContainsKey(storeID1));
            Assert.IsFalse(cur2.PermissionsOffers[storeID1].Contains(cur.UserId));
            Assert.IsFalse(cur.hasPermissions(storeID2, new List<string> { "owner permissions" }));
            Assert.IsFalse(cur.PenddingPermission.ContainsKey(storeID2));
            Assert.IsFalse(cur2.PermissionsOffers[storeID2].Contains(cur.UserId));

            //appoint store owner2
            trading.AppointStoreOwner(userID, storeID2, "omer@weka.io");
            trading.ReactToJobOffer(cur2.UserId, storeID2, cur.UserId, true);
            cur = DBHandler.Instance.GetMemberFromDBByEmail("omer@weka.io");
            cur2 = (PromotedMember)DBHandler.Instance.GetMemberFromDBById(buyerMemberID);
            Assert.IsTrue(cur2.hasPermissions(storeID2, new List<string> { "owner permissions" }));
            Assert.IsFalse(cur2.PermissionsOffers[storeID2].Contains(cur.UserId));
            Assert.IsFalse(cur.PenddingPermission.ContainsKey(storeID2));
        }

        #region user usage data

        [TestMethod()]
        public void DB_Enter_Visit_Success()
        {
            //Arrange & Act
            Guid id = trading.Enter().Value;

            //Assert
            Assert.IsTrue(DBHandler.Instance.GetAllVisits().Where(item=>item.UserID==id && item.Role=="guest").Count() == 1);
        }

        [TestMethod()]
        public void DB_MenberLogin_Visit_Success()
        {
            //Arrange & Act
            Guid id = trading.Enter().Value;
            trading.Register(id, "omer@weka.io", "omer", "shikma", "143AaC!@#");
            Guid memID = trading.Login(id, "omer@weka.io", "143AaC!@#").Value;

            //Assert
            List<Visit> visits=DBHandler.Instance.GetAllVisits();
            Assert.IsTrue(visits.Where(item => item.UserID == id && item.Role == "guest").Count() == 0
                && visits.Where(item => item.UserID == memID && item.Role == "member").Count() == 1);
        }

        [TestMethod()]
        public void DB_StoreFounderLogin_Visit_Success()
        {
            //Arrange & Act
            Guid id = trading.Enter().Value;
            trading.Register(id, "omer@weka.io", "omer", "shikma", "143AaC!@#");
            Guid memID = trading.Login(id, "omer@weka.io", "143AaC!@#").Value;
            trading.OpenNewStore(memID,"wow");
            trading.Exit(memID);

            Guid newId = trading.Enter().Value;
            Guid storeFounder = trading.Login(newId, "omer@weka.io", "143AaC!@#").Value;


            //Assert
            List<Visit> visits = DBHandler.Instance.GetAllVisits();
            Assert.IsTrue(visits.Where(item => item.UserID == id && item.Role == "guest").Count() == 0
                && visits.Where(item => item.UserID == memID && item.Role == "member").Count() == 1
                && visits.Where(item => item.UserID == newId && item.Role == "guest").Count() == 0
                && visits.Where(item => item.UserID == storeFounder && item.Role == "founder or owner").Count() == 1);
        }

        [TestMethod()]
        public void DB_StoreFounderLoginTwice_Visit_Success()
        {
            //Arrange & Act
            Guid id = trading.Enter().Value;
            trading.Register(id, "omer@weka.io", "omer", "shikma", "143AaC!@#");
            Guid memID = trading.Login(id, "omer@weka.io", "143AaC!@#").Value;
            trading.OpenNewStore(memID, "wow");
            trading.Exit(memID);

            Guid newId = trading.Enter().Value;
            Guid storeFounder = trading.Login(newId, "omer@weka.io", "143AaC!@#").Value;

            trading.Exit(storeFounder);
            Guid newIdEnterAgain = trading.Enter().Value;
            trading.Login(newIdEnterAgain, "omer@weka.io", "143AaC!@#"); //new visit is not created

            //Assert
            List<Visit> visits = DBHandler.Instance.GetAllVisits();
            Assert.IsTrue(visits.Where(item => item.UserID == id && item.Role == "guest").Count() == 0
                && visits.Where(item => item.UserID == memID && item.Role == "member").Count() == 1
                && visits.Where(item => item.UserID == newId && item.Role == "guest").Count() == 0
                && visits.Where(item => item.UserID == storeFounder && item.Role == "founder or owner").Count() == 1);
        }

        [TestMethod()]
        public void DB_StoreOwnerLogin_Visit_Success()
        {
            //Arrange & Act
            Guid id = trading.Enter().Value;
            trading.Register(id, "tal@gmail.com", "Tal", "Galmor", "w3ka!Tal");

            trading.Register(id, "omer@weka.io", "omer", "shikma", "143AaC!@#");
            Guid memID = trading.Login(id, "omer@weka.io", "143AaC!@#").Value;
            Guid storeId=trading.OpenNewStore(memID, "wow").Value;
            trading.AppointStoreOwner(memID, storeId,"tal@gmail.com");

            trading.Exit(memID);

            Guid newId = trading.Enter().Value;
            Guid storeOwner = trading.Login(newId, "tal@gmail.com", "w3ka!Tal").Value;

            //Assert
            List<Visit> visits = DBHandler.Instance.GetAllVisits();
            Assert.IsTrue(visits.Where(item => item.UserID == id && item.Role == "guest").Count() == 0
                && visits.Where(item => item.UserID == memID && item.Role == "member").Count() == 1
                && visits.Where(item => item.UserID == newId && item.Role == "guest").Count() == 0
                && visits.Where(item => item.UserID == storeOwner && item.Role == "founder or owner").Count() == 1);
        }

        [TestMethod()]
        public void DB_StoreManagerLogin_Visit_Success()
        {
            //Arrange & Act
            Guid id = trading.Enter().Value;
            trading.Register(id, "tal@gmail.com", "Tal", "Galmor", "w3ka!Tal");

            trading.Register(id, "omer@weka.io", "omer", "shikma", "143AaC!@#");
            Guid memID = trading.Login(id, "omer@weka.io", "143AaC!@#").Value;
            Guid storeId = trading.OpenNewStore(memID, "wow").Value;
            trading.AppointStoreManager(memID, storeId, "tal@gmail.com");

            trading.Exit(memID);

            Guid newId = trading.Enter().Value;
            Guid storeOwner = trading.Login(newId, "tal@gmail.com", "w3ka!Tal").Value;

            //Assert
            List<Visit> visits = DBHandler.Instance.GetAllVisits();
            Assert.IsTrue(visits.Where(item => item.UserID == id && item.Role == "guest").Count() == 0
                && visits.Where(item => item.UserID == memID && item.Role == "member").Count() == 1
                && visits.Where(item => item.UserID == newId && item.Role == "guest").Count() == 0
                && visits.Where(item => item.UserID == storeOwner && item.Role == "store manager").Count() == 1);
        }

        [TestMethod()]
        public void DB_StoreOwnerAndManagerLogin1_Visit_Success()
        {
            //Arrange & Act
            Guid id = trading.Enter().Value;
            trading.Register(id, "tal@gmail.com", "Tal", "Galmor", "w3ka!Tal");
            trading.Register(id, "omer@weka.io", "omer", "shikma", "143AaC!@#");
            Guid memID = trading.Login(id, "omer@weka.io", "143AaC!@#").Value;
            Guid storeIdWow = trading.OpenNewStore(memID, "wow").Value;
            trading.AppointStoreOwner(memID, storeIdWow, "tal@gmail.com"); //tal is owner

            Guid id1 = trading.Enter().Value;
            trading.Register(id1, "amihai@gmail.com", "Amihai", "Amihai", "asASD123!@");
            Guid memID1 = trading.Login(id1, "amihai@gmail.com", "asASD123!@").Value;
            Guid storeIdCow = trading.OpenNewStore(memID1, "cow").Value;
            trading.AppointStoreManager(memID1, storeIdCow, "tal@gmail.com"); //tal is manager

            Guid newId = trading.Enter().Value;
            Guid storeOwner = trading.Login(newId, "tal@gmail.com", "w3ka!Tal").Value;

            //Assert
            List<Visit> visits = DBHandler.Instance.GetAllVisits();
            Assert.IsTrue(visits.Where(item => item.UserID == id && item.Role == "guest").Count() == 0
                && visits.Where(item => item.UserID == memID && item.Role == "member").Count() == 1
                && visits.Where(item => item.UserID == newId && item.Role == "guest").Count() == 0
                && visits.Where(item => item.UserID == storeOwner && item.Role == "founder or owner").Count() == 1);
        }

        [TestMethod()]
        public void DB_StoreOwnerAndManagerLogin2_Visit_Success()
        { //same as 1- now we swap the order - first manager and then owner
            //Arrange & Act
            Guid id = trading.Enter().Value;
            trading.Register(id, "tal@gmail.com", "Tal", "Galmor", "w3ka!Tal");
            trading.Register(id, "omer@weka.io", "omer", "shikma", "143AaC!@#");
            Guid memID = trading.Login(id, "omer@weka.io", "143AaC!@#").Value;
            Guid storeIdWow = trading.OpenNewStore(memID, "wow").Value;
            trading.AppointStoreManager(memID, storeIdWow, "tal@gmail.com"); //tal is owner

            Guid id1 = trading.Enter().Value;
            trading.Register(id1, "amihai@gmail.com", "Amihai", "Amihai", "asASD123!@");
            Guid memID1 = trading.Login(id1, "amihai@gmail.com", "asASD123!@").Value;
            Guid storeIdCow = trading.OpenNewStore(memID1, "cow").Value;
            trading.AppointStoreOwner(memID1, storeIdCow, "tal@gmail.com"); //tal is manager

            Guid newId = trading.Enter().Value;
            Guid storeOwner = trading.Login(newId, "tal@gmail.com", "w3ka!Tal").Value;

            //Assert
            List<Visit> visits = DBHandler.Instance.GetAllVisits();
            Assert.IsTrue(visits.Where(item => item.UserID == id && item.Role == "guest").Count() == 0
                && visits.Where(item => item.UserID == memID && item.Role == "member").Count() == 1
                && visits.Where(item => item.UserID == newId && item.Role == "guest").Count() == 0
                && visits.Where(item => item.UserID == storeOwner && item.Role == "founder or owner").Count() == 1);
        }

        #endregion
    }
}