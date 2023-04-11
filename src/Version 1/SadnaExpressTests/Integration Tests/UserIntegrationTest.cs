using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using SadnaExpress.ServiceLayer;

namespace SadnaExpressTests.Integration_Tests
{
    [TestClass]
    public class UserIntegrationTest
    {
        private Server _server;
        [TestInitialize]
        public void SetUp()
        {
            _server = new Server();
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
        public void Login(int idx , string email , string pass)
        {
            Thread client1 = new Thread(() =>
            {
                _server.service.Enter();
                _server.service.Login(idx, email, pass);
            });
            client1.Start();
            client1.Join();
        }


        [TestMethod]
        public void Check_init_system()
        {
            int count = 1;
            lock (_server)
                _server.activateAdmin();
            Assert.IsTrue(_server.tradingSystemOpen && _server.service.GetMembers().ContainsKey(0));
        }
        [TestMethod]
        public void User_register_first_time()
        {
            int count = 1;
            _server.activateAdmin();
            Register(count,"tal.galmor@weka.io" , "password");
            
            Assert.IsTrue(_server.service.GetMembers().Count==2 &&
                          _server.service.GetMembers()[1].Email=="tal.galmor@weka.io");
        }
        [TestMethod]
        public void Two_users_try_register()
        {
            int count = 1;
            _server.activateAdmin();
            Register(count,"tal.galmor@weka.io","password");
            count++;
            Register(count,"tal.galmor@weka.io","drowssap");
            
            Assert.IsTrue(_server.service.GetMembers().Count==2 &&
                          _server.service.GetMembers()[1].Email=="tal.galmor@weka.io");
        }
        [TestMethod]
        public void Login_wrong_password()
        {
            int count = 1;
            _server.activateAdmin();
            lock (_server)
            {
                Register(count,"tal.galmor@weka.io","password");
                Login(count,"tal.galmor@weka.io","password1");
            }
            Assert.IsFalse(_server.service.GetMembers()[count].LoggedIn);
            Assert.ThrowsException<Exception>(() => { _server.service.isLogin(count);});
        }
        [TestMethod]
        public void Login_logout_and_login()
        {
            int count = 1;
            _server.activateAdmin();
            lock (_server)
            {
                Register(count,"tal.galmor@weka.io","password");
                Thread client1 = new Thread(() =>
                {
                    _server.service.Enter();
                    _server.service.Login(count,"tal.galmor@weka.io","password");
                    _server.service.Logout(count);
                });
                client1.Start();
                client1.Join();
            }
            Assert.IsFalse(_server.service.GetMembers()[count].LoggedIn);
            Assert.ThrowsException<Exception>(() => { _server.service.isLogin(count);});
        }
        [TestMethod]
        public void Member_update_security()
        {
            int count = 1;
            _server.activateAdmin();
            Register(count, "tal.galmor@weka.io",  "password");
            lock (_server)
            {
                Thread client2 = new Thread(() =>
                {
                    _server.service.Login(count, "tal.galmor@weka.io", "password");
                    _server.service.GetMembers()[count].SetSecurityQA("Cat name","Titi");
                });

                client2.Start();
                client2.Join();
            }
            Assert.IsTrue(_server.service.GetMembers()[count].CheckSecurityQ("Cat name","Titi"));
        }
    }
}
