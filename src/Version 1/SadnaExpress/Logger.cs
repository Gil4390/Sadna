using SadnaExpress.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public void Info(string str)
        {
            if (instance == null)
            {
                instance = new Logger("LoggerOutput");
            }
            Console.WriteLine("inserting to log : " + str);
            using (logger = new StreamWriter(pathName, true))
            {
                //logger.WriteLine(System.DateTime.Now.ToString() + "   " + str);

                logger.WriteLine("Logger info|                  "+ " time : "+ System.DateTime.Now.ToString() + "   " + str);
                

                logger.Close();
            }
        }

        public void Info(User user, string str)
        {
            if (instance == null)
            {
                instance = new Logger("LoggerOutput");
            }
            Console.WriteLine("inserting to log : " + str);
            using (logger = new StreamWriter(pathName, true))
            {
                //logger.WriteLine(System.DateTime.Now.ToString() + "   userId: "+user.UserId+", " + str);

                logger.WriteLine("Logger info|                  user " + user.UserId + " time: " + System.DateTime.Now.ToString() + ", " + str);
                
                logger.Close();
            }
        }

    }
}
