using System;
using System.Collections.Generic;
using SadnaExpress.API.WebClient.SignalR;
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
            if (storeOwners.ContainsKey(storeID))
            {
                foreach (Member member in storeOwners[storeID])
                {
                    if (member.UserId != userId) //we do not want to update the user about an operation he preformed by himself
                    {
                        Notification notification = new Notification(DateTime.Now, userId, message, member.UserId);
                        if (member.LoggedIn)
                            NotificationNotifier.GetInstance().SendNotification(member.UserId, message);
                        member.Update(notification);
                    }
                }
            }
        }

        public void NotifyObservers(List<Member> toNodify,Guid storeID, string message, Guid userId)
        {
           
            foreach (Member member in toNodify)
            {
                Notification notification = new Notification(DateTime.Now, userId, message, member.UserId);
                if (member.LoggedIn)
                    NotificationNotifier.GetInstance().SendNotification(member.UserId, message);
                member.Update(notification);        
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
        public void RemoveObservers(Guid storeID, List<Member> observers)
        {
            foreach (Member member in observers)
                RemoveObserver(storeID, member);
        }

        public void NotifyObserversInStores(ICollection<Guid> stores, string message, Guid userID)
        {
            foreach (Guid storeID in stores)
                NotifyObservers(storeID, message, userID);
        }
    }
    
}