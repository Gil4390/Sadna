using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.API.SignalR
{
    public class SystemActivityNotifier
    {
        private static SystemActivityNotifier instance { get; set; }

        private bool testMood;
        public bool TestMood { get => testMood; set => testMood = value; }


        private SystemActivityNotifier() { }

        public void SystemActivityUpdated()
        {
            if (!testMood)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<SystemActivityHub>();
                context.Clients.All.SystemActivityUpdated();
            }
        }

        public static SystemActivityNotifier GetInstance()
        {
            if (instance == null)
            {
                instance = new SystemActivityNotifier();
            }
            return instance;
        }
    }
}
