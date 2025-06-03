using System;
using System.ComponentModel.DataAnnotations;

namespace QuickMC.Json.JsonClasses;

public class ServerInfo
{
    public uint Ram { get; set; }
    public string path { get; set; }
    public string name { get; set; }
    public string version { get; set; }
    public bool firstRun { get; set; }
    [Key]
    public Guid guid { get; set; }
}