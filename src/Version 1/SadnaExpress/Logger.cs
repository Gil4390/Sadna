

using SadnaExpress.DomainLayer.User;
using System;
using System.IO;

namespace SadnaExpress
{

    public class Logger
    {
        private StreamWriter logger;
        private string pathName;

        public Logger(string enter_path)
        {
            pathName = enter_path;
            if (!pathName.Equals(""))
                init_Logger(enter_path);
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


        public void WriteToLog(string str)
        {
            Console.WriteLine("inserting to log : " + str);
            using (this.logger = new StreamWriter(this.pathName, true))
            {
                logger.WriteLine(System.DateTime.Now.ToString() + "   " + str);
                logger.Close();
            }
        }
        /// <summary>
        /// ///////////////////////////////////////////////
        /// </summary>
        /// <param name="enter_path"></param>

        public void init_Logger(string enter_path)
        {
            //logger = File.Create(enter_path);
            //logger = File.Open(enter_path, FileMode.Open);
        }
        public void Info(string content)
        {
            if (!pathName.Equals(""))
                File.WriteAllText(pathName, "Logger info|                  " + content);
        }
        public void Info(User user, string content)
        {
            if (!pathName.Equals(""))
                File.WriteAllText(pathName, "Logger info|                  user " + user.UserId + ": " + content);
        }
        public void Debug(User user, string content)
        {
            if (!pathName.Equals(""))
                File.WriteAllText(pathName, "Logger debug|                  user " + user.UserId + ": " + content);
        }
        public void Error(User user, string content)
        {
            if (!pathName.Equals(""))
                File.WriteAllText(pathName, "Logger error|                  user " + user.UserId + ": " + content);
        }


    }
}