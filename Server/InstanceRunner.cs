using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using MinecraftServer.Implement;
using MinecraftServer.Json;
using Serilog;

namespace MinecraftServer.Server;

public class InstanceRunner
{
    /// <summary>
    ///     Starts a minecraft instance asking a few questions first
    /// </summary>
    public static void InitRunner()
    {
        Log.Information("Type the server name");
        var name = Console.ReadLine();
        Log.Information("Insert which version should the manager get:");
        var version = Console.ReadLine();
        Log.Information("Insert how much ram should the server use in Gb:");
        var ram = Convert.ToInt32(Console.ReadLine());

        Log.Debug("Getting the jar file from the manifest");
        var manifest = ManifestSingleton.GetInstance();
        var entry = manifest.versions.Find(x => x.id == version);
        if (entry.Url == null)
        {
            throw new NullReferenceException("Invalid version!");
        }
        Log.Debug("Downloading jar");
        
        var serverManifest = (DownloadManifestStruct)Program.progress.InitBarDownload("Downloading version manifest"
            , new HttpClient(), entry.Url).Result;
        //wait for jar download
        Program.progress.InitBarDownload("Downloading jar file", new HttpClient(), serverManifest.url,serverManifest.id).Wait();
        ServerInfo info = new ServerInfo()
        {
            firstRun = true,
            name = name,
            path =  $"/usr/share/QuickMc/Servers/{serverManifest.id}",
            version = serverManifest.id
        };
        var process = new InstanceRunner().buildStarterProcess(serverManifest, ram);
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
    public Process buildStarterProcess(DownloadManifestStruct manifest, int ram)
    {
        var root_server = $"/usr/share/QuickMc/Servers/{manifest.id}";
        if (!Directory.Exists(root_server)) Directory.CreateDirectory(root_server);

        var startInfo = new ProcessStartInfo
        {
            FileName = "java",
            Arguments = $"-Xms{ram}G -Xmx{ram}G -jar {Logging.path_root}/QuickMc/Servers/{manifest.id}/server.jar",
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
    public static bool FirstRunServer(DownloadManifestStruct manifest, Process process)
    {
        Log.Warning("Waiting for server start to agree to eula");
        process.Start();
        Log.Debug("Process started waiting for exit now...");
        
        //Apparently constantly "pinging the process makes the exit detection more reliable?
        while (!process.HasExited)
        {
            Log.Information($"{process.Id}, {process.StartInfo.Arguments}");
            Thread.Sleep(500);
        }
        Log.Debug("Process exited overwriting lua");
        return accept_EULA(process.StartInfo.WorkingDirectory);
    }

    public static void RunServer(DownloadManifestStruct manifest, Process process)
    {
        process.OutputDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
                Console.WriteLine("[MC] " + args.Data);
        };

        process.ErrorDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
                Console.WriteLine("[MC:ERR] " + args.Data);
        };

        Console.Clear();
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        // Forward input to Minecraft server
        while (!process.HasExited) 
        { 
            var input = Console.ReadLine();
            if (input != null)
            {
                process.StandardInput.WriteLine(input);
            }
        }
    }

    public static bool accept_EULA(string root)
    {
        Log.Error("By pressing y and setting the eula setting below to TRUE you are indicating " +
                  "your agreement to Minecraft and Mojang's EULA (https://aka.ms/MinecraftEULA).");
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

