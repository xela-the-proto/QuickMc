using MinecraftServer.Interfaces;
using WindowsFirewallHelper;

namespace MinecraftServer.Utils;

public class Net :INet
{
    public void openPortW(IFirewallRule rule)
    {
        FirewallManager.Instance.Rules.Add(rule);
    }

    public void openPortU(int port)
    {
        throw new NotImplementedException();
    }

    public void closePortW(IFirewallRule rule)
    {
        FirewallManager.Instance.Rules.Remove(rule);
    }

    public void closePortU(int port)
    {
        throw new NotImplementedException();
    }

    public bool checkPortW(int port)
    {
        throw new NotImplementedException();
    }

    public bool checkPortU(int port)
    {
        throw new NotImplementedException();
    }

    public void WriteToPortDB()
    {
        throw new NotImplementedException();
    }

    public int ReadFromPortDB()
    {
        throw new NotImplementedException();
    }

    public IFirewallRule setupRuleW(int port, string path_to_exec, string server_guid)
    {
        var rule = FirewallManager.Instance.CreateApplicationRule(server_guid, FirewallAction.Allow, 
            path_to_exec);
        rule.Direction = FirewallDirection.Outbound;
        rule.Direction = FirewallDirection.Inbound;
        return rule;
    }
}