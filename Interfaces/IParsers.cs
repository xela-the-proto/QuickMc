using MinecraftServer.Json;

namespace MinecraftServer.Interfaces;

public interface IParsers
{
    public DownloadManifestStruct parseMainManifestForVersion(string filename);
}