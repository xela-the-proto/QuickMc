using MinecraftServer.Json;
using Newtonsoft.Json;

namespace MinecraftServer.Utils;

public class ManifestSingleton
{
    private ManifestSingleton(){}
    
    private static ManifestStruct _instance;
    
    private static readonly object _lock = new object();

    /// <summary>
    /// Gets or deserializes the main manifest file
    /// </summary>
    /// <returns>Returns the deserialized manifest as a List</returns>
    /// <exception cref="FileNotFoundException">Thrown when _instance is null</exception>
    public static ManifestStruct GetInstance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = JsonConvert.DeserializeObject<ManifestStruct>(File.ReadAllText
                    (Path.Combine(Logging.path_root , "/QuickMc/manifests/version_manifest_v2.json"))) ?? throw new FileNotFoundException();
                }
            }
        }
        return _instance;
    }
}