using System.IO;
using UnityEngine;

public static class AsteroidConfigLoader
{
    private static readonly string filePath = Path.Combine(Application.streamingAssetsPath, "asteroid_config.json");

    public static AsteroidConfig LoadConfig()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Config file not found: {filePath}");
            return null;
        }

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<AsteroidConfig>(json);
    }
}
