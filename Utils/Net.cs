using System.Net;
using MinecraftServer.Interfaces;
using MinecraftServer.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Spectre.Console;

namespace MinecraftServer.Implement;

public class Net : INet
{
    /*
    private int lastPercent = -1;
    private int lastPercentVer = -1;

    /// <summary>
    /// Other than actually checking, downloads the manifest with all the versions
    /// </summary>
    public void checkMojangServers()
    {
        try
        {
            var client = new WebClient();
            bool downloaded = false;
            Task.Run(() => Program.progress.InitBar());
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
        Task.Run(() => Program.progress.InitBar());
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
        var client = new HttpClient();

        File.Copy($"{manifest.id}.jar", $"{Logging.path_root}/QuickMc" +
                                        $"/Servers/{manifest.id}.jar", true);
        File.Delete($"{manifest.id}.jar");
        client.Dispose();
    }
    */

    public async Task<object> Download(HttpClient client, ProgressTask task, string url, string version  = null)
    {
        try
        {
            bool isInfo = false;
            bool isJar = false;
            ServerInfo info = null;
            DownloadManifestStruct manifestEntry = null;
            using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                Log.Verbose("Checking success in GET");
                response.EnsureSuccessStatusCode();
                    
                // Set the max value of the progress task to the number of bytes
                task.MaxValue(response.Content.Headers.ContentLength ?? 0);
                // Start the progress task
                Log.Verbose("Starting progressabr task");
                task.StartTask();

                var filename = url.Substring(url.LastIndexOf('/') + 1);
                Log.Verbose("checking type of file for serialization after");
                if (filename.Contains(".jar"))
                {
                    isJar = true;
                }else if (filename.Contains(".json") && filename != "version_manifest_v2.json")
                {
                    isInfo = true;
                }
                Log.Information($"Starting download of [u]{filename}[/] ({task.MaxValue} bytes)");
                
                Log.Verbose("Creating streams");
                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None,
                           8192, true))
                {
                    var buffer = new byte[8192];
                    while (true)
                    {
                        var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                        if (read == 0)
                        {
                            AnsiConsole.MarkupLine($"Download of [u]{filename}[/] [green]completed![/]");
                            break;
                        }

                        // Increment the number of read bytes for the progress task
                        task.Increment(read);

                        // Write the read bytes to the output stream
                        await fileStream.WriteAsync(buffer, 0, read);
                        
                    }

                    Log.Verbose("Closing filestream and http client");
                    client.Dispose();
                    fileStream.Close();

                    if (isInfo)
                    {
                        Log.Verbose("File is a manifest");
                        File.Copy(filename, Logging.path_root + $"/QuickMc/manifests/{filename}"
                            ,true);
                        var manifestStruct = Program.jsonParsers.parseMainManifestForVersion(filename);
                        return manifestStruct;
                    }
                    if (isJar)
                    {
                        Log.Verbose("File is a jar");
                        if (!Directory.Exists(Logging.path_root + $"/QuickMc/Servers/{filename}"))
                        {
                            Directory.CreateDirectory(Logging.path_root + $"/QuickMc/Servers/{version}");
                        }
                        File.Copy(filename, Logging.path_root + $"/QuickMc/Servers/{version}/{filename}"
                            ,true);
                        Log.Debug("copied jar to folder");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Information(e.Message);
        }

        return null;
    }
    public async Task DownloadMainManifest(HttpClient client, ProgressTask task, string url)
    {
        try
        {
            using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                // Set the max value of the progress task to the number of bytes
                task.MaxValue(response.Content.Headers.ContentLength ?? 0);
                // Start the progress task
                task.StartTask();

                var filename = url.Substring(url.LastIndexOf('/') + 1);
                AnsiConsole.MarkupLine($"Starting download of [u]{filename}[/] ({task.MaxValue} bytes)");

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None,
                           8192, true))
                {
                    var buffer = new byte[8192];
                    while (true)
                    {
                        var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                        if (read == 0)
                        {
                            AnsiConsole.MarkupLine($"Download of [u]{filename}[/] [green]completed![/]");
                            break;
                        }

                        // Increment the number of read bytes for the progress task
                        task.Increment(read);

                        // Write the read bytes to the output stream
                        await fileStream.WriteAsync(buffer, 0, read);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Information(e.Message);
        }
    }
}
