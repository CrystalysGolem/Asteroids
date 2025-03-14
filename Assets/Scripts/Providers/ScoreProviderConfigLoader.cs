using System;
using System.IO;
using UnityEngine;

[Serializable]
public class ScoreProviderConfig
{
    public int DestroyedUFO_points = 1500;
    public int DestroyedAsteroids_points = 500;
    public int FiredBullets_points = 50;
    public int Reloads_points = 250;
    public int FiredLasers_points = 100;
    public int LaserTime_points = 100;
    public int MaxSpeed_points = 100;
    public int Travelled_points = 10;
    public int SurvivedTime_points = 75;
}

public class ScoreProviderConfigLoader
{
    private static readonly string directoryPath = Application.streamingAssetsPath;
    private static readonly string filePath = Path.Combine(directoryPath, "score_provider_config.json");

    public ScoreProviderConfig LoadConfig()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("ScoreProvider config file not found. Creating a new one.");
            return CreateDefaultConfig();
        }

        try
        {
            string json = File.ReadAllText(filePath);
            ScoreProviderConfig config = JsonUtility.FromJson<ScoreProviderConfig>(json);
            if (config == null || !IsConfigComplete(config))
            {
                Debug.LogWarning("ScoreProvider config file is incomplete or corrupted. Creating a new one.");
                return CreateDefaultConfig();
            }
            return config;
        }
        catch
        {
            Debug.LogError("Failed to read ScoreProvider config file. Creating a new one.");
            return CreateDefaultConfig();
        }
    }

    public static void SaveConfig(ScoreProviderConfig config)
    {
        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(filePath, json);
    }

    private static ScoreProviderConfig CreateDefaultConfig()
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        ScoreProviderConfig defaultConfig = new ScoreProviderConfig();
        SaveConfig(defaultConfig);
        return defaultConfig;
    }

    private static bool IsConfigComplete(ScoreProviderConfig config)
    {
        return config.DestroyedUFO_points > 0 &&
               config.DestroyedAsteroids_points > 0 &&
               config.FiredBullets_points > 0 &&
               config.Reloads_points > 0 &&
               config.FiredLasers_points > 0 &&
               config.LaserTime_points > 0 &&
               config.MaxSpeed_points > 0 &&
               config.Travelled_points > 0 &&
               config.SurvivedTime_points > 0;
    }
}
