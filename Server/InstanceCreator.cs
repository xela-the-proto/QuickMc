using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Threading;
using QuickMC.Db;
using QuickMC.Json.JsonClasses;
using QuickMC.Utils;
using Serilog;

namespace QuickMC.Server;

public class InstanceCreator
{
    /// <summary>
    ///     Starts a minecraft instance asking a few questions first
    /// </summary>
    public void Create()
    {
        //TODO:Make more server option available
        ServerInfo info = new ServerInfo();
        DownloadManifestStruct serverManifest = null;
        Log.Information("Type the server name");
        var name = Console.ReadLine();
        Log.Information("Insert which version should the manager get:");
        var version = Console.ReadLine();
        Log.Information("Insert how much ram should the server use in Gb:");
        var ram = Convert.ToUInt32(Console.ReadLine());
        
        Log.Debug("Getting the jar file from the manifest");
        var manifest = ManifestSingleton.GetInstance();
        
        Log.Verbose("Finding version from manifest");
        var entry = manifest.versions.Find(x => x.id == version);
        if (entry.Url == null)
        {
            throw new NullReferenceException("Invalid version!");
        }
        //Check for db so no need to download
        var cachedEntry = (DownloadManifestStruct)Program.dbOp.checkSha256(entry);
        if ( cachedEntry != null)
        {
            Log.Warning("Found jar on database skipping download...");
            info.firstRun = true;
            info.guid = Guid.NewGuid();
            info.name = name;
            info.version = version;
            Directory.CreateDirectory(Logging.path_root + $"/QuickMc/Servers/{info.guid}");
            info.path = Logging.path_root + $"/QuickMc/Servers/{info.guid}";
            info.modded = false;
            info.serverSoftware = "VANILLA";
            File.WriteAllBytes(Logging.path_root + $"/QuickMc/Servers/{info.guid}/server.jar"  ,cachedEntry.JarFile);
        }
        else
        {
            Log.Verbose("Downloading version specific manifest");
            serverManifest = (DownloadManifestStruct)Program.progress.InitBarDownload("Downloading version manifest"
                , new HttpClient(), entry.Url).Result;
        
            //wait for jar download
            info = (ServerInfo)Program.progress.InitBarDownload("Downloading jar file", new HttpClient(), serverManifest.url,serverManifest.id).Result;
            info.name = name;
            
            serverManifest.JarFile = File.ReadAllBytes(info.path + "/server.jar");
            serverManifest.JsonManifestSha256 = entry.Sha1;
            Program.dbOp.storeOnDb(serverManifest);
        }
        
        using (var context = new DatabaseFramework())
        {
            var server = new ServerInfo
            {
                firstRun = info.firstRun,
                path = info.path,
                name = name,
                version = version,
                guid = info.guid,
                Ram = ram,
                modded = false,
                serverSoftware = "VANILLA"
            };
            context.Database.EnsureCreated();
            context.Add(server);
            context.SaveChanges();
        }
        Console.Clear();
    }
    
    
   
}