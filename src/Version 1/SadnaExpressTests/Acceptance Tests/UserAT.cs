using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using SadnaExpress.ServiceLayer;
using SadnaExpressTests.Acceptance_Tests;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpressTests.Integration_Tests
{
    [TestClass]
    public class UserAT: TradingSystemAT
    {
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
        }

        public void Register(Guid idx, string email, string pass)
        {
            Thread client1 = new Thread(() =>
            {
                proxyBridge.Enter();
                proxyBridge.Register(idx, email, " tal", " galmor", pass);
            });
            client1.Start();
            client1.Join();
        }
        public void Login(Guid idx, string email, string pass)
        {
            Thread client1 = new Thread(() =>
            {
                proxyBridge.Enter();
                proxyBridge.Login(idx, email, pass);
            });
            client1.Start();
            client1.Join();
        }

        #region Enter 1.1
        [TestMethod]
        public void GuestEnterToSystem_HappyTest()
        {
            //enter returns new user id, so will just check that new user is created with the id the system returned.
            Guid id = new Guid();
            Task<User> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.GetUser(id).Value;
             });
            Assert.IsTrue(task.Result!=null&&task.Result.UserId== id);
        }
        #endregion

        #region Exit 1.2
        [TestMethod]
        public void GuestExitTheSystem_HappyTest()
        {
            //exit the system with user id that already entered
            Task<ResponseT<User>> task = Task.Run(() => {
                proxyBridge.Exit(userid);
                return proxyBridge.GetUser(userid);
             });
            task.Wait();
            Assert.IsNull(task.Result.Value);
            Assert.IsTrue(task.Result.ErrorOccured);
        }

        [TestMethod]
        public void GuestExitTheSystem_BadTest()
        {
            //exit the system with user id that did not entered
            Task<Response> task = Task.Run(() => {
                return proxyBridge.Exit(Guid.NewGuid());
            });
            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured);
        }
        #endregion

        #region Register 1.3
        [TestMethod]
        public void UserRegister_HappyTest()
        { 
            Guid id = Guid.NewGuid();
            Task<Response> task = Task.Run(() => {
                id = proxyBridge.Enter().Value;
                return proxyBridge.Register(id, "tal.galmor@weka.io","tal", "galmor", "123AaC!@#");
            });
            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured);
            Assert.IsNotNull(proxyBridge.GetMember(id));
        }

        [TestMethod]
        public void UserRegisterwWithEmailThatAlreadyExist_BadTest()
        {
            //the mail of RotemSela@gmail.com is the mail of the system manager
            Guid id = Guid.NewGuid();
            Task<Response> task = Task.Run(() =>
            {
                id = proxyBridge.Enter().Value;
                return proxyBridge.Register(id, "RotemSela@gmail.com", "Rotem", "Sela", "123ABC!@#");
            });
            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured);
            Assert.IsTrue(proxyBridge.GetMember(id).ErrorOccured);
        }

        [TestMethod]
        public void UserRegisterwWithBadPassword1_BadTest()
        {
            //the mail of RotemSela@gmail.com is the mail of the system manager
            Guid id = Guid.NewGuid();
            Task<Response> task = Task.Run(() =>
            {
                id = proxyBridge.Enter().Value;
                return proxyBridge.Register(id, "tal.galmor@weka.io", "tal", "galmor", "123");
            });
            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured);
            Assert.IsTrue(proxyBridge.GetMember(id).ErrorOccured);
        }

        [TestMethod]
        public void UserRegisterwWithBadPassword2_BadTest()
        {
            //the mail of RotemSela@gmail.com is the mail of the system manager
            Guid id = Guid.NewGuid();
            Task<Response> task = Task.Run(() =>
            {
                id = proxyBridge.Enter().Value;
                return proxyBridge.Register(id, "tal.galmor@weka.io", "tal", "galmor", "");
            });
            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured);
            Assert.IsTrue(proxyBridge.GetMember(id).ErrorOccured);
        }

        [TestMethod]
        public void UserRegisterWithoutEnteredFirst_BadTest()
        {
            //the mail of RotemSela@gmail.com is the mail of the system manager
            Guid id = Guid.NewGuid();
            Task<Response> task = Task.Run(() =>
            {
                return proxyBridge.Register(id, "RotemSela@gmail.com", "Rotem", "Sela", "123");
            });
            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured);
            Assert.IsTrue(proxyBridge.GetMember(id).ErrorOccured);
        }

        protected Response RegisterClient(string email, string firstName, string LastName, string pass)
        {
            Guid id = proxyBridge.Enter().Value;
            return proxyBridge.Register(id, email, firstName, LastName, pass);
        }
    
        [TestMethod]
        [TestCategory("Concurrency")] 
        public void Register3UsersAtTheSameTime_HappyTest()
        {
            Task<Response>[] clientTasks = new Task<Response>[] {
            Task.Run(() => RegisterClient("adam@gmail.com","Adam","Alon","57571!@#$aS")),
            Task.Run(() => RegisterClient("Rami@gmail.com","Rami","Barak","LsJ&^$187")),
            Task.Run(() => RegisterClient("Yoni@gmail.com","Yoni","Kobi","87ASdFG%$"))
             };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsFalse(clientTasks[0].Result.ErrorOccured);
            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);
            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);  
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void Register3UsersAtTheSameTime_BadTest()
        {
            //task 2 run client with existing email
            Task<Response>[] clientTasks = new Task<Response>[] {
            Task.Run(() => RegisterClient("adam@gmail.com","Adam","Alon","57571!@#$aS")),
            Task.Run(() => RegisterClient("RotemSela@gmail.com","Rotem","Sela","LkJ&^$187")),
            Task.Run(() => RegisterClient("Yoni@gmail.com","Yoni","Kobi","87ASDsG%$"))
            };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsFalse(clientTasks[0].Result.ErrorOccured);
            Assert.IsTrue(clientTasks[1].Result.ErrorOccured);
            Assert.IsFalse(clientTasks[2].Result.ErrorOccured);
        }
        #endregion



        //[TestMethod]
        //public void Login_wrong_password()
        //{
        //    int count = 1;
        //    _server.activateAdmin();
        //    lock (_server)
        //    {
        //        Register(count, "tal.galmor@weka.io", "password");
        //        Login(count, "tal.galmor@weka.io", "password1");
        //    }
        //    Assert.IsFalse(_server.service.GetMembers()[count].LoggedIn);
        //    Assert.ThrowsException<Exception>(() => { _server.service.isLogin(count); });
        //}

        //[TestMethod]
        //public void Login_logout_and_login()
        //{
        //    int count = 1;
        //    _server.activateAdmin();
        //    lock (_server)
        //    {
        //        Register(count, "tal.galmor@weka.io", "password");
        //        Thread client1 = new Thread(() =>
        //        {
        //            _server.service.Enter();
        //            _server.service.Login(count, "tal.galmor@weka.io", "password");
        //            _server.service.Logout(count);
        //        });
        //        client1.Start();
        //        client1.Join();
        //    }
        //    Assert.IsFalse(_server.service.GetMembers()[count].LoggedIn);
        //    Assert.ThrowsException<Exception>(() => { _server.service.isLogin(count); });
        //}

        //[TestMethod]
        //public void Member_update_security()
        //{
        //    int count = 1;
        //    // _server.activateAdmin();
        //    Register(count, "tal.galmor@weka.io", "password");
        //    lock (_server)
        //    {
        //        Thread client2 = new Thread(() =>
        //        {
        //            _server.service.Login(count, "tal.galmor@weka.io", "password");
        //            _server.service.GetMembers()[count].SetSecurityQA("Cat name", "Titi");
        //        });

        //        client2.Start();
        //        client2.Join();
        //    }
        //    Assert.IsTrue(_server.service.GetMembers()[count].CheckSecurityQ("Cat name", "Titi"));
        //}
    }
}
