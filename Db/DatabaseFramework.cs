using System.Data.SQLite;
using System.IO;
using System.Net;
using Microsoft.EntityFrameworkCore;
using QuickMC.Json.JsonClasses;
using QuickMC.Utils;

namespace QuickMC.Db;


public class DatabaseFramework : DbContext
{
    public DbSet<ServerInfo> server { get; set; }
    public DbSet<DownloadManifestStruct> jarEntry { get; set; }
    public string DbPath { get; } = Logging.path_root + "/QuickMc/Db.sqlite";
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!File.Exists(DbPath))
        {
            SQLiteConnection.CreateFile(DbPath);
        }
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }
}
