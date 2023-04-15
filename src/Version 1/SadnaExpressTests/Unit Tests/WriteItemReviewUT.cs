using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpressTests.Unit_Tests
{
    public class WriteItemReviewUT
    {
        #region Properties

        #endregion

        #region SetUp
        [TestInitialize]
        public void SetUp()
        {

            //2 users 
            //1 store
            //2 items
            //user1 buys item1 at store1
            //user2 - nothing
            
            //user1 tries to write review on item1 and succeeds
            //user2 tries to write review on item1 and fails because he doesn't have any orders in the shop
            
            //user1 tries to write review on item 2 and fails because he hasn't bought it before 
        }

        #endregion
        
        #region Tests

        [TestMethod]
        public void WriteItemReviewGood()
        {
            
        }
        
        [TestMethod]
        public void WriteItemReviewBad1()
        {
            
        }
        
        [TestMethod]
        public void WriteItemReviewBad2()
        {
            
        }

        #endregion

        #region CleanUp
        [TestCleanup]
        public void CleanUp()
        {
            _userFacade.CleanUp();
        }
        #endregion
    }
}