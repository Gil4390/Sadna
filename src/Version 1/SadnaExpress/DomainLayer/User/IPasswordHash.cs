namespace SadnaExpress.DomainLayer.User
{
    public interface IPasswordHash
    {
        string Hash(string password);
        bool Rehash(string password, string correctHash);
        string Validate(string password);

    }
}