using QuickMC.Json;
using QuickMC.Json.Json_classes;

namespace QuickMC.Interfaces;

public interface IParsers
{
    public DownloadManifestStruct parseMainManifestForVersion(string filename);
}