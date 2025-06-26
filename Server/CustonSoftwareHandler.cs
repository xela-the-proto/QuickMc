using System.Threading.Tasks;
using QuickMC.Interfaces;
using QuickMC.Json.JsonClasses;

namespace QuickMC.Server;

public class CustonSoftwareHandler : ISoftware
{
    public Task ChooseSoftware()
    {
        throw new System.NotImplementedException();
    }

    public ServerInfo BuildCustomSoftwareServerInfo()
    {
        throw new System.NotImplementedException();
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