using System;

namespace SadnaExpress
{
    class SadnaException : Exception
    {
        public SadnaException(String description, String className, String funcionName)
        {
            Logger.WriteToLog("***Exception*** details: " + description + ". in Class: " + className + ", in function: " + funcionName + ".");
            throw new Exception(description);
        }
    }
}