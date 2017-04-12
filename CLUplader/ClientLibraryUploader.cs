using SenseNet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace CLUplader
{
    public class ClientLibraryUploader
    {
        ServerContext sctx;
        bool _IsDircetoryUpload = false;
        bool IsDirectoryUpload { get { return _IsDircetoryUpload; } }
        string _Source = string.Empty;
        string Source
        {
            get
            {
                return _Source;
            }
            set
            {
                _Source = value;
                FileAttributes attr = File.GetAttributes(_Source);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    _IsDircetoryUpload = true;
                }
                else
                {
                    _IsDircetoryUpload = false;
                }
            }
        }
        public string BaseDirectory {
            get
            {
                if (IsDirectoryUpload)
                {
                    return Source;
                }
                else
                {
                    FileInfo fi = new FileInfo(Source);
                    return fi.DirectoryName;
                }
            }
        }
        string Target { get; set; }
        bool Recursive = false;
        public ClientLibraryUploader(string source, string target)
        {
            sctx = new ServerContext
            {
                Url = ConfigurationManager.AppSettings["SiteUrl"],
                Username = ConfigurationManager.AppSettings["Username"],
                Password = ConfigurationManager.AppSettings["Password"]
            };
            Source = source;
            
            Target = target;

            if (ConfigurationManager.AppSettings["Recursive"].ToLower() == "true")
            {
                Recursive = true;
            }
        }

        public List<FileInfo> Files
        {
            get
            {
                List<FileInfo> files = new List<FileInfo>();
                if (IsDirectoryUpload)
                {
                    //Directory
                    DirectoryInfo di = new DirectoryInfo(Source);
                    if (di == null)
                    {
                        throw new Exception("Path does not exist.");
                    }

                    if (Recursive)
                    {
                        files.AddRange(di.GetFiles("*.*", SearchOption.AllDirectories));
                    }
                    else
                    {
                        files.AddRange(di.GetFiles());
                    }
                }
                else
                {
                    //File
                    FileInfo fi = new FileInfo(Source);
                    if (fi == null)
                    {
                        throw new Exception("Path does not exist.");
                    }
                    files.Add(fi);
                }
                return files;
            }
        }
        public async Task Upload()
        {
            try
            {
                Console.WriteLine("Initilization...");
                ClientContext.Initialize(new[] { sctx });

                Console.WriteLine("Upload Started");
                //Is Parent exists
                var content = await Content.LoadAsync(Target);
                if (content != null)
                {
                    //Uploading files
                    var tasks = new List<Task>();
                    foreach (var file in Files)
                    {
                        string fileTargetFolder = Target + file.DirectoryName.Replace(Source, "").Replace(BaseDirectory, "").Replace("\\", "/");
                        var fileTargetContentFolder = await Content.LoadAsync(fileTargetFolder);
                        if (fileTargetContentFolder == null)
                        {
                            if (CreateFolderPath(Target, file.DirectoryName.Replace(Source, "")))
                            {
                                fileTargetContentFolder = await Content.LoadAsync(fileTargetFolder);
                                Console.WriteLine("#Upload file: " + file.FullName);
                                tasks.Add(Content.UploadAsync(fileTargetContentFolder.Id, file.Name, file.OpenRead()));
                                LoggerClass.LogToCSV("File uploaded", file.Name);
                            }
                            else
                            {
                                LoggerClass.LogToCSV("File target folder does not exist or you do not have enough permission to see! File can not be uploaded. ", file.Name);
                            }
                        }
                        else
                        {
                            Console.WriteLine("#Upload file: " + file.FullName);
                            tasks.Add(Content.UploadAsync(fileTargetContentFolder.Id, file.Name, file.OpenRead()));
                            LoggerClass.LogToCSV("File uploaded", file.Name);
                        }
                    }
                    await Task.WhenAll(tasks);
                }
                else
                {
                    Console.WriteLine("Target does not exist or you do not have enough permission to see!");
                    LoggerClass.LogToCSV("Target does not exist or you do not have enough permission to see!");
                }
                Console.WriteLine("Upload finished.");
            }
            catch (Exception ex)
            {
                LoggerClass.LogToCSV(ex.Message);
            }
        }
        private  bool CreateFolderPath(string parentPath, string pathToCreate)
        {
            try
            {
                string[] folders = pathToCreate.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string folderName in folders)
                {
                    var fileTargetContentFolderTask =  Content.LoadAsync(parentPath+"/"+folderName);
                    fileTargetContentFolderTask.Wait();
                    if (fileTargetContentFolderTask.Result == null)
                    {
                        var folder = Content.CreateNew(parentPath, ConfigurationManager.AppSettings["FolderType"], folderName);
                        var task = folder.SaveAsync();
                        task.Wait();
                    }
                    parentPath += "/" + folderName;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
