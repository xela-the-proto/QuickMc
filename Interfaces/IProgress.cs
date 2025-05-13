using System.Net;

namespace MinecraftServer.Interfaces;

public interface IProgress
{
    public Task<object> InitBarDownload(string item, HttpClient client, string url, string version = null);
}