using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.Owin.Hosting;
using SadnaExpress.API;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using SadnaExpress.Services;
using SadnaExpress.API.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using System.Data.SqlTypes;
using SadnaExpress.DataLayer;

namespace SadnaExpress
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InitAppSettings();
            State state = State.Instance;
            string flag = ApplicationOptions.StateFileConfig;
            if (flag.Equals("data"))
            {
                DBHandler.Instance.CleanDB();
                try
                {
                    state.stateFile(flag + ".json");
                    state.checkFile0();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    DBHandler.Instance.CleanDB();
                }
            }
            else if (flag.Equals("data2"))
            {
                DBHandler.Instance.CleanDB();
                try
                {
                    state.stateFile(flag + ".json");
                    state.checkFile1();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    DBHandler.Instance.CleanDB();
                }
            }
            else if (flag.Equals("dataBad"))
            {
                DBHandler.Instance.CleanDB();
                try
                {
                    state.stateFile(flag + ".json");

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    DBHandler.Instance.CleanDB();
                }
            }

            else //we will not load a file 
            {
                if (ApplicationOptions.StartWithCleanDB)
                    DBHandler.Instance.CleanDB();

                if (ApplicationOptions.RunLoadData)
                    TradingSystem.Instance.LoadData();
            }

            //start the api server
            ServerServiceHost serverServiceHost = new ServerServiceHost();
            serverServiceHost.Start();

            //start the signalR server
            SignalRServiceHost signalRServiceHost = new SignalRServiceHost();
            signalRServiceHost.Start();


            Console.ReadLine();



        }


        public static void InitAppSettings()
        {
            string Host = ConfigurationManager.AppSettings["Host"];
            if (string.IsNullOrEmpty(Host))
            {
                ThrowConfigException("Host"); 
            }
            ApplicationOptions.Host = Host;

            string ApiPort = ConfigurationManager.AppSettings["ApiPort"];
            if (string.IsNullOrEmpty(ApiPort) == false)
            {
                if (int.TryParse(ApiPort, out int intValue))
                {
                    ApplicationOptions.ApiPort = intValue;
                }
                else
                {
                    ThrowConfigException("ApiPort");
                }
            }
            else
            {
                ThrowConfigException("ApiPort");
            }

            string SignalRPort = ConfigurationManager.AppSettings["SignalRPort"];
            if (string.IsNullOrEmpty(SignalRPort) == false)
            {
                if (int.TryParse(SignalRPort, out int intValue))
                {
                    ApplicationOptions.SignalRPort = intValue;
                }
                else
                {
                    ThrowConfigException("SignalRPort");
                }
            }
            else
            {
                ThrowConfigException("SignalRPort");
            }

            string PaymentServiceURL = ConfigurationManager.AppSettings["PaymentServiceURL"];
            if (string.IsNullOrEmpty(PaymentServiceURL))
            {
                ThrowConfigException("PaymentServiceURL");
            }
            ApplicationOptions.PaymentServiceURL = PaymentServiceURL;

            string SupplierServiceURL = ConfigurationManager.AppSettings["SupplierServiceURL"];
            if (string.IsNullOrEmpty(SupplierServiceURL))
            {
                ThrowConfigException("SupplierServiceURL");
            }
            ApplicationOptions.SupplierServiceURL = SupplierServiceURL;

            string SystemManagerEmail = ConfigurationManager.AppSettings["SystemManagerEmail"];
            if (string.IsNullOrEmpty(SystemManagerEmail))
            {
                ThrowConfigException("SystemManagerEmail");
            }
            ApplicationOptions.SystemManagerEmail = SystemManagerEmail;

            string SystemManagerFirstName = ConfigurationManager.AppSettings["SystemManagerFirstName"];
            if (string.IsNullOrEmpty(SystemManagerFirstName))
            {
                ThrowConfigException("SystemManagerFirstName");
            }
            ApplicationOptions.SystemManagerFirstName = SystemManagerFirstName;

            string SystemManagerLastName = ConfigurationManager.AppSettings["SystemManagerLastName"];
            if (string.IsNullOrEmpty(SystemManagerLastName))
            {
                ThrowConfigException("SystemManagerLastName");
            }
            ApplicationOptions.SystemManagerLastName = SystemManagerLastName;

            string SystemManagerPass = ConfigurationManager.AppSettings["SystemManagerPass"];
            if (string.IsNullOrEmpty(SystemManagerPass))
            {
                ThrowConfigException("SystemManagerPass");
            }
            ApplicationOptions.SystemManagerPass = SystemManagerPass;

            string InitTradingSystem = ConfigurationManager.AppSettings["InitTradingSystem"];
            if (string.IsNullOrEmpty(InitTradingSystem) == false)
            {
                if (bool.TryParse(InitTradingSystem, out bool boolValue))
                {
                    ApplicationOptions.InitTradingSystem = boolValue;
                }
                else
                {
                    ThrowConfigException("InitTradingSystem");
                }
            }
            else
            {
                ThrowConfigException("InitTradingSystem");
            }

            string StartWithCleanDB = ConfigurationManager.AppSettings["StartWithCleanDB"];
            if (string.IsNullOrEmpty(StartWithCleanDB) == false)
            {
                if (bool.TryParse(StartWithCleanDB, out bool boolValue))
                {
                    ApplicationOptions.StartWithCleanDB = boolValue;
                }
                else
                {
                    ThrowConfigException("StartWithCleanDB");
                }
            }
            else
            {
                ThrowConfigException("StartWithCleanDB");
            }

            string RunLoadData = ConfigurationManager.AppSettings["RunLoadData"];
            if (string.IsNullOrEmpty(RunLoadData) == false)
            {
                if (bool.TryParse(RunLoadData, out bool boolValue))
                {
                    ApplicationOptions.RunLoadData = boolValue;
                }
                else
                {
                    ThrowConfigException("RunLoadData");
                }
            }
            else
            {
                ThrowConfigException("RunLoadData");
            }

            string StateFileConfig = ConfigurationManager.AppSettings["StateFileConfig"];
            if (StateFileConfig==""|| StateFileConfig == "data" || StateFileConfig == "data2"|| StateFileConfig == "dataBad")
            {
                ApplicationOptions.StateFileConfig = StateFileConfig;
                
            }
            else
            {
                ThrowConfigException("StateFileConfig");
            }

            string TestDB = ConfigurationManager.AppSettings["TestDB"];
            if (string.IsNullOrEmpty(TestDB))
            {
                ThrowConfigException("TestDB");
            }
            ApplicationOptions.TestDB = TestDB;
        }

        private static void ThrowConfigException(string configKey)
        {
            Logger.Instance.Error($"Configuration is not valid, system can not load, {configKey} was not initialize properly");
            throw new Exception($"Configuration is not valid, system can not load, {configKey} was not initialize properly");
        }
    }
}