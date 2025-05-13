using Microsoft.Extensions.DependencyInjection;
using MinecraftServer.Implement;
using MinecraftServer.Interfaces;
using MinecraftServer.Json;
using MinecraftServer.Server;
using Serilog;
using Spectre.Console;
using Spectre.Console.Rendering;
using IProgress = MinecraftServer.Interfaces.IProgress;
using ProgressBar = MinecraftServer.Implement.ProgressBar;

namespace MinecraftServer;

class Program
{
    public static ILogging logging;
    public static INet net;
    public static IRegistry registry;
    public static IParsers jsonParsers;
    public static IProgress progress;
    private static string title =
        "   ____        _      __   __  _________\n  / __ \\__  __(_)____/ /__/  |/  / ____/\n / / /" +
        " / / / / / ___/ //_/ /|_/ / /     \n/ /_/ / /_/ / / /__/ ,< / /  / / /___   " +
        "\n\\___\\_\\__,_/_/\\___/_/|_/_/  /_/\\____/";
    static async Task Main(string[] args)
    {
        
        var services =  new Program().registerServices();
        net = services.GetRequiredService<INet>();
        logging = services.GetRequiredService<ILogging>();
        registry = services.GetRequiredService<IRegistry>();
        jsonParsers = services.GetRequiredService<IParsers>();
        progress = services.GetRequiredService<IProgress>();
        
        //get the main manifest
        await progress.InitBarDownload("Downloading main manifest",
            new HttpClient(), "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json");
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
            Log.Information("\nOptions:\n1)Start a server");
            switchArg = Console.ReadLine();
            switch (switchArg)
            {
                case "1":
                    InstanceRunner.InitRunner();
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
            .AddSingleton<IProgress, ProgressBar>()
            .BuildServiceProvider();
    }   
}