using System.Net;

namespace MinecraftServer.Interfaces;

public interface IConsoleUI
{
    public Task<object> InitBarDownload(string item, HttpClient client, string url, string version = null);

    public void DrawTable(List<string> columns, List<string> rows);
}