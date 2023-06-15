using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mime;
using Newtonsoft.Json;
using SadnaExpress.DataLayer;
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

        [NotMapped]
        public List<Notification> AwaitingNotification { get => awaitingNotification; set => awaitingNotification = value; }

        private bool loggedIn;
        public bool LoggedIn { get => loggedIn; set => loggedIn = value; }
        

        // todo in database
        [NotMapped]
        public List<Notification> awaitingNotification { get; set; }

        public Member(Guid id, string memail, string mfirstName, string mlastLame, string mpassword): base ()
        {
            userId = id;
            email = memail;
            firstName = mfirstName;
            lastName = mlastLame;
            password = mpassword;
            LoggedIn = false;
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
            awaitingNotification = promotedMember.awaitingNotification;
        }

        public void deepCopy(User user)
        {
            shoppingCart.AddUserShoppingCart(user.ShoppingCart);
            foreach (Bid bid in user.Bids)
            {
                bid.User = this;
                Bids.Add(bid);
                bid.AddToDB();
            }
        }
        
        public virtual PromotedMember promoteToMember() {
            return new PromotedMember(UserId, email, firstName, lastName, password, shoppingCart, loggedIn);
        }
        public virtual PromotedMember openNewStore(Guid storeID)
        {
            PromotedMember founder = promoteToMember();
            founder.createFounder(storeID);
            founder.LoggedIn = true;
            return founder;
        }
        
        public void MarkNotificationAsRead(Guid notificationID)
        {
            foreach (Notification notification in awaitingNotification)
            {
                if (notification.NotificationID == notificationID)
                {
                    notification.Read = true;
                    DBHandler.Instance.MarkNotificatinAsRead(notification);
                }
            }
        }


        public void Update(Notification notification, DatabaseContext db=null)
        {
            awaitingNotification.Add(notification);

            DBHandler.Instance.AddNotification(notification, db);
        }


        
        public Member()
        {

        }

        public override string GetRole()
        {
            return "Member";
        }

    }
}