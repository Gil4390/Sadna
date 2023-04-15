using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SadnaExpressTests.Mocks;

namespace SadnaExpressTests.Unit_Tests
{
    [TestClass()]
    public class StoreFacadeUnitTest
    {
        private IStoreFacade _storeFacade;

        [TestInitialize]
        public void SetUp()
        {
            _storeFacade = new StoreFacade();
        }


        [TestMethod()]
        public void StoreFacade_HappyTest()
        {
            //Arrange
            

            //Act
           

            //Assert
           
        }

        [TestCleanup]
        public void CleanUp()
        {
            _storeFacade.CleanUp();
        }
    }
}
