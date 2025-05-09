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
    private int lastPercent = -1;
    private int lastPercentVer = -1;
    Konsole.ProgressBar pb = new Konsole.ProgressBar(PbStyle.DoubleLine,100);

    /// <summary>
    /// Other than actually checking, downloads the manifest with all the versions
    /// </summary>
    public void checkMojangServers()
    {
        try
        {
            var client = new WebClient();
            bool downloaded = false;
            client.DownloadProgressChanged += Program.progress.ManifestProgress;

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
    public DownloadManifestStruct getVersionSpecificManifest(ManifestEntryStruct entry)
    {
        DownloadManifestStruct serverDownload = null;
        string filename = string.Concat(entry.id.Replace(".", "-"), "_manifest.json");
        var client = new WebClient();
        client.DownloadProgressChanged += Program.progress.ManifestProgress;
        
        Task.WaitAll(client.DownloadFileTaskAsync(new Uri(entry.Url), filename));
        client.Dispose();
        File.Copy(filename, $"{Logging.path_root}/QuickMc" +
                                              $"/manifests/{filename}", true);
        File.Delete(filename);

        serverDownload = Program.jsonParsers.parseMainManifestForVersion(filename, serverDownload, entry.id);
        return serverDownload;
    }

    public void downloadServerJar(DownloadManifestStruct manifest)
    {
        if (!Directory.Exists($"{Logging.path_root}/QuickMc/Servers"))
        {
            Directory.CreateDirectory($"{Logging.path_root}/QuickMc/Servers");
        }
        var client = new WebClient();
        client.DownloadProgressChanged += Program.progress.ManifestProgress;

        Task.WaitAll(client.DownloadFileTaskAsync(new Uri(manifest.url), $"{manifest.id}.jar"));
        File.Copy($"{manifest.id}.jar", $"{Logging.path_root}/QuickMc" +
                                        $"/Servers/{manifest.id}.jar", true);
        File.Delete($"{manifest.id}.jar");
        client.Dispose();
    }

    
}