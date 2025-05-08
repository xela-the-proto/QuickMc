using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Win32;
using MinecraftServer.Interfaces;
using Serilog;

namespace MinecraftServer.Implement;

public class Registry : IRegistry
{
    /// <summary>
    /// CheckJava for linux systems
    /// </summary>
    public void CheckJava()
    {
        try
        {
            Process process = new Process();
            process.StartInfo.FileName = "java";
            process.StartInfo.ArgumentList.Add("-version");
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
        
            var line = process.StandardError.ReadLine();
            int start = line.IndexOf('"') + 1;
            int end = line.LastIndexOf('"');
            line = line.Substring(start, end - start);
            Log.Warning($"Found java version {line}");
        }
        catch (Win32Exception e)
        {
            Log.Fatal("No java runtime found! quitting!");
            Thread.Sleep(1000);
            Environment.Exit(1);
            throw;
        }
    }

    public void CheckJavaForVersion(string version)
    {
        throw new NotImplementedException();
    }
}