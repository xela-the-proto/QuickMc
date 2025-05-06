using MinecraftServer.Interfaces;
using Serilog;

namespace MinecraftServer.Implement;

public class Logging :ILogging
{
    public static string path_root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    public void initLogging()
    {
        string path = path_root + "/QuickMc";
        Log.Warning(path);
        Directory.CreateDirectory(path);
        Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File(path + "log.txt").CreateLogger();
    }

    public void saveTrackingInfo()
    {
        
    }
}