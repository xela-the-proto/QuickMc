using MinecraftServer.Implement;
using Serilog;

namespace MinecraftServer.Server;

public class InstanceRunner
{
    /// <summary>
    /// Starts a minecraft instance asking a few questions first
    /// </summary>
    public static void Runner()
    {
        var builder = new ServerInstanceBuilder();
        Log.Information("Insert which version should the manager get:");
        string version = Console.ReadLine();
        Log.Information("Insert how much ram should the server use in Mb:");
        int ram = Convert.ToInt32(Console.ReadLine());
        
        builder.setVersion(version);
        builder.setRam(4098);
        var instance = builder.BuildInstance();
        
        var manifest = ManifestSingleton.GetInstance();
        var entry = manifest.versions.Find(x => x.id == instance.Version);
        
        Program.net.getVersionSpecificManifest(entry);
        
    }
}