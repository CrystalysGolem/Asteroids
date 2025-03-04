using System;
using System.IO;
using UnityEngine;

[Serializable]
public class AsteroidConfig
{
    public float moveSpeed;
    public float rotationSpeed;
    public int healthEasy;
    public int healthMedium;
    public int healthHard;
    public int minFragments;
    public int maxFragments;
}

public static class AsteroidConfigLoader
{
    private static readonly string directoryPath = Application.streamingAssetsPath;
    private static readonly string filePath = Path.Combine(directoryPath, "asteroid_config.json");

    public static AsteroidConfig LoadConfig()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Config file not found. Creating a new one.");
            return CreateDefaultConfig();
        }

        try
        {
            string json = File.ReadAllText(filePath);
            AsteroidConfig config = JsonUtility.FromJson<AsteroidConfig>(json);

            if (config == null || !IsConfigComplete(config))
            {
                Debug.LogWarning("Config file is incomplete or corrupted. Creating a new one.");
                return CreateDefaultConfig();
            }

            return config;
        }
        catch
        {
            Debug.LogError("Failed to read config file. Creating a new one.");
            return CreateDefaultConfig();
        }
    }

    private static AsteroidConfig CreateDefaultConfig()
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        AsteroidConfig defaultConfig = new AsteroidConfig
        {
            moveSpeed = 10.0f,
            rotationSpeed = 50.0f,
            healthEasy = 3,
            healthMedium = 5,
            healthHard = 8,
            minFragments = 2,
            maxFragments = 5
        };

        SaveConfig(defaultConfig);
        return defaultConfig;
    }

    private static bool IsConfigComplete(AsteroidConfig config)
    {
        return config.moveSpeed != 0 &&
               config.rotationSpeed != 0 &&
               config.healthEasy != 0 &&
               config.healthMedium != 0 &&
               config.healthHard != 0 &&
               config.minFragments != 0 &&
               config.maxFragments != 0;
    }

    public static void SaveConfig(AsteroidConfig config)
    {
        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(filePath, json);
    }
}
