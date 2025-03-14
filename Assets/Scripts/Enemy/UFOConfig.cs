using System;
using System.IO;
using UnityEngine;

[Serializable]
public class UFOConfig
{
    public float moveSpeed;
    public float rotationSpeed;
    public int healthEasy;
    public int healthMedium;
    public int healthHard;
}

public class UFOConfigLoader
{
    private static readonly string directoryPath = Application.streamingAssetsPath;
    private static readonly string filePath = Path.Combine(Application.streamingAssetsPath, "ufo_config.json");

    public UFOConfig LoadConfig()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Config file not found. Creating a new one.");
            return CreateDefaultConfig();
        }

        try
        {
            string json = File.ReadAllText(filePath);
            UFOConfig config = JsonUtility.FromJson<UFOConfig>(json);

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

    public static void SaveConfig(UFOConfig config)
    {
        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(filePath, json);
    }


    private static UFOConfig CreateDefaultConfig()
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        UFOConfig defaultConfig = new UFOConfig
        {
            moveSpeed = 5.5f,
            rotationSpeed = 50.0f,
            healthEasy = 2,
            healthMedium = 3,
            healthHard = 5
        };

        SaveConfig(defaultConfig);
        return defaultConfig;
    }

    private static bool IsConfigComplete(UFOConfig config)
    {
        return config.moveSpeed != 0 &&
               config.rotationSpeed != 0 &&
               config.healthEasy != 0 &&
               config.healthMedium != 0 &&
               config.healthHard != 0;
    }
}
