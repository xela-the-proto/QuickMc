using System.Net;
using Konsole;
using MinecraftServer.Interfaces;
using MinecraftServer.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace MinecraftServer.Implement;

public class Net : INet
{
    /// <summary>
    /// Other than actually checking, downloads the manifest with all the versions
    /// </summary>
    public void checkMojangServers()
    {
        try
        {
            var client = new WebClient();
            bool downloaded = false;
            client.DownloadProgressChanged += ProgressBar;

            if (!Directory.Exists($"{Logging.path_root}/QuickMc/manifests"))
            {
                Directory.CreateDirectory($"{Logging.path_root}/QuickMc/manifests");
            }
            
            Task.WaitAll(client.DownloadFileTaskAsync(new Uri("https://piston-meta.mojang.com/mc/game/version_manifest_v2.json"), 
                "version_manifest_v2.json"));
            client.Dispose();
            
            File.Copy("version_manifest_v2.json", $"{Logging.path_root}/QuickMc" +
                                                  $"/manifests/version_manifest_v2.json", true);
            File.Delete("version_manifest_v2.json");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }  
    }
    
    /// <summary>
    /// Downloads a specific minecraft version jar
    /// </summary>
    /// <param name="entry">ManifestStructEntry taken from the main manifest</param>
    public void getVersionSpecificManifest(ManifestEntryStruct entry)
    {
        string filename = string.Concat(entry.id.Replace(".", "-"), "_manifest.json");
        var client = new WebClient();
        client.DownloadProgressChanged += ProgressBar;
        
        Task.WaitAll(client.DownloadFileTaskAsync(new Uri(entry.Url), filename));
        client.Dispose();
        File.Copy(filename, $"{Logging.path_root}/QuickMc" +
                                              $"/manifests/{filename}", true);
        File.Delete(filename);
        var obj = JObject.Parse(File.ReadAllText
            (Logging.path_root + $"/QuickMc/manifests/{filename}")) ?? throw new FileNotFoundException();
        var serverJson = obj["downloads"]?["server"];
        if (serverJson != null)
        {
            var serverDownload = JsonConvert.DeserializeObject<DownloadManifestStruct>(serverJson.ToString());
            Log.Debug($"Parsed uri {serverDownload.url}");
        }
    }

    private void ProgressBar(object sender, DownloadProgressChangedEventArgs args)
    {
        var pb = new ProgressBar(PbStyle.DoubleLine, (int)args.TotalBytesToReceive);
        pb.Refresh((int)args.BytesReceived,"Downloading updated manifest");
    }
}