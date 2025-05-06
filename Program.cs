using System.Net.NetworkInformation;
using Microsoft.Extensions.DependencyInjection;
using MinecraftServer.Implement;
using MinecraftServer.Interfaces;
using MinecraftServer.Server;
using Serilog;

namespace MinecraftServer;

class Program
{
    static public ILogging logging;
    public static INet net;
    private static string title =
        "   ____        _      __   __  _________\n  / __ \\__  __(_)____/ /__/  |/  / ____/\n / / /" +
        " / / / / / ___/ //_/ /|_/ / /     \n/ /_/ / /_/ / / /__/ ,< / /  / / /___   " +
        "\n\\___\\_\\__,_/_/\\___/_/|_/_/  /_/\\____/";
    static async Task Main(string[] args)
    {
        var services =  new Program().registerServices();
        net = services.GetRequiredService<INet>();
        logging = services.GetRequiredService<ILogging>();
        logging.initLogging();
        new Program().Runner();
    }

    public void Runner()
    {
        string switchArg;
        Log.Information($"\n{title}");
        net.checkMojangServers();
        
        while (true)
        {
            Log.Information("\nOptions:\n1)Start a server");

            switchArg = Console.ReadLine();
            switch (switchArg)
            {
                case "1":
                    InstanceRunner.Runner();
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
            .BuildServiceProvider();
    }   
}