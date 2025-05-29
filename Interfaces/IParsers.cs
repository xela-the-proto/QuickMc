using QuickMC.Json;
using QuickMC.Json.JsonClasses;

namespace QuickMC.Interfaces;

public interface IParsers
{
    public DownloadManifestStruct parseMainManifestForVersion(string filename);
}