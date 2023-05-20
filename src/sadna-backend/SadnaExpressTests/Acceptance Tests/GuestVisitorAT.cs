using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using SadnaExpress.ServiceLayer;
using SadnaExpressTests.Acceptance_Tests;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.DataLayer;

namespace SadnaExpressTests.Acceptance_Tests
{
    [TestClass]
    public class GuestVisitorAT: TradingSystemAT
    {
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
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
                return proxyBridge.Register(id, "RotemSela@gmail.com", "Rotem", "Sela", "ASK9876as!!");
            });
            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured);
            Assert.IsTrue(proxyBridge.GetMember(id).ErrorOccured);
        }

        [TestMethod]
        public void UserTryToRegisteTwice_BadTest()
        {
            Guid id = Guid.NewGuid();
            Task<Response> task = Task.Run(() =>
            {
                id = proxyBridge.Enter().Value;
                proxyBridge.Register(id, "shsh@gmail.com", "Rotem", "Sela", "ASK9876as!!");
                return proxyBridge.Register(id, "RotemSela@gmail.com", "Rotem", "Sela", "ASK9876as!!");
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured);  
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

        [TestMethod]
        [TestCategory("Concurrency")]
        public void Register2UsersAtTheSameTimeWithSameEmail_BadTest()
        {
            //task 2 run client with existing email
            Task<Response>[] clientTasks = new Task<Response>[] {
            Task.Run(() => RegisterClient("adam@gmail.com","Adam","Alon","57571!@#$aS")),
            Task.Run(() => RegisterClient("adam@gmail.com","Rotem","Sela","LkJ&^$187")),
            };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsTrue((clientTasks[0].Result.ErrorOccured == true && clientTasks[1].Result.ErrorOccured == false) ||
               (clientTasks[0].Result.ErrorOccured == false && clientTasks[1].Result.ErrorOccured == true));
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void Register2UsersAtTheSameTimeWithBadPass_BadTest()
        {
            //task 2 run client with existing email
            Task<Response>[] clientTasks = new Task<Response>[] {
            Task.Run(() => RegisterClient("adam@gmail.com","Adam","Alon","123")),
            Task.Run(() => RegisterClient("adam@gmail.com","Rotem","Sela","LkJ&^$187")),
            };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsTrue(clientTasks[0].Result.ErrorOccured == true && clientTasks[1].Result.ErrorOccured == false);
               
        }
        #endregion

        #region Login 1.4

        [TestMethod]
        public void UserLogin_HappyTest()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                return proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");
            });

            task.Wait();
            Assert.IsFalse(task.Result.ErrorOccured); //no error accured 
            Assert.IsNull(proxyBridge.GetMember(tempid).Value); //member with temp id does not exist
            Assert.IsTrue(proxyBridge.GetMember(tempid).ErrorOccured); //member with temp id does not exist
            Assert.IsTrue(proxyBridge.GetMember(systemManagerid).Value.UserId == systemManagerid);
        }

        [TestMethod]
        public void UserLoginWithoutEnter_BadTest()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                return proxyBridge.Login(systemManagerid, "RotemSela@gmail.com", "AS87654askj");
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 
            Assert.IsTrue(task.Result.Value==Guid.Empty); //Guid returns empty (default value)
        }

        [TestMethod]
        public void UserLoginBadPass_BadTest()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                return proxyBridge.Login(tempid, "RotemSela@gmail.com", "WrongPassword123");
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 
            Assert.IsTrue(task.Result.Value == Guid.Empty); //Guid returns empty (default value)
        }

        [TestMethod]
        public void UserLoginBadEmail_BadTest()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                return proxyBridge.Login(tempid, "RotemSalla@gmail.com", "AS87654askj");
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 
            Assert.IsTrue(task.Result.Value == Guid.Empty); //Guid returns empty (default value)
        }


        [TestMethod]
        public void UserLoginThatAlreadyLogin1_BadTest()
        {
            //Arrange
            proxyBridge.GetMember(systemManagerid).Value.LoggedIn = true;

            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                return proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 
            Assert.IsTrue(task.Result.Value == Guid.Empty); //Guid returns empty (default value)
        }

        [TestMethod]
        public void UserLoginThatAlreadyLogin2_BadTest()
        {
            Guid tempid = Guid.Empty;
            Task<ResponseT<Guid>> task = Task.Run(() => {
                tempid = proxyBridge.Enter().Value;
                proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");
                return proxyBridge.Login(tempid, "RotemSela@gmail.com", "AS87654askj");
            });

            task.Wait();
            Assert.IsTrue(task.Result.ErrorOccured); //error accured 
            Assert.IsTrue(task.Result.Value == Guid.Empty); //Guid returns empty (default value)
        }

        private ResponseT<Guid> LoginLogoutLogin(string email, string pass)
        {
            Guid tempid = proxyBridge.Enter().Value;
            proxyBridge.Login(tempid,email,pass);
            return proxyBridge.Logout(systemManagerid);
        }

        private ResponseT<Guid> RegisterAndLogin(string email, string pass, string firstName, string LastName)
        {
            Guid id = proxyBridge.Enter().Value;
            proxyBridge.Register(id, email, firstName, LastName, pass);
            return proxyBridge.Login(id, email, pass);
             
        }

        [TestMethod]
        [TestCategory("Concurrency")]
        public void UserLoginLogoutLoginAndOtherUserRegisterAndLogIn__HappyTest()
        {
            //task 2 run client with existing email
            Task<ResponseT<Guid>>[] clientTasks = new Task<ResponseT<Guid>>[] {
                Task.Run(() => RegisterAndLogin("adam@gmail.com","57571!@#$aS","Adam","Alon")),
                Task.Run(() => LoginLogoutLogin("RotemSela@gmail.com", "AS87654askj"))
            };

            // Wait for all clients to complete
            Task.WaitAll(clientTasks);

            Assert.IsFalse(clientTasks[0].Result.ErrorOccured);
            Assert.IsFalse(clientTasks[1].Result.ErrorOccured);
        }
        #endregion

        [TestCleanup]
        public override void CleanUp()
        {
            proxyBridge.GetMember(systemManagerid).Value.LoggedIn = false;
            DBHandler.Instance.TestMode();
        }
    }
}
