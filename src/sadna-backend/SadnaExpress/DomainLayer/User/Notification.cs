﻿using System;

namespace SadnaExpress.DomainLayer
{
    public class Notification
    {
        private Guid notificationID;
        private DateTime time;
        private Guid sentFrom;
        private string message;
        private Guid sentTo;
        private bool read;

        public Notification(DateTime time, Guid sentFrom, string message, Guid sentTo)
        {
            this.notificationID = Guid.NewGuid();
            this.time = time;
            this.sentFrom = sentFrom;
            this.message = message;
            this.sentTo = sentTo;
            this.Read = false;
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
        public Guid NotificationID { get => notificationID; set => notificationID = value; }
        public bool Read { get => read; set => read = value; }
    }
}