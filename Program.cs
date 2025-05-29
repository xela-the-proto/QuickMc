using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickMC.Interfaces;
using QuickMC.Json;
using QuickMC.Net;
using QuickMC.Server;
using QuickMC.Utils;
using Serilog;

namespace QuickMC;

class Program
{
    public static string Manifest;
    public static ILogging logging;
    public static IConfigurationRoot _config;
    public static IWeb net;
    public static IRegistry registry;
    public static IParsers jsonParsers;
    public static IConsoleUI progress;
    public static IServer server;
    
    private static string title =
        "   ____        _      __   __  _________\n  / __ \\__  __(_)____/ /__/  |/  / ____/\n / / /" +
        " / / / / / ___/ //_/ /|_/ / /     \n/ /_/ / /_/ / / /__/ ,< / /  / / /___   " +
        "\n\\___\\_\\__,_/_/\\___/_/|_/_/  /_/\\____/";
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        _config = config;
        var services =  new Program().registerServices();
        net = services.GetRequiredService<IWeb>();
        logging = services.GetRequiredService<ILogging>();
        registry = services.GetRequiredService<IRegistry>();
        jsonParsers = services.GetRequiredService<IParsers>();
        progress = services.GetRequiredService<IConsoleUI>();
        server = services.GetRequiredService<IServer>();
        
        //get the main manifest
        await progress.InitBarDownload("Downloading main manifest",
            new HttpClient(),config["AppSettings:MojangUrl"] );
        logging.initLogging();
        Log.Verbose("got main manifest and initted logging");
        Log.Verbose("Running main while loop");
        new Program().Runner();
    }

    public void Runner()
    {
        try
        {
            string switchArg;
            var runner = new InstanceRunner();
            Log.Information($"\n{title}");
            while (true)
            {
                Log.Information("\nOptions:\n1)Start a server\n2)List current servers\nx)exit");
                switchArg = Console.ReadLine();
                switch (switchArg)
                {
                    case "1":
                        runner.InitRunner();
                        break;
                    case "2":
                        Console.Clear();
                        server.listServers();
                        break;
                    case "x":
                        Environment.Exit(0);
                        break;
                    default:
                        Log.Warning("Bad option");
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
       
    }

    public ServiceProvider registerServices()
    {
        Log.Debug("Registering Services");
        return new ServiceCollection()
            .AddSingleton<IWeb, Web>()
            .AddSingleton<ILogging, Logging>()
            .AddSingleton<IRegistry, Registry>()
            .AddSingleton<IParsers, Parsers>()
            .AddSingleton<IConsoleUI, ProgressBar>()
            .AddSingleton<IServer, Utils.Server>()
            .BuildServiceProvider();
    }   
}