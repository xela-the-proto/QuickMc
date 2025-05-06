using System.Net;
using Konsole;
using MinecraftServer.Interfaces;
using Serilog;

namespace MinecraftServer.Implement;

public class Net : INet
{
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
            File.Copy("version_manifest_v2.json", $"{Logging.path_root}/QuickMc" +
                                                  $"/manifests/version_manifest_v2.json", true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }  
    }

    private void ProgressBar(object sender, DownloadProgressChangedEventArgs args)
    {
        var pb = new ProgressBar(PbStyle.DoubleLine, (int)args.TotalBytesToReceive);
        pb.Refresh((int)args.BytesReceived,"Downloading updated manifest");
        Console.ReadLine();
    }
}