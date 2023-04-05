using System;

namespace SadnaExpress
{
    class SadnaException : Exception
    {
       // public static Logger log = Logger.Instance;
        public SadnaException(String description, String className, String funcionName)
        {
            Logger.Instance.Info("***Exception*** details: " + description + ". in Class: " + className + ", in function: " + funcionName + ".");
            throw new Exception(description);
        }


    }
}