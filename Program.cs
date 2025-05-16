using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinecraftServer.Utils;
using MinecraftServer.Interfaces;
using MinecraftServer.Json;
using MinecraftServer.Server;
using Serilog;

namespace MinecraftServer;

class Program
{
    public static ILogging logging;
    public static IConfigurationRoot _config;
    public static INet net;
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
        net = services.GetRequiredService<INet>();
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
        string switchArg;
        Log.Information($"\n{title}");
        while (true)
        {
            Log.Information("\nOptions:\n1)Start a server\n2)List current servers");
            switchArg = Console.ReadLine();
            switch (switchArg)
            {
                case "1":
                    InstanceRunner.InitRunner();
                    break;
                case "2":
                    Console.Clear();
                    server.listServers();
                    break;
                default:
                    Environment.Exit(1);
                    break;
            }
        }
    }

    public ServiceProvider registerServices()
    {
        Log.Debug("Registering Services");
        return new ServiceCollection()
            .AddSingleton<INet, Net>()
            .AddSingleton<ILogging, Logging>()
            .AddSingleton<IRegistry, Registry>()
            .AddSingleton<IParsers, Parsers>()
            .AddSingleton<IConsoleUI, ProgressBar>()
            .AddSingleton<IServer, Utils.Server>()
            .BuildServiceProvider();
    }   
}