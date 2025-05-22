using System;
using System.Diagnostics;
using System.Threading;
using QuickMC.Exceptions;
using QuickMC.Interfaces;
using Serilog;

namespace QuickMC.Utils;

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
            if (line == null)
            {
                throw new NoJavaException();
            }
            int start = line.IndexOf('"') + 1;
            int end = line.LastIndexOf('"');
            line = line.Substring(start, end - start);
            Log.Warning($"Found java version {line}");
        }
        catch (NoJavaException e)
        {
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