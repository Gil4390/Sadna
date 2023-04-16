using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.ServiceLayer;

namespace SadnaExpressTests.Acceptance_Tests
{
    [TestClass]
    public class EmployeeAT: TradingSystemAT
    {
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
        }

        [TestMethod]
        public void Test()
        {

        }

        [TestCleanup]
        public override void CleanUp()
        {
            base.CleanUp();
        }
    }
}