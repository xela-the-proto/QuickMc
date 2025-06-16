using System;
using System.Diagnostics.CodeAnalysis;
using NetFwTypeLib;
using QuickMC.Interfaces;


namespace QuickMC.Network;

// So the compiler doesn't bother me with unreachable call sites
[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class Net :INet
{
    public void openPortW(string appName, string appPath)
    {
        INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
            Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

        INetFwRule appRule = (INetFwRule)Activator.CreateInstance(
            Type.GetTypeFromProgID("HNetCfg.FWRule"));

        appRule.Name = appName;
        appRule.ApplicationName = appPath; // ✅ full path to the executable
        appRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
        appRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
        appRule.Enabled = true;
        appRule.InterfaceTypes = "All";
        appRule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL;

        firewallPolicy.Rules.Add(appRule);
    }

    public void openPortU(int port)
    {
        throw new NotImplementedException();
    }

    public void closePortW(INetFwRule rule)
    {
        
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

    public INetFwRule setupRuleW(int port, string path_to_exec, string server_guid)
    {
        return null;
    }
}