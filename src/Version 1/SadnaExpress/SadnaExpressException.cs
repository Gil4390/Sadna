using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress
{
    class SadnaExpressException : Exception
    {
        public static Logger log = new Logger("LoggerOutput");


        public SadnaExpressException(String description, String className, String funcionName)
        {
            log.WriteToLog("***Exception*** details: " + description + ". in Class: " + className + ", in function: " + funcionName + ".");
            throw new Exception(description);
        }

        public static void WriteToLog(String str)
        {
            log.WriteToLog(str);
        }
    }
}
