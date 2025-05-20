using System.Diagnostics;
using MinecraftServer.Json;
using MinecraftServer.Utils;
using Serilog;

namespace MinecraftServer.Server;

public class InstanceRunner
{
    /// <summary>
    ///     Starts a minecraft instance asking a few questions first
    /// </summary>
    public void InitRunner()
    {
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
        Log.Verbose("Downloading version manifest");
        var serverManifest = (DownloadManifestStruct)Program.progress.InitBarDownload("Downloading version manifest"
            , new HttpClient(), entry.Url).Result;
        //wait for jar download
        var info = (ServerInfo)Program.progress.InitBarDownload("Downloading jar file", new HttpClient(), serverManifest.url,serverManifest.id).Result;
        info.name = name;
        var process = new InstanceRunner().buildStarterProcess(info, ram);
        Program.server.writeServerInfoToDir(info.path, info.firstRun,name,version, info.guid);
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
        process.OutputDataReceived += Program.server.LogServerInfo;

        process.ErrorDataReceived += Program.server.LogServerError;
        Log.Warning("Waiting for server start to agree to eula");
        process.Start();
        Log.Debug("Process started waiting for exit now...");
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        //Apparently constantly "pinging the process makes the exit detection more reliable?
        while (!process.HasExited)
        {
            //do jack shit just wait
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

