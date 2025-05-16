namespace MinecraftServer.Json;

public class ServerInfo
{
    public string path { get; set; }
    public string name { get; set; }
    public string version { get; set; }
    public bool firstRun { get; set; }
    
    public Guid guid { get; set; }
}