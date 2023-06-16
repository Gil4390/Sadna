using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace SadnaExpressTests.Unit_Tests
{
    [TestClass()]
    public class UserUsageDataUT :TradingSystemUT
    {
        private UserUsageData userUsageData;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            userUsageData = UserUsageData.Instance;
        }

        [TestMethod()]
        public void AddGuestSuccess()
        {
            //Arrange
            User user = new User();

            //Act
            userUsageData.AddGuestVisit(user);

            //Assert
            Assert.IsTrue(userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[0] == 1);
            Visit guestVisit = userUsageData.UsersVisits.FirstOrDefault(item => item.UserID == user.UserId);
            Assert.IsTrue(guestVisit != null && guestVisit.Role == "guest");
        }

        [TestMethod()]
        public void AddMemberSuccess()
        {
            //Arrange
            User user = new User();
            Member member = new Member(Guid.NewGuid(), "bruno@gmail.com", "bruno", "biton", "!!@@ADGGsksjdbdjd");

            //Act
            userUsageData.AddGuestVisit(user);
            userUsageData.AddMemberVisit(user.UserId,member);

            //Assert
            Assert.IsTrue(userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[1] == 1 && userUsageData.GetUserUsageData(DateTime.Today, DateTime.Today)[0] == 0);
            Visit memberVisit = userUsageData.UsersVisits.FirstOrDefault(item => item.UserID == member.UserId);
            Assert.IsTrue(memberVisit != null && memberVisit.Role == "member");
        }


        [TestCleanup]
        public void CleanUp()
        {
            userUsageData.UsersVisits=new ConcurrentBag<Visit>();
        }
    }
}
