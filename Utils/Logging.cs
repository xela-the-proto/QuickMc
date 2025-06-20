﻿using System;
using System.IO;
using QuickMC.Interfaces;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SpectreConsole;

namespace QuickMC.Utils;

//apart from the path and the init this can remain untouched

public class Logging :ILogging
{
    public static string path_root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    /// <summary>
    /// Initializes the logging by setting Serilog to log to file and console
    /// </summary>
    public void initLogging()
    {
        if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            path_root = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        }
        string path = Path.Combine(path_root + "/QuickMc");
        Log.Warning(path);
        Directory.CreateDirectory(path);
        Log.Logger = new LoggerConfiguration().WriteTo.File(Path.Combine(path,"log.txt")).WriteTo.SpectreConsole("{Timestamp:HH:mm:ss} [{Level:u4}] {Message:lj}{NewLine}{Exception}", minLevel: LogEventLevel.Information)
            .WriteTo.Console().
            MinimumLevel.Information().CreateLogger();
    }
    
}