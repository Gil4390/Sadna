using System;

namespace SadnaExpress
{
    class SadnaExpressException : Exception
    {
        public SadnaExpressException(String description, String className, String funcionName)
        {
            Logger.WriteToLog("***Exception*** details: " + description + ". in Class: " + className + ", in function: " + funcionName + ".");
            throw new Exception(description);
        }

        public static void WriteToLog(String str)
        {
            Logger.WriteToLog(str);
        }
    }
}