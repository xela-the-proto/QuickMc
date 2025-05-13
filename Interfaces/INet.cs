using MinecraftServer.Json;
using Spectre.Console;

namespace MinecraftServer.Interfaces;

public interface INet
{
    public Task<object> Download(HttpClient client, ProgressTask task, string url, string version = null);
    public Task DownloadMainManifest(HttpClient client, ProgressTask task, string url);


}