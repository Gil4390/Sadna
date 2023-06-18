using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.API.SignalR;
using SadnaExpress.DataLayer;
using SadnaExpress.DomainLayer;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpressTests.Unit_Tests
{
    
    [TestClass()]
    public class StoreOwnerUT : TradingSystemUT
    {
        private Guid storeID;
        private Guid founderID;
        private PromotedMember founder;
        private Guid storeOwnerAppointID;
        private Member storeOwnerAppoint;
        private Guid storeOwnerDirectID;
        private PromotedMember storeOwnerDirect;
        
        #region SetUp
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            UserFacade userFacade = new UserFacade();
            NotificationSystem.Instance.userFacade = userFacade;
            founderID = Guid.NewGuid();
            storeID = Guid.NewGuid();
            storeOwnerDirectID = Guid.NewGuid();
            storeOwnerAppointID = Guid.NewGuid();
            founder =
                new PromotedMember(founderID, "AssiAzar@gmail.com", "Assi", "Azar", "ShaY1787%$%");
            founder.openNewStore(storeID);
            NotificationSystem.Instance.RegisterObserver(storeID, founder);
            Member memberBeforeOwner = new Member(storeOwnerDirectID, "RotemSela@gmail.com", "Rotem", "Sela",
                "ShaY1787%$%");
           
            userFacade.members.TryAdd(founder.UserId, founder);
            storeOwnerDirect = founder.AppointStoreOwner(storeID, memberBeforeOwner);
            storeOwnerAppoint = new Member(storeOwnerAppointID, "Tal@gmail.com", "Tal", "Galmor",
                "ShaY1787%$%");
            userFacade.members.TryAdd(storeOwnerAppoint.UserId, storeOwnerAppoint);
            userFacade.members.TryAdd(storeOwnerDirect.UserId, storeOwnerDirect);
        }
        #endregion
        #region appoint store owner    
        [TestMethod]
        public void AppointStoreOwnerHasPermission()
        {
            Assert.ThrowsException<Exception>(() =>
                founder.AppointStoreOwner(storeID, storeOwnerDirect));
        }
        
        [TestMethod]
        public void AppointStoreOwnerSuccess()
        {
            //Act
            founder.AppointStoreOwner(storeID, storeOwnerAppoint);
            // check the dict
            Assert.IsTrue(storeOwnerAppoint.PenddingPermission[storeID].ContainsKey(storeOwnerDirect.Email));
            Assert.IsTrue(founder.PermissionsOffers.IsEmpty);
            Assert.AreEqual(1, storeOwnerDirect.PermissionsOffers[storeID].Count);
            PromotedMember promotedMember = storeOwnerDirect.ReactToJobOffer(storeID, storeOwnerAppoint, true);
            //Assert
            Assert.IsTrue(founder.getAppoint(storeID).Contains(promotedMember));
            Assert.IsTrue(promotedMember.hasPermissions(storeID, new List<string>{"owner permissions"}));
            Assert.IsTrue(promotedMember.PenddingPermission.IsEmpty);
            Assert.IsTrue(founder.PermissionsOffers.IsEmpty || founder.PermissionsOffers[storeID].Count == 0);
            Assert.AreEqual(0, storeOwnerDirect.PermissionsOffers[storeID].Count);
        }
        
        [TestMethod]
        public void AppointStoreOwnerAgain()
        {
            //Arrange
            Member member = new Member(storeOwnerAppointID, "Tal@gmail.com", "Tal", "Galmor",
                "ShaY1787%$%");
            PromotedMember promotedMember = founder.AppointStoreOwner(storeID, member); //first time
            promotedMember = storeOwnerDirect.ReactToJobOffer(storeID, member, true); // accept the offer
            //Assert
            Assert.ThrowsException<Exception>(() =>
                founder.AppointStoreOwner(storeID, promotedMember));
        }

        [TestMethod]
        public void AppointStoreOwnerNotEveryOneApprove()
        {
            //Act
            founder.AppointStoreOwner(storeID, storeOwnerAppoint);
            founder.LoggedIn = false;
            // check the dict
            Assert.IsTrue(storeOwnerAppoint.PenddingPermission[storeID].ContainsKey(storeOwnerDirect.Email));
            Assert.IsTrue(founder.PermissionsOffers.IsEmpty);
            Assert.AreEqual(1, storeOwnerDirect.PermissionsOffers[storeID].Count);
            PromotedMember promotedMember = storeOwnerDirect.ReactToJobOffer(storeID, storeOwnerAppoint, false);
            //Assert
            Assert.IsFalse(founder.getAppoint(storeID).Contains(promotedMember));
            Assert.IsNull(promotedMember);
            Assert.IsTrue(storeOwnerAppoint.PenddingPermission.IsEmpty);
            Assert.IsTrue(founder.PermissionsOffers.IsEmpty || founder.PermissionsOffers[storeID].Count == 0);
            Assert.AreEqual(0, storeOwnerDirect.PermissionsOffers[storeID].Count);
            Assert.AreEqual(1, founder.awaitingNotification.Count);
        }
        #endregion   
        #region remove store owner       
        [TestMethod]
        public void RemoveStoreOwnerSuccess()
        {
            //Act
            founder.RemoveStoreOwner(storeID, storeOwnerDirect);
            //Assert
            Assert.IsFalse(storeOwnerDirect.hasPermissions(storeID, new List<string>{"owner permissions"}));
            Assert.IsFalse(founder.getAppoint(storeID).Contains(storeOwnerDirect));
        }
        [TestMethod]
        public void RemoveStoreOwnerAndTheAppointSuccess()
        {
            //Arrange
            storeOwnerDirect.AppointStoreOwner(storeID, storeOwnerAppoint);
            storeOwnerAppoint = founder.ReactToJobOffer(storeID, storeOwnerAppoint, true);
            //Act
            storeOwnerAppoint = founder.RemoveStoreOwner(storeID, storeOwnerDirect).Item1[0];
            //Assert
            Assert.IsFalse(storeOwnerDirect.hasPermissions(storeID, new List<string>{"owner permissions"})); //direct owner - no permissions
            Assert.IsFalse(storeOwnerAppoint.hasPermissions(storeID, new List<string>{"owner permissions"})); //appoint owner - no permissions
            Assert.IsNull(storeOwnerDirect.getAppoint(storeID));
            Assert.IsFalse(founder.getAppoint(storeID).Contains(storeOwnerDirect));
            Assert.AreEqual(storeOwnerAppoint.GetType(), typeof(Member));
            Assert.AreEqual(0, storeOwnerAppoint.PenddingPermission.Keys.Count);
        }

        [TestMethod]
        public void RemoveStoreOwnerAndOnlyTheAppointSuccess()
        {
            //Arrange                                                                                                                                    
            storeOwnerDirect.AppointStoreOwner(storeID, storeOwnerAppoint);
            storeOwnerAppoint = founder.ReactToJobOffer(storeID, storeOwnerAppoint, true);
            //Act                                                                                                                                        
            storeOwnerDirect.RemoveStoreOwner(storeID, storeOwnerAppoint);
            //Assert                                                                                                                                                
            Assert.IsFalse(
                storeOwnerAppoint.hasPermissions(storeID,
                    new List<string> { "owner permissions" })); //appoint owner - no permissions           
            Assert.IsFalse(storeOwnerDirect.getAppoint(storeID).Contains(storeOwnerDirect));
            Assert.IsTrue(founder.getAppoint(storeID).Contains(storeOwnerDirect));
        }
        [TestMethod]                                                                              
        public void RemoveStoreOwnerNotDirectOne()                                    
        {                                                                                         
            //Arrange                                                                             
            storeOwnerDirect.AppointStoreOwner(storeID, storeOwnerAppoint);
            storeOwnerAppoint = founder.ReactToJobOffer(storeID, storeOwnerAppoint, true);
            //Assert                                                                              
            Assert.ThrowsException<Exception>(() =>                                         
                founder.RemoveStoreOwner(storeID, storeOwnerAppoint));                      
        }      
        #endregion   
        #region CleanUp
        [TestCleanup]
        public void CleanUp()
        {
            try
            {
                founder.removeAllDictOfStore(storeID);
                storeOwnerDirect.removeAllDictOfStore(storeID);
            }
            catch (Exception e)
            {
                
            }
            
        }
        #endregion
    }
}