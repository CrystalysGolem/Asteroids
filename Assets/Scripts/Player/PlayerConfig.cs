using System;
using System.IO;
using UnityEngine;

[Serializable]
public class PlayerConfig
{
    public int currentHealth;
    public float maxSpeed;
    public float acceleration;
    public float deceleration;
    public float shootingForce;
    public float shootCooldown;
    public int maxAmmo;
    public float reloadTime;
    public float objectActiveDuration;
    public float objectCooldown;
}

public static class PlayerConfigLoader
{
    private static readonly string directoryPath = Application.streamingAssetsPath;
    private static readonly string filePath = Path.Combine(Application.streamingAssetsPath, "player_config.json");

    public static PlayerConfig LoadConfig()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Config file not found. Creating a new one.");
            return CreateDefaultConfig();
        }

        try
        {
            string json = File.ReadAllText(filePath);
            PlayerConfig config = JsonUtility.FromJson<PlayerConfig>(json);

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

    private static PlayerConfig CreateDefaultConfig()
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        PlayerConfig defaultConfig = new PlayerConfig
        {
            currentHealth = 3,
            maxSpeed = 25f,
            acceleration = 20f,
            deceleration = 15f,
            shootingForce = 20f,
            shootCooldown = 0.25f,
            maxAmmo = 20,
            reloadTime = 2f,
            objectActiveDuration = 3f,
            objectCooldown = 15f
        };

        SaveConfig(defaultConfig);
        return defaultConfig;
    }

    private static bool IsConfigComplete(PlayerConfig config)
    {
        return config.currentHealth > 0 &&
               config.maxSpeed > 0 &&
               config.acceleration > 0 &&
               config.deceleration > 0 &&
               config.shootingForce > 0 &&
               config.shootCooldown > 0 &&
               config.maxAmmo > 0 &&
               config.reloadTime > 0 &&
               config.objectActiveDuration > 0 &&
               config.objectCooldown > 0;
    }

    public static void SaveConfig(PlayerConfig config)
    {
        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(filePath, json);
    }
}
