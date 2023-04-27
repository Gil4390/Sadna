using System;

namespace SadnaExpress.DomainLayer
{
    public interface IObserver
    {
        void Update(string message ,Guid from);
        


    }
}