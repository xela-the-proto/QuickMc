using System;
using Microsoft.EntityFrameworkCore;
using QuickMC.Json.JsonClasses;

namespace QuickMC.Interfaces;

public interface IDb
{
    //public void DbOnSavingChanges(object? sender, SavingChangesEventArgs e);
    
    //public void DbOnSavedChanges(object? sender, SavedChangesEventArgs e);

    public object checkSha256(SingleVersionEntryStruct version);

    public  void storeOnDb(DownloadManifestStruct version);
}