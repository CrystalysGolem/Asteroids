using System;
using System.IO;
using UnityEngine;

[Serializable]
public class AsteroidFragmentConfig
{
    public float minSpeed;
    public float maxSpeed;
    public float minSpeedRotation;
    public float maxSpeedRotation;
    public float rotationSpeed;
    public int healthEasy;
    public int healthMedium;
    public int healthHard;
}

public class AsteroidFragmentConfigLoader
{
    private static readonly string directoryPath = Application.streamingAssetsPath;
    private static readonly string filePath = Path.Combine(directoryPath, "asteroid_fragment_config.json");

    public AsteroidFragmentConfig LoadConfig()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Config file not found. Creating a new one.");
            return CreateDefaultConfig();
        }

        try
        {
            string json = File.ReadAllText(filePath);
            AsteroidFragmentConfig config = JsonUtility.FromJson<AsteroidFragmentConfig>(json);

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

    public static void SaveConfig(AsteroidFragmentConfig config)
    {
        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(filePath, json);
    }

    private static AsteroidFragmentConfig CreateDefaultConfig()
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        AsteroidFragmentConfig defaultConfig = new AsteroidFragmentConfig
        {
            minSpeed = 2.0f,
            maxSpeed = 12.0f,
            minSpeedRotation = 60.0f,
            maxSpeedRotation = 120.0f,
            rotationSpeed = 100.0f,
            healthEasy = 1,
            healthMedium = 1,
            healthHard = 2
        };

        SaveConfig(defaultConfig);
        return defaultConfig;
    }

    private static bool IsConfigComplete(AsteroidFragmentConfig config)
    {
        return config.minSpeed != 0 &&
               config.maxSpeed != 0 &&
               config.minSpeedRotation != 0 &&
               config.maxSpeedRotation != 0 &&
               config.rotationSpeed != 0 &&
               config.healthEasy != 0 &&
               config.healthMedium != 0 &&
               config.healthHard != 0;
    }
}
