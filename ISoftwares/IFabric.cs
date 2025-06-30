using System;
using System.Collections.Generic;
using System.Net.Http;
using QuickMC.Json.JsonClasses;

namespace QuickMC.ISoftwares;

public class IFabric : IBase
{
    public ServerInfo GetSoftwareMainManifest()
    {
        List<string> loaderList = new List<string>();
        string url = Program.config["AppSettings:FabricUrl"];
        
        Program.progress.InitBarDownloadFile("Downloading main fabric manifest",new HttpClient(),url,
            "FabricMainManifest.json");
        return null;
    }

    public ServerInfo GetSoftwareVersionManifest()
    {
        throw new System.NotImplementedException();
    }
}