using System;

namespace SadnaExpress.DomainLayer
{
    public class Notification
    {
        private DateTime time;
        private Guid sentFrom;
        private string message;
        private Guid sentTo;

        public Notification(DateTime time, Guid sentFrom, string message, Guid sentTo)
        {
            this.time = time;
            this.sentFrom = sentFrom;
            this.message = message;
            this.sentTo = sentTo;
        }

        public Guid SentTo
        {
            get => sentTo;
            set => sentTo = value;
        }

        public string Message
        {
            get => message;
            set => message = value;
        }

        public Guid SentFrom
        {
            get => sentFrom;
            set => sentFrom = value;
        }

        public DateTime Time
        {
            get => time;
            set => time = value;
        }
    }
}
