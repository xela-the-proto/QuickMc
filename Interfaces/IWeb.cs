using System.Net.Http;
using System.Threading.Tasks;
using Spectre.Console;

namespace QuickMC.Interfaces;

public interface IWeb
{
    public Task<object> Download(HttpClient client, ProgressTask task, string url, string version = null);
    public Task DownloadMainManifest(HttpClient client, ProgressTask task, string url);


}