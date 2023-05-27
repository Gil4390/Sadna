using System;
using System.ComponentModel.DataAnnotations;

namespace SadnaExpress.DomainLayer
{
    public class Notification
    {
        
        private Guid notificationID;
        [Key]
        public Guid NotificationID { get => notificationID; set => notificationID = value; }

        private DateTime time;
        public DateTime Time { get => time; set => time = value; }

        private Guid sentFrom;
        public Guid SentFrom { get => sentFrom; set => sentFrom = value; }

        private string message;
        public string Message { get => message; set => message = value; }

        private Guid sentTo;
        public Guid SentTo { get => sentTo; set => sentTo = value; }

        private bool read;
        public bool Read { get => read; set => read = value; }

        public Notification(DateTime time, Guid sentFrom, string message, Guid sentTo)
        {
            this.notificationID = Guid.NewGuid();
            this.time = time;
            this.sentFrom = sentFrom;
            this.message = message;
            this.sentTo = sentTo;
            this.Read = false;
        }

        public Notification(DateTime time, string message, Guid sentTo)
        {
            this.notificationID = Guid.NewGuid();
            this.time = time;
            this.sentFrom = default;
            this.message = message;
            this.sentTo = sentTo;
            this.Read = false;
        }

        public Notification()
        {
        }
    }
}
