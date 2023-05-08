using System;
using System.IO;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpress
{
    public class Logger
    {
       private static StreamWriter logger;
       private static string pathName;

        private static string normalPathName;
        private static string testsPathName;

        private static readonly object lockThreads = new object();  // only add this if this class needs to be thread safe

        private static Logger instance = null;

        public static Logger Instance 
        {
            get
            {
               lock(lockThreads) {
                    if (instance == null)
                    {
                        instance = new Logger("LoggerOutput");
                    }
                    return instance;
               }
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
                // saving normal path
                normalPathName = pathName;

                // saving test path
                testsPathName = directory_path + "\\" + "TestLoggerOutput" + ".txt";

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public void SwitchOutputFile()
        {
            // switch from test mode to normal 
            lock (this)
            {
                //
                // first check if test file is created
                // here opens tests output File if it is not created already
                if (File.Exists(testsPathName).Equals(false))
                {
                    using (logger = new StreamWriter(testsPathName, true))
                    {
                        if (File.Exists(testsPathName))
                        {
                            logger.WriteLine("!************** Program Tests Started At " + System.DateTime.Now.ToString() + " **************!");
                        }
                        logger.Close();
                    }
                }

                if (pathName.Equals(normalPathName))
                    pathName = testsPathName;
                else
                    pathName = normalPathName;
            }
        }

        public void Info(string str)
        {
            lock (this)
            {
                using (logger = new StreamWriter(pathName, true))
                {
                    logger.WriteLine(System.DateTime.Now.ToString() + "|Logger info|                  " + str);
                    logger.Close();
                }
            }
        }

        public void Info(User user, string str)
        {
            lock (this)
            {
                using (logger = new StreamWriter(pathName, true))
                {
                    logger.WriteLine(System.DateTime.Now.ToString() + "|Logger info|                  user " +
                                     user.UserId + ", " + str);
                    logger.Close();
                }
            }
        }

        public void Info(Guid userid, string str)
        {
            lock (this)
            {
                using (logger = new StreamWriter(pathName, true))
                {
                    logger.WriteLine(System.DateTime.Now.ToString() + "|Logger info|                  user " +
                                     userid + ", " + str);
                    logger.Close();
                }
            }
        }

        public void Error(string str)
        {
            lock (this)
            {
                using (logger = new StreamWriter(pathName, true))
                {
                    logger.WriteLine(System.DateTime.Now.ToString() + "|Logger error|                 " + str);
                    logger.Close();
                }
            }
        }
        public void Error(User user, string str)
        {
            lock (this)
            {
                using (logger = new StreamWriter(pathName, true))
                {
                    logger.WriteLine(System.DateTime.Now.ToString() + "|Logger error|                 user " +
                                     user.UserId + ", " + str);
                    logger.Close();
                }
            }
        }

        public void Error(Guid userid, string str)
        {
            lock (this)
            {
                using (logger = new StreamWriter(pathName, true))
                {
                    logger.WriteLine(System.DateTime.Now.ToString() + "|Logger error|                 user " +
                                     userid + ", " + str);
                    logger.Close();
                }
            }
        }
    }
}