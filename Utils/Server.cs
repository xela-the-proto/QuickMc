using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuickMC.Interfaces;
using QuickMC.Json;
using QuickMC.Json.Json_classes;
using Serilog;

namespace QuickMC.Utils;

public class Server : IServer
{
    public void writeServerInfoToDir(string path, bool firstRun, string name, string version, Guid guid)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        File.WriteAllText(path + @"/QuickMc.json", JsonConvert.SerializeObject(new ServerInfo
        {
            firstRun = firstRun,
            path = path,
            name = name,
            version = version,
            guid = guid
        }, Formatting.Indented));
    }

    public void listServers()
    {
        var columns =  new List<string> { "Path", "Name", "Version", "Guid" };
        foreach (var directory in Directory.GetDirectories(Path.Combine(Logging.path_root + @"/QuickMc/Servers"))) 
        {
            var obj = JObject.Parse(File.ReadAllText
                (Path.Combine(directory+@"/QuickMc.json"))) ?? throw new FileNotFoundException();
            var server= JsonConvert.DeserializeObject<ServerInfo>(obj.ToString()) ?? 
                            throw new FormatException("bad server manifest format!");
            var rows = new List<string> { server.path, server.name, server.version, server.guid.ToString() };
            Program.progress.DrawTable(columns, rows);
        }
    }

    public void LogServerInfo(object obj, DataReceivedEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.Data))
        {
            Log.Information("[MC] " + args.Data);
        }
    }
    
    public void LogServerError(object obj, DataReceivedEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.Data))
        {
            Log.Error("[MC] " + args.Data);
        }
    }
}