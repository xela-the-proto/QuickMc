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

public class InstanceRunner
{
    /// <summary>
    ///     Starts a minecraft instance asking a few questions first
    /// </summary>
    public void InitRunner()
    {
        ServerInfo info = new ServerInfo();
        DownloadManifestStruct serverManifest = null;
        Log.Information("Type the server name");
        var name = Console.ReadLine();
        Log.Information("Insert which version should the manager get:");
        var version = Console.ReadLine();
        Log.Information("Insert how much ram should the server use in Gb:");
        var ram = Convert.ToInt32(Console.ReadLine());
        
        Log.Debug("Getting the jar file from the manifest");
        var manifest = ManifestSingleton.GetInstance();
        
        Log.Verbose("Finding version from manifest");
        var entry = manifest.versions.Find(x => x.id == version);
        if (entry.Url == null)
        {
            throw new NullReferenceException("Invalid version!");
        }
        //Check for db so no need to download
        var cachedEntry = (DownloadManifestStruct)InsertVersionManifest.checkSha256(entry);
        if ( cachedEntry != null)
        {
            Log.Warning("Found jar on database skipping download...");
            info.firstRun = true;
            info.guid = Guid.CreateVersion7();
            info.name = name;
            info.version = version;
            Directory.CreateDirectory(Logging.path_root + $"/QuickMc/Servers/{info.guid}");
            info.path = Logging.path_root + $"/QuickMc/Servers/{info.guid}";
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
            InsertVersionManifest.storeOnDb(serverManifest);
        }
        
        
        
        var process = new InstanceRunner().buildStarterProcess(info, ram);
        using (var context = new DatabaseFramework())
        {
            var server = new ServerInfo
            {
                firstRun = info.firstRun,
                path = info.path,
                name = name,
                version = version,
                guid = info.guid
            };
            context.Database.EnsureCreated();
            context.Add(server);
            context.SaveChanges();
        }
        
        if (info.firstRun)
        {
            if (FirstRunServer(serverManifest, process))
            {
                Log.Information("Eula accepted, starting in 5 seconds");
                Thread.Sleep(5000);
                RunServer(serverManifest, process);
            }
        }
        Console.Clear();
        
    }
   

    /// <summary>
    /// Builds the process to start
    /// </summary>
    /// <param name="manifest"></param>
    /// <returns></returns>
    public Process buildStarterProcess(ServerInfo info, int ram)
    {
        var root_server = $"{Logging.path_root}/QuickMc/Servers/{info.guid}";
        if (!Directory.Exists(root_server)) Directory.CreateDirectory(root_server);

        var startInfo = new ProcessStartInfo
        {
            FileName = "java",
            Arguments = $"-Xms{ram}G -Xmx{ram}G -jar {Logging.path_root}/QuickMc/Servers/{info.guid}/server.jar --nogui",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
            WorkingDirectory = root_server
        };
        var process = new Process { StartInfo = startInfo };
        return process;
    }
    
    /// <summary>
    /// used just for the first run of the server, to accept the eula (there probably is a more elegant solution but i
    /// cant think of one)
    /// </summary>
    /// <param name="manifest"></param>
    /// <returns></returns>
    public bool FirstRunServer(DownloadManifestStruct manifest, Process process)
    {
        //I i interact with porcess again without making a new one DO NOT subscribe 2 times, even if i cancel the read
        //output it keeps hooked
        process.OutputDataReceived += Program.server.LogServerInfo;

        process.ErrorDataReceived += Program.server.LogServerError;
        Log.Warning("Waiting for server start to agree to eula");
        process.Start();
        Log.Debug("Process started waiting for exit now...");
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        while (!process.HasExited)
        {
            //do jack shit just wait
            Log.Debug("Bleh :3");
        }
        Log.Debug("Process exited overwriting lua");
        
       
        process.CancelOutputRead();
        process.CancelErrorRead();
        return accept_EULA(process.StartInfo.WorkingDirectory);
    }

    public static void RunServer(DownloadManifestStruct manifest, Process process)
    {
        
        Console.Clear();
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        // Forward input to Minecraft server
        while (!process.HasExited) 
        { 
            var input = Console.ReadLine();
            if (input != null && input.ToLower() != "stop")
            { 
                process.StandardInput.WriteLine(input);
            }
            else
            {
                process.StandardInput.WriteLine("stop");
                break;
            }
        }
        
        //we broke the loop so the server is shutting down
        while (!process.HasExited)
        {
            Thread.Sleep(500);
            //try to shut down fast
        }
        Log.Information("Minecraft server exited");
        process.CancelOutputRead();
        process.CancelErrorRead();
        process.Dispose();
    }

    public static bool accept_EULA(string root)
    {
        Log.Error("By pressing y " +
                  "you agree to Minecraft and Mojang's EULA (https://aka.ms/MinecraftEULA).");
        var accept = Console.ReadLine();
        if (accept?.ToLower() == "y")
        {
            var stream = File.ReadAllText(Path.Combine(root, "eula.txt"));
            stream = stream.Replace("eula=false", "eula=true");
            File.WriteAllText(Path.Combine(root, "eula.txt"), stream);
            Log.Warning("Restarting server...");
            return true;
        }
        else
        {
            Log.Fatal("Quitting");
            Environment.Exit(2);
            return false;
        }
    }

   
}

