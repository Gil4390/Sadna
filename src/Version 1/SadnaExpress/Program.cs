using System;
using System.IO;
using System.Threading;
using SadnaExpress.ServiceLayer;

namespace SadnaExpress
{

    static class Logger
    { 
        static StreamWriter logger;
        static string pathName;
        
        public static void Init (string enter_path)
        {
            pathName = enter_path;
            string directory_path = @"c:\SadnaExpress Log";
            try
            {
                if (!Directory.Exists(directory_path))
                    Directory.CreateDirectory(directory_path);

                using (logger = new StreamWriter(enter_path, true))
                {
                    if (File.Exists(enter_path))
                    {
                        logger.WriteLine("!************** Program Started At " + System.DateTime.Now.ToString() + " **************");
                    }
                    logger.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public static void WriteToLog(string str)
        {
            Console.WriteLine("inserting to log : " + str);
            using (logger = new StreamWriter(pathName, true))
            {
                logger.WriteLine(System.DateTime.Now.ToString() + "   " + str);
                logger.Close();
            }
        }
        public static void Info(string content)
        {
            if (!pathName.Equals(""))
                File.WriteAllText(pathName, "Logger info|                  " + content);
        }
        // public static void Info(User user, string content)
        // {
        //     if (!pathName.Equals(""))
        //         File.WriteAllText(pathName, "Logger info|                  user " + user.UserId + ": " + content);
        // }
        // public static void Debug(User user, string content)
        // {
        //     if (!pathName.Equals(""))
        //         File.WriteAllText(pathName, "Logger debug|                  user " + user.UserId + ": " + content);
        // }
        // public static void Error(User user, string content)
        // {
        //     if (!pathName.Equals(""))
        //         File.WriteAllText(pathName, "Logger error|                  user " + user.UserId + ": " + content);
        // }
    }
    internal class Program
    {
        public static void Main(string[] args)
        {
            //Logger.Init("_path");
            Console.WriteLine("Start!");
            Thread serverThread = new Thread(delegate ()
            {
                Server myserver = new Server("127.0.0.1", 10011);
            });
            serverThread.Start();
        }
    }
}