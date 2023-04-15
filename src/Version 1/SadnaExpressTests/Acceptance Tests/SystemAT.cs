using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpressTests.Acceptance_Tests
{
    public class SystemAT : TradingSystemAT
    {

        [TestInitialize]
        public override void SetUp()
        {
            
        }


        [TestMethod()]
        public void InitTradingSystem_UserIsntSystemManager_BadTest()
        {
            //Arrange


            //Act


            //Assert

        }

        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
        }
    }
}
