using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpressTests.Unit_Tests
{
    [TestClass()]
    public class UserFacadeUnitTest
    {
        private IUserFacade _userFacade;
        private ConcurrentDictionary<int, Member> members;
        
        public UserFacadeUnitTest() 
        {
            int userId=0;
            int memberid = 0;
            members = new ConcurrentDictionary<int, Member>();
            members.TryAdd(memberid, new Member(0,"shayk1934@gmail.com", "shay", "kresner", "123"));
            _userFacade = new UserFacade(new ConcurrentDictionary<int, User>(), members,userId,new PasswordHash());
        }

        

        [TestMethod()]
        public void UserFacadeInitializeTradingSystem_HappyTest() 
        {
            members[0].LoggedIn = true;
            Assert.IsTrue(_userFacade.InitializeTradingSystem(0));
        }

        [TestMethod()]
        public void UserFacadeInitializeTradingSystemUserIsNotLoggedIn_BadTest()
        {
            members[0].LoggedIn = false;
            Assert.ThrowsException<Exception>(() => _userFacade.InitializeTradingSystem(0));
        }

        [TestMethod()]
        public void UserFacadeInitializeTradingSystemUserNotExist_BadTest()
        {
            int badId = 8;
            Assert.ThrowsException<Exception>(() => _userFacade.InitializeTradingSystem(badId));
        }

        [TestCleanup]
        public void CleanUp()
        {
            _userFacade.CleanUp();
        }

    }
}
