using Serilog;

namespace MinecraftServer.Exceptions;

public class NoJavaException : Exception
{
    public NoJavaException()
    { 
        Log.Fatal("No valid java version detected!");
    }
    public NoJavaException(string message) : base(message)
    { 
        Log.Fatal(message);
    }
}