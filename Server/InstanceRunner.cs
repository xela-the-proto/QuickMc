using Serilog;

namespace MinecraftServer.Server;

public class InstanceRunner
{
    public static void Runner()
    {
        var builder = new ServerInstanceBuilder();
        Log.Information("Insert which version should the manager get:");
        string version = Console.ReadLine();
        Log.Information("Insert how much ram shoudl the server use in Mb:");
        int ram = Convert.ToInt32(Console.ReadLine());
        
        builder.setVersion(version);
        builder.setRam(4098);
        var instance = builder.BuildInstance();
        
        
    }
}