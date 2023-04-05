using System;
using System.IO;
using System.Threading;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using SadnaExpress.Services;

namespace SadnaExpress
{

    //static class Logger
    //{ 
    //    static StreamWriter logger;
    //    static string pathName;
        
    //    public static void Init (string enter_path)
    //    {
    //        pathName = enter_path;
    //        string directory_path = @"c:\SadnaExpress Log";
    //        try
    //        {
    //            if (!Directory.Exists(directory_path))
    //                Directory.CreateDirectory(directory_path);

    //            using (logger = new StreamWriter(enter_path, true))
    //            {
    //                if (File.Exists(enter_path))
    //                {
    //                    logger.WriteLine("!************** Program Started At " + System.DateTime.Now.ToString() + " **************");
    //                }
    //                logger.Close();
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine(ex.ToString());
    //        }
    //    }


    //    public static void WriteToLog(string str)
    //    {
    //        Console.WriteLine("inserting to log : " + str);
    //        using (logger = new StreamWriter(pathName, true))
    //        {
    //            logger.WriteLine(System.DateTime.Now.ToString() + "   " + str);
    //            logger.Close();
    //        }
    //    }
    //    public static void Info(string content)
    //    {
    //        if (!pathName.Equals(""))
    //            File.WriteAllText(pathName, "Logger info|                  " + content);
    //    }
    //    public static void Info(User user, string content)
    //    {
    //        if (!pathName.Equals(""))
    //            File.WriteAllText(pathName, "Logger info|                  user " + user.UserId + ": " + content);
    //    }
    //    public static void Debug(User user, string content)
    //    {
    //        if (!pathName.Equals(""))
    //            File.WriteAllText(pathName, "Logger debug|                  user " + user.UserId + ": " + content);
    //    }
    //    public static void Error(User user, string content)
    //    {
    //        if (!pathName.Equals(""))
    //            File.WriteAllText(pathName, "Logger error|                  user " + user.UserId + ": " + content);
    //    }
    //}
    internal class Program
    {
        public static void Main(string[] args)
        {
            //Logger.Init("_path");
            //Console.WriteLine("Server Started!");
            //Console.WriteLine("In order to login enter: Admin Admin");
            //Thread serverThread = new Thread(delegate ()
            //{
            //    Server myserver = new Server("127.0.0.1", 10011, new Mock_);
            //});
            //serverThread.Start();

            Tradingsystem tradingsystem = new Tradingsystem(new Mock_SupplierService(), new Mock_PaymentService());

            tradingsystem.register(0, "first123@gmail.com", "first", "last", "First1234");
            int resultLogin = tradingsystem.login(0, "first123@gmail.com", "First1234");

            bool resultS = tradingsystem.checkSupplierConnection();
            bool resultP = tradingsystem.checkPaymentConnection();


        }


        private class Mock_SupplierService : ISupplierService
        {
            int shipmentNum = 0;
            bool isConnected = false;

            public Mock_SupplierService()
            {
                isConnected = true;
            }

            public virtual void CancelOrder(string orderNum)
            {

            }

            public virtual bool Connect()
            {
                return isConnected;
            }


            public virtual string ShipOrder(string address)
            {
                shipmentNum++;
                return "test " + shipmentNum;
            }


            public virtual bool ValidateAddress(string address)
            {
                return true;
            }
        }


        private class Mock_PaymentService : IPaymentService
        {
            bool isConnected = false;

            public Mock_PaymentService()
            {
                isConnected = true;
            }

            public virtual bool Connect()
            {
                return isConnected;
            }


            public virtual void Pay(double amount, string payment)
            {

            }

            public virtual bool ValidatePayment(string payment)
            {
                return true;
            }
        }


    }
}