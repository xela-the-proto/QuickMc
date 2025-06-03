using System;
using System.Diagnostics;
using QuickMC.Json;
using QuickMC.Json.JsonClasses;

namespace QuickMC.Interfaces;

public interface IServer
{
    public void writeServerInfoToDir(string path, bool firstRun, string name, string version, Guid guid, uint ram);

    public void listServers();

    public void listServer(ServerInfo info);

    public void LogServerInfo(object obj, DataReceivedEventArgs args);
    public void LogServerError(object obj, DataReceivedEventArgs args);

}