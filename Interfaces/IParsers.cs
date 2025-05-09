using MinecraftServer.Json;

namespace MinecraftServer.Interfaces;

public interface IParsers
{
    public DownloadManifestStruct parseMainManifestForVersion(string filename, DownloadManifestStruct serverDownload,
        string entry);
}