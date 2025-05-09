using MinecraftServer.Interfaces;
using Serilog;

namespace MinecraftServer.Implement;

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
        Log.Logger = new LoggerConfiguration().WriteTo.File(path + "log.txt").WriteTo.Console().
            MinimumLevel.Debug().CreateLogger();
    }

    public void saveTrackingInfo()
    {
        
    }
}