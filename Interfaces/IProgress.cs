using System.Net;

namespace MinecraftServer.Interfaces;

public interface IProgress
{
    public void ManifestProgress(object sender, DownloadProgressChangedEventArgs args);
    public void ProgressBarVersion(object sender, DownloadProgressChangedEventArgs args);
}