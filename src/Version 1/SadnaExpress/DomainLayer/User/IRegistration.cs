namespace SadnaExpress.DomainLayer.User
{
    public interface IRegistration
    {
        bool ValidateStrongPassword(string pass);
        bool ValidateEmail(string email);
    }
}