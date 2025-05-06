using MinecraftServer.Json;

namespace MinecraftServer.Interfaces;

public interface INet
{
    public void checkMojangServers();
    public void getVersionSpecificManifest(ManifestEntryStruct entry);
}