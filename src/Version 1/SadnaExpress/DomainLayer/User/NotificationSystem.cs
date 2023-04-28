using System;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpress.DomainLayer
{
    public class NotificationSystem : ISubject
    {
        private static NotificationSystem instance;
        private Dictionary<Guid , List<Member>> storeOwners;

        public Dictionary<Guid, List<Member>> StoreOwners
        {
            get => storeOwners;
            set => storeOwners = value;
        }

        private NotificationSystem()
        {
            storeOwners = new Dictionary<Guid, List<Member>>();
        }

        public static NotificationSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NotificationSystem();
                }
                return instance;
            }
        }

        public void NotifyObservers(Guid storeID , string message, Guid userId)
        {
            List<Member> owners  = storeOwners[storeID];
            foreach (Member member in owners)
            {
                Notification notification = new Notification(DateTime.Now, userId, message, member.UserId);
                if (member.LoggedIn)
                    member.showMessage();
                else
                    member.addToAwaitingNotification(notification);
            }
        }
        
        public void update(Member member, string message, Guid userId)
        { 
            Notification notification = new Notification(DateTime.Now, userId, message, member.UserId);
            if (member.LoggedIn)
                member.showMessage();
            else
                member.addToAwaitingNotification(notification);
        }
        public void updateMany(List<Member> members, string message, Guid userId)
        {
            foreach (Member member in members)
            {
                Notification notification = new Notification(DateTime.Now, userId, message, member.UserId);
                if (member.LoggedIn)
                    member.showMessage();
                else
                    member.addToAwaitingNotification(notification);
            }
        }
        public void RegisterObserver(Guid storeID , Member observer)
        {
            List<Member> members;
            if (storeOwners.TryGetValue(storeID, out members))
            {
                members.Add(observer);
            }
            else
            {
                members = new List<Member> { observer };
                storeOwners.Add(storeID, members);
            }
            
        }

        public void RemoveObserver(Guid storeID, Member observer)
        {
            if (storeOwners.TryGetValue(storeID, out List<Member> observers))
            {
                 observers.Remove(observer);
            }
          
        }

  
    }
    
}