using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using SadnaExpress.DomainLayer.Store;

namespace SadnaExpress.DomainLayer.User
{
    public class Member : User , IObserver
    {
        protected string email;
        public string Email {get => email; set => email = value;}
        protected string firstName;
        public string FirstName { get => firstName; set => firstName = value;}
            
        protected string lastName;
        public string LastName { get => lastName; set => lastName = value;}
        protected string password;
        public string Password { get => password; set => password = value; }
        public List<Notification> AwaitingNotification { get => awaitingNotification; set => awaitingNotification = value; }

        private bool loggedIn;
        public bool LoggedIn { get => loggedIn; set => loggedIn = value; }

        private Dictionary<string , string> securityQuestions;
        protected List<Notification> awaitingNotification;
        protected NotificationSystem notificationSystem = NotificationSystem.Instance;



        public Member(Guid id, string memail, string mfirstName, string mlastLame, string mpassword): base ()
        {
            userId = id;
            email = memail;
            firstName = mfirstName;
            lastName = mlastLame;
            password = mpassword;
            LoggedIn = false;
            securityQuestions = new Dictionary<string, string>();
            awaitingNotification = new List<Notification>();
        }

        public Member(PromotedMember promotedMember)
        {
            userId = promotedMember.userId;
            email = promotedMember.email;
            firstName = promotedMember.FirstName;
            lastName = promotedMember.lastName;
            password = promotedMember.Password;
            LoggedIn = promotedMember.LoggedIn;
            shoppingCart = promotedMember.ShoppingCart;
            securityQuestions = promotedMember.SecurityQuestions;
            awaitingNotification = promotedMember.awaitingNotification;
        }
        public void addToAwaitingNotification(Notification notification) {
            this.awaitingNotification.Add(notification);
        }
        
        
        public PromotedMember promoteToMember() {
            return new PromotedMember(UserId, email, firstName, lastName, password, shoppingCart, loggedIn);
        }
        public virtual PromotedMember openNewStore(Guid storeID)
        {
            PromotedMember founder = promoteToMember();
            founder.createFounder(storeID);
            founder.LoggedIn = true;
            return founder;
        }
        public void SetSecurityQA(string q , string a)
        {
            securityQuestions.Add(q,a);
        }
        public string GetSecurityQ()
        {
            return securityQuestions.ElementAt(new Random().Next(0, securityQuestions.Count)).Value;
        }
        public bool CheckSecurityQ(string q , string a)
        {
            return securityQuestions[q] == a;
        }

        public Dictionary<string, string> SecurityQuestions
        {
            get => securityQuestions;
        }
        

        public void MarkNotificationAsRead(Guid notificationID)
        {
            foreach (Notification notification in awaitingNotification)
            {
                if (notification.NotificationID == notificationID)
                {
                    notification.Read = true;
                }
            }
        }


        public void Update(Notification notification)
        {
            awaitingNotification.Add(notification);
        }
    }
}