using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickMC.Db;
using QuickMC.Interfaces;
using QuickMC.Json;
using QuickMC.Net;
using QuickMC.Server;
using QuickMC.Utils;
using Serilog;
using Spectre.Console;

namespace QuickMC;

class Program
{
    //TODO: fix the lowercase and the whole cluster fuck of service registering
#pragma warning disable CS8618
    public static string manifest;
    public static DatabaseFramework db;
    public static ILogging logging;
    public static IConfigurationRoot config;
    public static IWeb web;
    public static IParsers jsonParsers;
    public static IConsoleUI progress;
    public static IServer server;
    public static INet net;
    public static IDb dbOp;
#pragma warning restore CS8618

    
    private static string title =
        "   ____        _      __   __  _________\n  / __ \\__  __(_)____/ /__/  |/  / ____/\n / / /" +
        " / / / / / ___/ //_/ /|_/ / /     \n/ /_/ / /_/ / / /__/ ,< / /  / / /___   " +
        "\n\\___\\_\\__,_/_/\\___/_/|_/_/  /_/\\____/";
    static async Task Main(string[] args)
    {
        var appSettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var services =  new Program().registerServices();
        logging = services.GetRequiredService<ILogging>();
        logging.initLogging();
        db = new DatabaseFramework();
        db.Database.EnsureCreated();

        config = appSettings;
        web = services.GetRequiredService<IWeb>();
        jsonParsers = services.GetRequiredService<IParsers>();
        progress = services.GetRequiredService<IConsoleUI>();
        server = services.GetRequiredService<IServer>();
        net = services.GetRequiredService<INet>();
        dbOp = services.GetRequiredService<IDb>();
        //db.SavingChanges += dbOp.DbOnSavingChanges;
        //db.SavedChanges += dbOp.DbOnSavedChanges;
        
        //get the main manifest
        await progress.InitBarDownload("Downloading main manifest",
            new HttpClient(),config["AppSettings:MojangUrl"] );
        
        Log.Verbose("got main manifest and initted logging");
        Log.Verbose("Running main while loop");
        new Program().Runner();
    }

    public void Runner()
    {
        try
        {
            var creator = new InstanceCreator();
            var runner = new instanceRunner();
            Log.Information($"\n{title}");
            while (true)
            {
                var optPrompt = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("\n[red]Options:[/]")
                        .PageSize(5)
                        .AddChoices(new[] {
                            "1)Create server","2)Start a created server","3)List servers","x)exit"
                        }));
                var switchArg = optPrompt[0];

                switch (switchArg)
                {
                    case '1':
                        creator.Create();
                        break;
                    case '2':
                        runner.Runner();
                        break;
                    case '3':
                        server.listServers();
                        break;
                    case 'x':
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
            .AddSingleton<IParsers, Parsers>()
            .AddSingleton<IConsoleUI, ProgressBar>()
            .AddSingleton<IServer, Utils.Server>()
            .AddDbContext<DatabaseFramework>()
            .AddSingleton<INet,Network.Net>()
            .AddSingleton<IDb,DbOperations>()
            .BuildServiceProvider();
    }   
}