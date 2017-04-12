using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLUplader;
using System.IO;

namespace ClientLibraryUploaderTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourcePath =  @"c:\Temp";
            string targetPath = "/Root/YourDocuments";

            CLUplader.LoggerClass.InitLoggerClass();
                        
            while (true)
            {                
                try
                {
                    CLUplader.ClientLibraryUploader clu = new CLUplader.ClientLibraryUploader(sourcePath, targetPath);
                    var uploadTask = clu.Upload();
                    uploadTask.Wait();                    
                }
                catch(Exception e)
                {                   
                    CLUplader.LoggerClass.LogToCSV(e.Message);
                }
            }
        }

        

        
    }
}
