using QuickMC.Json.JsonClasses;

namespace QuickMC.ISoftwares;

public interface IBase
{
    public ServerInfo GetSoftwareMainManifest();
    public ServerInfo GetSoftwareVersionManifest();
}