using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CLUplader;
namespace ClientLibraryUploader
{
    class Program
    {
        internal static List<string> ArgNames =
           new List<string>(new string[] { "TARGET", "SOURCE", "WAIT", "?" });
        #region Usage screen
        private static string UsageScreen = String.Concat(
            //  0         1         2         3         4         5         6         7         |
            //  01234567890123456789012345678901234567890123456789012345678901234567890123456789|
            "Sense/Net Client Library based Uploader. " + Environment.NewLine +
            "Description: You can upload files with this Odata REST API based application." +
            "Usage:" + Environment.NewLine,
            "ClientLibraryUploader.exe [-?] [-HELP]" + Environment.NewLine,
            "ClientLibraryUploader.exe [-target <target>] [-source <source>] [-WAIT]" + Environment.NewLine,
            Environment.NewLine,
            "Parameters:" + Environment.NewLine,
            "<source>:         Directory Path or File Path. If the path is a DirectoryPath," + Environment.NewLine +
            "the App will upload all of files under the foldere." + Environment.NewLine,
            "<target>:       This the destination Path, it's a container in the Content Repository. " + Environment.NewLine + Environment.NewLine
        );
        #endregion
        static void Main(string[] args)
        {
            CLUplader.LoggerClass.InitLoggerClass();
            Dictionary<string, string> parameters;
            string message;
            ParseParameters(args, ArgNames, out parameters, out message);

            if (!parameters.ContainsKey("TARGET") || !parameters.ContainsKey("SOURCE"))
            {
                Console.WriteLine("Missing parameters!");
                Usage(message);
                return;
            }
            string sourcePath = parameters["SOURCE"]; 
            string targetPath = parameters["TARGET"];           

            bool waitForAttach = parameters.ContainsKey("WAIT");
            bool help = parameters.ContainsKey("?") || parameters.ContainsKey("HELP");

            if (help)
            {
                Usage(message);
            }
            else
            {
                if (waitForAttach)
                {
                    Console.WriteLine(
                        "Running in wait mode - now you can attach to the process with a debugger.");
                    Console.WriteLine("Press ENTER to continue.");
                    Console.WriteLine();
                    Console.ReadLine();
                }
                try
                {
                    CLUplader.ClientLibraryUploader clu = new CLUplader.ClientLibraryUploader(sourcePath, targetPath);
                    var uploadTask = clu.Upload();
                    uploadTask.Wait();
                }
                catch (Exception e)
                {
                    Console.WriteLine();
                    Console.WriteLine("========================================");
                    Console.WriteLine("ClientLibraryUploader ends with error:");
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    Console.WriteLine();
                    Console.WriteLine("========================================");
                    Console.WriteLine("Operation has ended.");
                    Console.ReadLine();                    
                }
            }
          
        }
        internal static bool ParseParameters(string[] args, List<string> argNames,
            out Dictionary<string, string> parameters,
            out string message)
        {
            message = null;
            parameters = new Dictionary<string, string>();
            if (args.Length == 0)
                return false;

            int argIndex = -1;
            int paramIndex = -1;
            string paramToken = null;
            while (++argIndex < args.Length)
            {
                string arg = args[argIndex];
                if (arg.StartsWith("-"))
                {
                    paramToken = arg.Substring(1).ToUpper();

                    
                    paramIndex = ArgNames.IndexOf(paramToken);
                    if (!argNames.Contains(paramToken))
                    {
                        message = "Unknown argument: " + arg;
                        return false;
                    }
                    parameters.Add(paramToken, null);
                }
                else
                {
                    if (paramToken != null)
                    {
                        parameters[paramToken] = arg;
                        paramToken = null;
                    }
                    else
                    {
                        message = String.Concat("Missing parameter name before '", arg, "'");
                        return false;
                    }
                }
            }
            return true;
        }

        private static void Usage(string message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                Console.WriteLine("--------------------");
                Console.WriteLine(message);
                Console.WriteLine("--------------------");
            }
            Console.WriteLine(UsageScreen);
        }               
    }
}
