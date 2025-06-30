using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using QuickMC.Db;
using QuickMC.Interfaces;
using QuickMC.Json.JsonClasses;
using QuickMC.Utils;
using Serilog;
using Spectre.Console;

namespace QuickMC.Net;

//TODO: this is a clusterfuck (i blame chatgpt bc i am shit at web and net stuff) need to redo it myself
//future xela here
//what the fuck
public class Web : IWeb
{
    public async Task<object> Download(HttpClient client, ProgressTask task, string url, string version = null)
    {
        try
        {
            bool isMainManifest = false;
            bool isServerVersionManifest = false;
            bool isJar = false;

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
                }
                else if (filename.Contains(".json") && filename != "version_manifest_v2.json")
                {

                    isServerVersionManifest = true;
                }
                else
                {
                    isMainManifest = true;
                }

                Log.Verbose($"Starting download of {filename} ({task.MaxValue} bytes)");

                Log.Verbose("Creating streams");
                var contentStream = await response.Content.ReadAsStreamAsync();
                using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write,
                           FileShare.None, 8192, true))
                {
                    var buffer = new byte[8192];
                    while (true)
                    {
                        var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                        if (read == 0)
                        {
                            Log.Verbose($"Download of [u]{filename}[/] [green]completed![/]");
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

                    //Get the version specific manifest (Ex. 1.21)
                    if (isServerVersionManifest)
                    {
                        Log.Verbose("File is a manifest");
                        File.Copy(filename, Logging.path_root + $"/QuickMc/manifests/{filename}"
                            , true);

                        var manifestStruct = Program.jsonParsers.parseMainManifestForVersion(filename);

                        return manifestStruct;
                    }

                    //Get the jar file
                    if (isJar)
                    {
                        var guid = Guid.NewGuid();
                        Log.Verbose("File is a jar");
                        if (!Directory.Exists(Logging.path_root + $"/QuickMc/Servers/{guid}"))
                        {
                            Directory.CreateDirectory(Logging.path_root + $"/QuickMc/Servers/{guid}");
                        }

                        File.Copy(filename, Logging.path_root + $"/QuickMc/Servers/{guid}/{filename}"
                            , true);
                        Log.Verbose("copied jar to folder");
                        ServerInfo serverInfo = new ServerInfo()
                        {
                            firstRun = true,
                            path = $"{Logging.path_root}/QuickMc/Servers/{guid}",
                            version = version,
                            guid = guid
                        };
                        return serverInfo;
                    }
                    if (isMainManifest)
                    {
                        if (!Directory.Exists(Logging.path_root + $"/QuickMc/manifests"))
                        {
                            Directory.CreateDirectory(Logging.path_root + $"/QuickMc/manifests");
                        }

                        File.Copy(filename, Logging.path_root + $"/QuickMc/manifests/{filename}", true);
                        Program.manifest = File.ReadAllText(Logging.path_root + $"/QuickMc/manifests/{filename}");
                    }
                }

                contentStream.Close();
            }
        }
        catch (Exception e)
        {
            Log.Fatal(e.Message);
            throw;
        }

        return null;
    }
    
     public async Task<object> DownloadFile(HttpClient client, ProgressTask task, string url, string filename)
    {
        try
        {
            bool isMainManifest = false;
            bool isServerVersionManifest = false;
            bool isJar = false;

            using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                Log.Verbose("Checking success in GET");
                response.EnsureSuccessStatusCode();

                // Set the max value of the progress task to the number of bytes
                task.MaxValue(response.Content.Headers.ContentLength ?? 0);
                // Start the progress task
                Log.Verbose("Starting progressabr task");
                task.StartTask();
                
                Log.Verbose($"Starting download of {filename} ({task.MaxValue} bytes)");

                Log.Verbose("Creating streams");
                var contentStream = await response.Content.ReadAsStreamAsync();
                using (var fileStream = new FileStream(Path.GetTempPath() + filename, FileMode.Create, FileAccess.Write,
                           FileShare.None, 8192, true))
                {
                    var buffer = new byte[8192];
                    while (true)
                    {
                        var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                        if (read == 0)
                        {
                            Log.Verbose($"Download of [u]{filename}[/] [green]completed![/]");
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
                }

                contentStream.Close();
            }
        }
        catch (Exception e)
        {
            Log.Fatal(e.Message);
            throw;
        }

        return null;
    }
}