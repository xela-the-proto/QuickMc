using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace QuickMC.Json.JsonClasses;

//TODO: The naming is all over the place
/// <summary>
/// The main class to access the Manifest where mojang stores all of the versions 
/// </summary>
public class MainVersionManifest
{
    public LatestVersionsStruct latest;
    public List<SingleVersionEntryStruct> versions;
}

/// <summary>
/// Struct for the actual version .jar download
/// </summary>
public class DownloadManifestStruct
{
    [Key]
    public string id { get; set; }
    public int size { get; set; }
    public string url { get; set; }
    public string JsonManifestSha256 { get; set; }
    public byte[] JarFile { get; set; }
}

/// <summary>
/// Single entry in the main manifest
/// </summary>
public class SingleVersionEntryStruct
{
    [Key]
    public string id { get; set; }
    public string type { get; set; }
    public String Url { get; set; }
    public string Sha1 { get; set; }
}
/// <summary>
/// Latest version in the main manifest
/// </summary>
public struct LatestVersionsStruct
{
    public string release { get; set; }
    public string snapshot { get; set; }
}