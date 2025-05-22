using WindowsFirewallHelper;

namespace QuickMC.Interfaces;

public interface INet
{
    public void openPortW(IFirewallRule item);
    
    public void openPortU(int port);
    
    public void closePortW(IFirewallRule item);
    
    public void closePortU(int port);
    
    public bool checkPortW(int port);
    
    public bool checkPortU(int port);

    public void WriteToPortDB();
    
    public int ReadFromPortDB();
    
    public IFirewallRule setupRuleW(int port, string path_to_exec, string server_guid);
}