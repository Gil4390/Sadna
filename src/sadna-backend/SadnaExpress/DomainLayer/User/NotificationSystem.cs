using System;
using System.Collections.Generic;
using SadnaExpress.API.SignalR;
using SadnaExpress.DataLayer;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.SModels;

namespace SadnaExpress.DomainLayer
{
    public class NotificationSystem : ISubject
    {
        private static NotificationSystem instance;
        private Dictionary<Guid , List<Member>> notificationOfficials;
        public IUserFacade userFacade { get; set; }
       
        public Dictionary<Guid, List<Member>> NotificationOfficials
        {
            get => notificationOfficials;
            set => notificationOfficials = value;
        }

        private NotificationSystem()
        {
            notificationOfficials = new Dictionary<Guid, List<Member>>();
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

        public void NotifyObservers(Guid storeID , string message, Guid userId, DatabaseContext db = null)
        {
            if (notificationOfficials.ContainsKey(storeID))
            {
                foreach (Member member in notificationOfficials[storeID])
                {
                    if (member.UserId != userId) //we do not want to update the user about an operation he preformed by himself
                    {
                        Notification notification = new Notification(DateTime.Now, userId, message, member.UserId);
                        Member realMember = userFacade.GetMember(member.UserId);
                        if (realMember.LoggedIn)
                            NotificationNotifier.GetInstance().SendNotification(member.UserId, message);
                        realMember.Update(notification, db);
                    }
                }
            }
        }

        public void NotifyObservers(List<Member> toNodify, Guid storeID, string message, Guid userId)
        {
            foreach (Member member in toNodify)
            {
                Notification notification = new Notification(DateTime.Now, userId, message, member.UserId);
                Member realMember = userFacade.GetMember(member.UserId);
                if (realMember.LoggedIn)
                    NotificationNotifier.GetInstance().SendNotification(member.UserId, message);
                realMember.Update(notification);        
            }
        }

        public void NotifyObserver(User.User toNodify, Guid storeID, string message)
        {
            if (toNodify.GetType() == typeof(Member))
            {
                Notification notification = new Notification(DateTime.Now, message, toNodify.UserId);
                Member realMember = userFacade.GetMember(toNodify.UserId);
                if (realMember.LoggedIn)
                    NotificationNotifier.GetInstance().SendNotification(toNodify.UserId, message);
                realMember.Update(notification);        
            }
            else
            {
                NotificationNotifier.GetInstance().SendNotification(toNodify.UserId, message);
            }
        }

        public void RegisterObserver(Guid storeID , Member observer)
        {
            List<Member> members;
            if (notificationOfficials.TryGetValue(storeID, out members))
            {
                members.Add(observer);
            }
            else
            {
                members = new List<Member> { observer };
                notificationOfficials.Add(storeID, members);
            }
            
        }

        public void RemoveObserver(Guid storeID, Member observer)
        {
            if (notificationOfficials.TryGetValue(storeID, out List<Member> observers))
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

        public void LoadNotificationOfficialsFromDB()
        {
            NotificationOfficials = DBHandler.Instance.LoadNotificationOfficialsFromDB();
        }
    }
    
}