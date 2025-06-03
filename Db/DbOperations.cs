using System.Linq;
using Microsoft.EntityFrameworkCore;
using QuickMC.Interfaces;
using QuickMC.Json.JsonClasses;
using Spectre.Console;

namespace QuickMC.Db;

public class DbOperations : IDb
{
    private static bool spin = true;
    public void DbOnSavingChanges(object? sender, SavingChangesEventArgs e)
    {
        AnsiConsole.Status().Spinner(Spinner.Known.BouncingBall).StartAsync("Syncing with database", x =>
        {
            while (spin)
            {
                
            }
            return null;
        });
    }

    public void DbOnSavedChanges(object? sender, SavedChangesEventArgs e)
    {
        spin = false;
    }

    public object checkSha256(SingleVersionEntryStruct version)
    {
        //if the sha doesnt exist cache in the server
        using (var context = new DatabaseFramework())
        {
            var entry = context.jarEntry.Where(x => x.JsonManifestSha256 == version.Sha1).FirstOrDefault();
            if (entry != null)
            {
                return entry;
            }
        }
        return null;
    }

    public void storeOnDb(DownloadManifestStruct version)
    {
        using (var context = new DatabaseFramework())
        {
            
            context.jarEntry.Add(version);
            context.SaveChanges();
        }
        
    }
}