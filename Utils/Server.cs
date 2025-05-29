using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuickMC.Db;
using QuickMC.Interfaces;
using QuickMC.Json.JsonClasses;
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
        List<ServerInfo> ServerList = null;
        using (var context = new DatabaseFramework())
        {
            ServerList = context.server.ToList();
        }
        foreach (var server in ServerList) 
        {
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