using MVCCodeGenerator.Utils;
using System;
using System.Configuration;

namespace MVCCodeGenerator
{
    public abstract class ABasicGenerator
    {
        private string _logFilePath;

        private string _header = @"
        /*****************************************************************
        * Code Generated at {0}
        * By Code MVCCodeGenerator
        *
        *
        ******************************************************************/
        ";

        public string Heading
        {
            get
            {
               return string.Format(_header, DateTime.Now.ToString()) + Environment.NewLine;
            }
        }

        public ABasicGenerator()
        {
            _logFilePath = ConfigurationManager.AppSettings["logpath"] + "\\" + DateTime.Now.ToString("dd-MM-YYYY") + ".txt";
        }

        public void LogItem(string message)
        {
            FileUtils.AppendToFile(_logFilePath, message);
        }

        public void LogAndDisplay(string message)
        {
            FileUtils.AppendToFile(_logFilePath, message);
            Console.WriteLine(message);
        }
    }
}
