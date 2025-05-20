using MinecraftServer.Interfaces;
using MinecraftServer.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace MinecraftServer.Json;

public class Parsers : IParsers
{
    public DownloadManifestStruct parseMainManifestForVersion(string filename)
    {
        DownloadManifestStruct serverDownload = null;   
        var obj = JObject.Parse(File.ReadAllText
            (Path.Combine(Logging.path_root + $"/QuickMc/manifests/{filename}"))) ?? throw new FileNotFoundException();
        var serverJson = obj["downloads"]?["server"];
        if (serverJson != null)
        {
            serverDownload = JsonConvert.DeserializeObject<DownloadManifestStruct>(serverJson.ToString()) ?? 
                             throw new FormatException("bad manifest format!");
            serverDownload.id = filename.Remove(filename.LastIndexOf('.'), filename.Length - filename.LastIndexOf('.'));
            Log.Debug($"Parsed uri {serverDownload.url}");
        }

        return serverDownload;
    }
}