using System.Diagnostics;
using MinecraftServer.Json;

namespace MinecraftServer.Interfaces;

public interface IServer
{
    public void writeServerInfoToDir(string path, bool firstRun, string name, string version, Guid guid);

    public void listServers();

    public void LogServerInfo(object obj, DataReceivedEventArgs args);
    public void LogServerError(object obj, DataReceivedEventArgs args);

}