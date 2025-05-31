using QuickMC.Db;

public class DBController
{
    private readonly DatabaseFramework _context;

    public DBController(DatabaseFramework context)
    {
        _context = context;
    }
}