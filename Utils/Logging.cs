using MinecraftServer.Interfaces;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SpectreConsole;

namespace MinecraftServer.Utils;

public class Logging :ILogging
{
    public static string path_root = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

    /// <summary>
    /// Initializes the logging by setting Serilog to log to file and console
    /// </summary>
    public void initLogging()
    {
        string path = path_root + "/QuickMc";
        Log.Warning(path);
        Directory.CreateDirectory(path);
        Log.Logger = new LoggerConfiguration().WriteTo.File(path + "log.txt").WriteTo.SpectreConsole("{Timestamp:HH:mm:ss} [{Level:u4}] {Message:lj}{NewLine}{Exception}", minLevel: LogEventLevel.Information)
            .WriteTo.Console().
            MinimumLevel.Verbose().CreateLogger();
    }

    public void saveTrackingInfo()
    {
        
    }
}