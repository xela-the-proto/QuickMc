using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace QuickMC.Interfaces;

public interface IConsoleUI
{
    public Task<object> InitBarDownload(string item, HttpClient client, string url, string version = null);

    public void DrawTable(List<string> columns, List<string> rows);
}