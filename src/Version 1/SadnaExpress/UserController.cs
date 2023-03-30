using ConsoleApp1.DomainLayer;

namespace SadnaExpress;

public class UserController
{
    private List<User> listOfUsers;
    
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
    
}