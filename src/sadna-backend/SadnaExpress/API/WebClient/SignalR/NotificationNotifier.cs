using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.API.WebClient.SignalR
{
    public class NotificationNotifier
    {
        private static NotificationNotifier instance { get; set; }

        private NotificationNotifier() { }

        public void SendNotification(Guid memberId, string message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.All.SendNotification(memberId, message);
        }

        public static NotificationNotifier GetInstance()
        {
            if (instance == null)
            {
                instance = new NotificationNotifier();
            }
            return instance;
        }
    }
}
