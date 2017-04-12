# SenseNet Client Library Uploader

You can upload files, and folders with this Odata REST API based application to a [SenseNet ECM](https://github.com/SenseNet/sensenet) repository. This tool based on [Sense/Net Client library for .Net](https://github.com/SenseNet/sn-client-dotnet)

# Usage 

First of all, you can set the configuration file. Here is an example: 

```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5.2" />
    </startup>
  <appSettings>
    <add key="SiteUrl" value="http://daily.demo.sensenet.com/" />
    <add key="Username" value="admin" />
    <add key="Password" value="admin" />
    <add key="Recursive" value="true" />
    <add key="FolderType" value="Folder" />
    <add key="LogEnabled" value="True" />
    <add key="LogFolder" value="Logs" />
  </appSettings>
</configuration>
```
- **SiteUrl**: this is url of the SN Portal 
- **Username and Password** for authentication. 
- **Recursive**: if it is true the uploader will upload all files under subdirectories
- **FolderTye**: when you upload folders and the repository doesn't contains a folder, the tool will create a new container. This configuration for determining what type is created.
- **LogEnabled**: if it is true, the logging is enabled.
- **LogFolder**: the place wher you will be able to find logs.

After you setted the configuration file, you can run the tool. You need following this syntax: 

```
ClientLibraryUploader.exe [-target <target>] [-source <source>] [-WAIT]
```

- **target** is the repository path
- **source** is the file system location

For this usage screen: 

```
ClientLibraryUploader.exe [-?] [-HELP]
```


