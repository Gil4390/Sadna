using SadnaExpress.API.SignalR;
using SadnaExpress.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpressTests.Unit_Tests
{
    public class TradingSystemUT
    {
        public virtual void SetUp()
        {
            DBHandler.Instance.TestMood = true;
            DatabaseContextFactory.TestMode = true;
            SystemActivityNotifier.GetInstance().TestMood = true;
            NotificationNotifier.GetInstance().TestMood = true;
        }
    }
}
