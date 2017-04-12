using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;

namespace CLUplader
{
    static public class LoggerClass
    {
        private static string STARTTIME;
        private static int COUNTER;

        static public void InitLoggerClass()
        {
            STARTTIME = DateTime.Now.ToString("yyyyMMdd_HH_mm_ss");
        }
       
        static public void LogToCSV(string logtext, string filename = "")
        {
            if (ConfigurationManager.AppSettings["LogEnabled"].ToLower() == "true")
            {
                string logDirectory = ConfigurationManager.AppSettings["LogFolder"];
                if(!new DirectoryInfo(logDirectory).Exists)
                {
                    Directory.CreateDirectory(logDirectory);
                }
                using (StreamWriter writer = new StreamWriter(logDirectory + @"\"+ STARTTIME + ".csv", true))
                {
                    Console.WriteLine(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss - ") + filename + " - " + logtext);
                    writer.WriteLine(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss,") + filename + "," + logtext);
                }
            }
            
        }
    }
}
