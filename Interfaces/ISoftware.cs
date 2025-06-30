using System.Threading.Tasks;
using QuickMC.Json.JsonClasses;

namespace QuickMC.Interfaces;

public interface ISoftware
{
    public Task ChooseSoftware();

    public ServerInfo BuildCustomSoftwareServerInfo(string software);
    
    public void DownloadFabricMainManifest();

    public void DownloadFabricVersionManifest();
    
}