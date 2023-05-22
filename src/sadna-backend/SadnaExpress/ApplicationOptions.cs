using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress
{
    public static class ApplicationOptions
    {
        public static string Host="";

        public static int ApiPort = 0;

        public static int SignalRPort = 0;

        public static string DataBaseServer = "";

        public static string DataBaseName = "";

        public static string DataBaseUIdAuthentication = "";

        public static string DataBasePwdAuthentication = "";

        public static string PaymentServiceURL = "";

        public static string SupplierServiceURL = "";

        public static string SystemManagerEmail = "";

        public static string SystemManagerFirstName = "";

        public static string SystemManagerLastName = "";

        public static string SystemManagerPass = "";

        public static bool InitTradingSystem = false;

    }
}
