using MinecraftServer.Json;

namespace MinecraftServer.Interfaces;

public interface INet
{
    public void checkMojangServers();
    public DownloadManifestStruct getVersionSpecificManifest(ManifestEntryStruct entry);
    public void downloadServerJar(DownloadManifestStruct manifest);
}