using System.Threading.Tasks;
using QuickMC.Interfaces;
using QuickMC.ISoftwares;
using QuickMC.Json.JsonClasses;
using Spectre.Console;

namespace QuickMC.Server;

public class CustomSoftwareHandler : ISoftware
{
    public Task ChooseSoftware()
    {
        var optPrompt = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\nChoose which software to use (red for experimental downlaod support, " +
                       "yellow for still not completed" +
                       "Green for supported")
                .PageSize(5)
                .AddChoices(new[] {
                    "[Green]Vanilla[/]", "[Red]Fabric[/]", "[Red]Forge[/]"
                }));
        var switchArg
            = optPrompt.ToString();
        BuildCustomSoftwareServerInfo(switchArg.Substring(switchArg.IndexOf(']')));
        return null;
    }

    public ServerInfo BuildCustomSoftwareServerInfo(string software)
    {
        ServerInfo info = new ServerInfo();
        switch (software)
        {
            case "Vanilla":
                break;
            case "Fabric":
                new IFabric().GetSoftwareMainManifest();
                break;
            case "Forge":
                break;
        }
        return info;
    }

    public void DownloadFabricMainManifest()
    {
        throw new System.NotImplementedException();
    }

    public void DownloadFabricVersionManifest()
    {
        throw new System.NotImplementedException();
    }
}