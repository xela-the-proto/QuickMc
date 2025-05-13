using System.Net;
using Serilog;
using Spectre.Console;
using IProgress = MinecraftServer.Interfaces.IProgress;

namespace MinecraftServer.Implement;

public class ProgressBar : IProgress
{
    public async Task<object> InitBarDownload(string item, HttpClient client, string url, string version = null)
    {
        object result = null;
        Log.Verbose("Init progressbar");
        await AnsiConsole.Progress()
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn(),
            })
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask(item, new ProgressTaskSettings
                {
                        AutoStart = false
                });
                if (version != null)
                {
                    result = await Program.net.Download(client, task, url,version);
                }
                else
                {
                    result = await Program.net.Download(client, task, url);
                }
            });
        if (result != null)
        {
            Log.Verbose($"Return type is {result.ToString()}");
            return result;
        }
        else
        {
            return null;
        }
    }
}