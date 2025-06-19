using System.IO;
using Newtonsoft.Json;
using QuickMC.Json.JsonClasses;

namespace QuickMC.Utils;

//TODO: why is this a singleton?
public class ManifestSingleton
{
    private ManifestSingleton(){}
    
    private static MainVersionManifest _instance;
    
    private static readonly object _lock = new object();

    /// <summary>
    /// Gets or deserializes the main manifest file
    /// </summary>
    /// <returns>Returns the deserialized manifest as a List</returns>
    /// <exception cref="FileNotFoundException">Thrown when _instance is null</exception>
    public static MainVersionManifest GetInstance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = JsonConvert.DeserializeObject<MainVersionManifest>(Program.manifest) ?? throw new FileNotFoundException();
                }
            }
        }
        return _instance;
    }
}