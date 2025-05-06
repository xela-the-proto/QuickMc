namespace MinecraftServer.Json;

/// <summary>
/// The main class to access the Manifest where mojang stores all of the versions 
/// </summary>
public class ManifestStruct
{
    public ManifestLatestStruct latest;
    public List<ManifestEntryStruct> versions;
}

/// <summary>
/// Struct for the actual version .jar download
/// </summary>
public class DownloadManifestStruct
{
    public string sha1 { get; set; }
    public int size { get; set; }
    public string url { get; set; }
}

/// <summary>
/// Single entry in the main manifest
/// </summary>
public struct ManifestEntryStruct
{
    public string id { get; set; }
    public string type { get; set; }
    public String Url { get; set; }
}
/// <summary>
/// Latest version in the main manifest
/// </summary>
public struct ManifestLatestStruct
{
    public string release { get; set; }
    public string snapshot { get; set; }
}