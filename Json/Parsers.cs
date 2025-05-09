using MinecraftServer.Implement;
using MinecraftServer.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace MinecraftServer.Json;

public class Parsers : IParsers
{
    public DownloadManifestStruct parseMainManifestForVersion(string filename, DownloadManifestStruct serverDownload,
        string entry)
    {
        
        var obj = JObject.Parse(File.ReadAllText
            (Logging.path_root + $"/QuickMc/manifests/{filename}")) ?? throw new FileNotFoundException();
        var serverJson = obj["downloads"]?["server"];
        if (serverJson != null)
        {
            serverDownload = JsonConvert.DeserializeObject<DownloadManifestStruct>(serverJson.ToString()) ?? 
                             throw new FormatException("bad manifest format!");
            serverDownload.id = entry;
            Log.Debug($"Parsed uri {serverDownload.url}");
        }

        return serverDownload;
    }
}