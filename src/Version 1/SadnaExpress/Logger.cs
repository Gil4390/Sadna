using ConsoleApp1.DomainLayer;

namespace SadnaExpress;

public class Logger
{
    private FileStream logger;
    private string pathName;

    public Logger(string enter_path)
    {
        pathName = enter_path;
        if (!pathName.Equals(""))
            init_Logger(enter_path);
    }

    public void init_Logger(string enter_path)
    {
        logger = File.Create(enter_path);
        logger = File.Open(enter_path, FileMode.Open);
    }
    public void Info(string content)
    {
        if (!pathName.Equals(""))
            File.WriteAllText(pathName, "Logger info|                  "+content);
    }
    public void Info(User user , string content)
    {
        if (!pathName.Equals(""))
            File.WriteAllText(pathName, "Logger info|                  user "+user.UserId+ ": "+content);
    }
    public void Debug(User user , string content)
    {
        if (!pathName.Equals(""))
            File.WriteAllText(pathName, "Logger debug|                  user "+user.UserId+ ": "+content);
    }
    public void Error(User user , string content)
    {
        if (!pathName.Equals(""))
            File.WriteAllText(pathName, "Logger error|                  user "+user.UserId+ ": "+content);
    }
    
    
}