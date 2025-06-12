using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using QuickMC.Json.JsonClasses;
using QuickMC.Utils;
using Serilog;
using Spectre.Console;

namespace QuickMC.Server;

public class instanceRunner
{
    public void Runner()
    {
        ServerInfo info;
        //TODO: For VERY big databases split in multiple queries like a binary tree
        var servers = Program.db.server.ToList();
        string[] ServerNames = new string[servers.Count];

        for (int i = 0; i < servers.Count; i++)
        {
            ServerNames[i] = servers[i].name;
        }
        
        while (true)
        {
            Console.Clear();
            var list = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose which server to start")
                    .AddChoices(ServerNames));
            var entry = servers.First(x => x.name == list);
            Program.server.listServer(entry);
            var conf = AnsiConsole.Prompt(new TextPrompt<bool>("Is this the right server?")
                .AddChoice(true)
                .AddChoice(false)
                .DefaultValue(true)
                .WithConverter(choice => choice ? "y" : "n"));
            if (conf)
            {
                info = entry;
                break;
            }
        }

        var process = buildStarterProcess(info);
        //ONLY SUBSCRIBE HERE! DONT RE SUB!
        process.OutputDataReceived += Program.server.LogServerInfo;
        process.ErrorDataReceived += Program.server.LogServerError;
        
       if (info.firstRun)
       {
           if (FirstRunServer(process))
           {
               Log.Information("Eula accepted, waiting for database update...");
               var server = Program.db.server.First(x => x.guid == info.guid);
               server.firstRun = false;
               Program.db.SaveChanges();
               
           }
       }
       else
       {
           RunServer(process, info);
       }
    }

    

    /// <summary>
    /// used just for the first run of the server, to accept the eula (there probably is a more elegant solution but i
    /// cant think of one)
    /// </summary>
    /// <param name="manifest"></param>
    /// <returns></returns>
    private bool FirstRunServer(Process process)
    {
        

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

    private static void RunServer(Process process, ServerInfo info)
    {
        //TODO: FIX BROKEN FIREWALL
        //Program.net.openPortW(info.name,process.StartInfo.WorkingDirectory + "/server.jar");
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
        process.OutputDataReceived -= Program.server.LogServerInfo;
        process.ErrorDataReceived -= Program.server.LogServerError;
        process.Dispose();
    }

    private static bool accept_EULA(string root)
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
    
    ///<summary>
    /// Builds the process to start
    /// </summary>
    /// <param name="manifest"></param>
    /// <returns></returns>
    private Process buildStarterProcess(ServerInfo info)
    {
        var root_server = $"{Logging.path_root}/QuickMc/Servers/{info.guid}";
        if (!Directory.Exists(root_server)) Directory.CreateDirectory(root_server);

        var startInfo = new ProcessStartInfo
        {
            FileName = "java",
            Arguments = $"-Xms{info.Ram}g -Xmx{info.Ram}g -jar {Logging.path_root}/QuickMc/Servers/{info.guid}/server.jar --nogui",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
            WorkingDirectory = root_server
        };
        var process = new Process { StartInfo = startInfo };
        return process;
    }

}