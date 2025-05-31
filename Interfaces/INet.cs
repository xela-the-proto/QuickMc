
using NetFwTypeLib;

namespace QuickMC.Interfaces;

public interface INet
{
    public void openPortW(string appName, string appPath);
    
    public void openPortU(int port);
    
    public void closePortW(INetFwRule item);
    
    public void closePortU(int port);
    
    public bool checkPortW(int port);
    
    public bool checkPortU(int port);

    public void WriteToPortDB();
    
    public int ReadFromPortDB();
    
    public INetFwRule setupRuleW(int port, string path_to_exec, string server_guid);
}