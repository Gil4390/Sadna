using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpressTests.Unit_Tests
{
    
    [TestClass()]
    public class StoreOwnerUT
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
        public void SetUp()
        {
            founderID = Guid.NewGuid();
            storeID = Guid.NewGuid();
            founder =
                new PromotedMember(founderID, "AssiAzar@gmail.com", "shay", "kresner", "ShaY1787%$%");
            founder.openNewStore(storeID);
            Member memberBeforeOwner = new Member(storeOwnerDirectID, "RotemSela@gmail.com", "noga", "schwartz",
                "ShaY1787%$%");
            storeOwnerDirect = founder.AppointStoreOwner(storeID, memberBeforeOwner);
            storeOwnerAppoint = new Member(storeOwnerAppointID, "Tal@gmail.com", "Tal", "Galmor",
                "ShaY1787%$%");
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
            PromotedMember promotedMember = founder.AppointStoreOwner(storeID, storeOwnerAppoint);
            //Assert
            Assert.IsTrue(founder.getAppoint(storeID).Contains(promotedMember));
            Assert.IsTrue(promotedMember.hasPermissions(storeID, new List<string>{"owner permissions"}));
        }
        
        [TestMethod]
        public void AppointStoreOwnerAgain()
        {
            //Arrange
            Member member = new Member(storeOwnerAppointID, "Tal@gmail.com", "Tal", "Galmor",
                "ShaY1787%$%");
            PromotedMember promotedMember = founder.AppointStoreOwner(storeID, member); //first time
            //Assert
            Assert.ThrowsException<Exception>(() =>
                founder.AppointStoreOwner(storeID, promotedMember));
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
            storeOwnerAppoint = storeOwnerDirect.AppointStoreOwner(storeID, storeOwnerAppoint);
            //Act
            founder.RemoveStoreOwner(storeID, storeOwnerDirect);
            //Assert
            Assert.IsFalse(storeOwnerDirect.hasPermissions(storeID, new List<string>{"owner permissions"})); //direct owner - no permissions
            Assert.IsFalse(storeOwnerAppoint.hasPermissions(storeID, new List<string>{"owner permissions"})); //appoint owner - no permissions
            Assert.IsNull(storeOwnerDirect.getAppoint(storeID));
            Assert.IsFalse(founder.getAppoint(storeID).Contains(storeOwnerDirect));
        }

        [TestMethod]
        public void RemoveStoreOwnerAndOnlyTheAppointSuccess()
        {
            //Arrange                                                                                                                                    
            storeOwnerAppoint = storeOwnerDirect.AppointStoreOwner(storeID, storeOwnerAppoint);
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
            storeOwnerAppoint = storeOwnerDirect.AppointStoreOwner(storeID, storeOwnerAppoint);   
            //Assert                                                                              
            Assert.ThrowsException<Exception>(() =>                                         
                founder.AppointStoreOwner(storeID, storeOwnerAppoint));                      
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