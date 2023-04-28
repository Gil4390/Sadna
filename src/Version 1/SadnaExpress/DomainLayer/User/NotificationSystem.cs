using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NodaTime;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpress.DomainLayer
{
    public class NotificationSystem : ISubject
    {

        public List<Member> storeOwners;
        public NotificationSystem()
        {
            storeOwners = new List<Member>();
        }

        public List<Member> Obesevers { get => storeOwners; set => storeOwners = value; }


        public void NotifyObservers(string message, Guid userId)
        {
            foreach (Member member in storeOwners)
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

        public void RegisterObserver(Member observer)
        {
            storeOwners.Add(observer);
        }
        public void RegisterObservers(List<Member> observers)
        {
            foreach (Member member in observers)
            {
                storeOwners.Add(member);
            }
        }

        public void RemoveObserver(Member observer)
        {
            storeOwners.Remove(observer);

        }

  
    }
    
}