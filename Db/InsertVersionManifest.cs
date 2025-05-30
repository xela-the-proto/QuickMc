using System.Linq;
using QuickMC.Json.JsonClasses;
using QuickMC.Utils;

namespace QuickMC.Db;

public class InsertVersionManifest
{
    public static object checkSha256(SingleVersionEntryStruct version)
    {
        //if the sha doesnt exist cache in the server
        using (var context = new DatabaseFramework())
        {
            var entry = context.jarEntry.Where(x => x.JsonManifestSha256 == version.Sha1).First();
            if (entry != null)
            {
                return entry;
            }
        }
        return false;
    }

    public static void storeOnDb(DownloadManifestStruct version)
    {
        using (var context = new DatabaseFramework())
        {
            
                context.jarEntry.Add(version);
                context.SaveChanges();
        }
    }
}