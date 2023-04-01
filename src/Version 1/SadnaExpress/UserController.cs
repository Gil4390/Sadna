using ConsoleApp1.DomainLayer;

namespace SadnaExpress;

public class UserController
{
    private List<User> listOfUsers;
    private Dictionary<string, string> members = new Dictionary<string, string>();
    public static PasswordHash passwordHash = new PasswordHash();
    
    public UserController()
    {
        listOfUsers = new List<User>();
    }

    public void addUser(User user)
    {
        listOfUsers.Add(user);
    }
    public void removeUser(User user)
    {
        listOfUsers.Remove(user);
    }
    public bool registerUser(Guest guest , string username , string password)
    {
        bool success = false;
        members.Add(username,passwordHash.HashPassword(password));
        if (success)
        {
            listOfUsers.Remove(guest);
            Member member = new Member();
            listOfUsers.Add(member);
        }
        return success;
    }

    public bool authentication(string username, string password)
    {
        return members.ContainsValue(username) && passwordHash.ValidatePassword(password , members[username]);
    }
    
}