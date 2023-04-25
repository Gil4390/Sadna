namespace SadnaExpress.DomainLayer
{
    public interface Subject
    {
        void RegisterObserver(Observer observer);
        void RemoveObserver(Observer observer);
        void NotifyObservers();
    }
}