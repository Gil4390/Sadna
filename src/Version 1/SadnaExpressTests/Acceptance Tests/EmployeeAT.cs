using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.ServiceLayer;

namespace SadnaExpressTests.Integration_Tests
{
    [TestClass]
    public class EmployeeAT
    {
        private Server _server;
        
        [TestInitialize]
        public void SetUp()
        {
            _server = new Server();
            _server.activateAdmin();
        }
        public void Register(int idx , string email , string pass)
        {
            Thread client1 = new Thread(() =>
            {
                _server.service.Enter();
                _server.service.Register(idx, email, " tal", " galmor", pass);
            });
            client1.Start();
            client1.Join();
        }
        [TestMethod]
        public void Appoint_store_appoint()
        {
            
        }
    }
}