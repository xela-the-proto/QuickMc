using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using QuickMC.Interfaces;
using Serilog;
using Spectre.Console;

namespace QuickMC.Utils;

public class ProgressBar : IConsoleUI
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

    public void DrawTable(List<string> columns, List<string> rows)
    {
        int i = 0;
        var table = new Table();
        table.HideHeaders();
        table.Border(TableBorder.Rounded);
        table.AddColumn("");
        table.AddColumn("");
        foreach (var row in rows)
        {
            table.AddRow(columns[i],row);
            i++;
        }
        
        AnsiConsole.Write(table);
    }
}