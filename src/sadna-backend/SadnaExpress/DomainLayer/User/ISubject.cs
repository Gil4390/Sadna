using System;
using SadnaExpress.DataLayer;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpress.DomainLayer
{
    public interface ISubject
    {
        void RegisterObserver(Guid storeID ,Member observer);
        void RemoveObserver(Guid storeID ,Member observer);
        void NotifyObservers(Guid storeID ,string message, Guid userId, DatabaseContext db=null);
    }
}