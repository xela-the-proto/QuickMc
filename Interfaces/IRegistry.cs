namespace MinecraftServer.Interfaces;

public interface IRegistry
{
    public void CheckJava();

    public void CheckJavaForVersion(string version);
}