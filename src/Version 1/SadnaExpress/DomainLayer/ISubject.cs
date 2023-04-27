using System;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpress.DomainLayer
{
    public interface ISubject
    {
        void RegisterObserver(Member observer);
        void RemoveObserver(Member observer);
        void NotifyObservers(string message, Guid userId);
    }
}