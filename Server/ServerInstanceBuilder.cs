namespace MinecraftServer.Server;

public interface IServerInstanceBuilder
{
    void setVersion(string version);
    void setRam(int amount);
}

public class ServerInstanceBuilder : IServerInstanceBuilder
{
    private ServerInstance instance = new ServerInstance();

    public ServerInstanceBuilder()
    {
        this.Reset();
    }   

    private void Reset()
    {
        instance = new ServerInstance();
    }

    public void setVersion(string version)
    {
        instance.Version = version;
    }

    public void setRam(int amount)
    {
        instance.Ram = amount;
    }

    public ServerInstance BuildInstance()
    {
        ServerInstance final = instance;
        
        Reset();

        return final;
    }
}