using System.Net;
using Konsole;
using IProgress = MinecraftServer.Interfaces.IProgress;

namespace MinecraftServer.Implement;

public class ProgressBar : IProgress
{
    Konsole.ProgressBar pb = new Konsole.ProgressBar(Program.window, 50);
    public void ManifestProgress(object sender, DownloadProgressChangedEventArgs args)
    {
        // Set Max only once
        if (pb.Max != (int)args.TotalBytesToReceive && args.TotalBytesToReceive > 0)
        {
            pb.Max = (int)args.TotalBytesToReceive;
        }
        
        pb.Refresh((int)args.BytesReceived, $"Downloading main manifest");
    }
    
    public void ProgressBarVersion(object sender, DownloadProgressChangedEventArgs args)
    {
        // Set Max only once
        if (pb.Max != (int)args.TotalBytesToReceive && args.TotalBytesToReceive > 0)
        {
            pb.Max = (int)args.TotalBytesToReceive / 1000000;
        }
        pb.Refresh(args.ProgressPercentage, "");
    }
}