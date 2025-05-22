using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using QuickMC.Interfaces;
using QuickMC.Json.Json_classes;
using QuickMC.Utils;
using Serilog;
using Spectre.Console;

namespace QuickMC.Network;

public class Web : IWeb
{
    public async Task<object> Download(HttpClient client, ProgressTask task, string url, string version  = null)
    {
        try
        {
            bool isManifest = false;
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
                else
                {
                    isManifest = true;
                }
                Log.Information($"Starting download of {filename} ({task.MaxValue} bytes)");
                
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
                        File.Copy(filename,Path.Combine(Logging.path_root ,$"/QuickMc/manifests/{filename}")
                            ,true);
                        var manifestStruct = Program.jsonParsers.parseMainManifestForVersion(filename);
                        return manifestStruct;
                    }
                    if (isJar)
                    {
                        var guid = Guid.CreateVersion7();
                        Log.Verbose("File is a jar");
                        if (!Directory.Exists(Path.Combine(Logging.path_root + $"/QuickMc/Servers/{guid}")))
                        {
                            Directory.CreateDirectory(Path.Combine(Logging.path_root + $"/QuickMc/Servers/{guid}"));
                        }
                        File.Copy(filename, Logging.path_root + $"/QuickMc/Servers/{guid}/{filename}"
                            ,true);
                        Log.Debug("copied jar to folder");
                        ServerInfo serverInfo = new ServerInfo()
                        {
                            firstRun = true,
                            path =  $"{Logging.path_root}/QuickMc/Servers/{guid}",
                            version = version,
                            guid = guid
                        };
                        return serverInfo;
                    }

                    if (isManifest)
                    {
                        if (!Directory.Exists(Logging.path_root + $"/QuickMc/manifests"))
                        {
                            Directory.CreateDirectory(Logging.path_root + $"/QuickMc/manifests");
                        }
                        File.Copy(filename, Logging.path_root + $"/QuickMc/manifests/{filename}");
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
