using System;
using System.IO;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpress
{
    public class Logger
    {
        private static StreamWriter logger;
        private static string pathName;

        //private static readonly object lockThreads = new object();  // only add this if this class needs to be thread safe

        private static Logger instance = null;

        public static Logger Instance 
        {
            get
            {
                // if lock(lockThreads) {
                if (instance == null)
                {
                    instance = new Logger("LoggerOutput");
                }
                return instance;
                //}
            }
        }

        private Logger(string enter_path)
        {
            string directory_path = @"c:\SadnaExpress Log";

            pathName = directory_path + "\\" + enter_path + ".txt";
            try
            {
                if (!Directory.Exists(directory_path))
                    Directory.CreateDirectory(directory_path);

                using (logger = new StreamWriter(pathName, true))
                {
                    if (File.Exists(pathName))
                    {
                        logger.WriteLine("!************** Program Started At " + System.DateTime.Now.ToString() + " **************!");
                    }
                    logger.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void init()
        {
            if (instance == null)
            {
                instance = new Logger("LoggerOutput");
            }
        }

        public void Info(string str)
        {
            using (logger = new StreamWriter(pathName, true))
            {
                logger.WriteLine(System.DateTime.Now.ToString() + "|Logger info|                   " + str);
                logger.Close();
            }
        }

        public void Info(User user, string str)
        {
            init();
            using (logger = new StreamWriter(pathName, true))
            {
                logger.WriteLine(System.DateTime.Now.ToString() + "|Logger info|                  user " + user.UserId + ", " + str);
                logger.Close();
            }
        }
        public void Error(string str)
        {
            init();
            using (logger = new StreamWriter(pathName, true))
            {
                logger.WriteLine(System.DateTime.Now.ToString() + "|Logger error|                 " + str);
                logger.Close();
            }
        }
        public void Error(User user, string str)
        {
            init();
            using (logger = new StreamWriter(pathName, true))
            {
                logger.WriteLine(System.DateTime.Now.ToString() + "|Logger error|                 user " + user.UserId + ", " + str);
                logger.Close();
            }
        }
    }
}