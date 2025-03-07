using UnityEngine;
using Cysharp.Threading.Tasks;
using System.IO;
using Zenject;
using System;

public class ScoreProvider : IInitializable
{
    private int Score;
    private int DestroyedUFO;
    private int DestroyedUFO_points = 1500;
    private int DestroyedAsteroids;
    private int DestroyedAsteroids_points = 500;
    private int FiredBullets;
    private int FiredBullets_points = 50;
    private int Reloads;
    private int Reloads_points = 250;
    private int FiredLasers;
    private int FiredLasers_points = 100;
    private float LaserTime;
    private int LaserTime_points = 100;
    private int MaxSpeed;
    private int MaxSpeed_points = 100;
    private int Travelled;
    private int Travelled_points = 10;
    private int SurvivedTime;
    private int SurvivedTime_points = 75;

    [Inject] private DifficultyProvider _difficultyLevel;

    public event Action<int> OnScoreChanged; 
    public async void Initialize()
    {
        ResetScores();
        await TrackSurvivedTime();
    }


    private bool isTrackingTime = false;

    public int GetScore()
    {
        return Score;
    }

    private void SetScore(int value)
    {
        if (Score != value)
        {
            Score = value;
            OnScoreChanged?.Invoke(Score);
        }
    }


    public void AddDestroyedUFO(int value)
    {
        DestroyedUFO += value;
    }

    public void AddDestroyedAsteroids(int value)
    {
        DestroyedAsteroids += value;
    }

    public void AddFiredBullets(int value)
    {
        FiredBullets += value;
    }

    public void AddReloads(int value)
    {
        Reloads += value;
    }

    public void AddFiredLasers(int value)
    {
        FiredLasers += value;
    }

    public void AddLaserTime(float amount)
    {
        LaserTime += amount;
    }

    public void SetMaxSpeed(int amount)
    {
        if (MaxSpeed < amount) MaxSpeed = amount;
    }

    public void SetTravelled(int amount)
    {
        Travelled = amount;
    }

    public void ResetScores()
    {
        Score = 0;
        DestroyedUFO = 0;
        DestroyedAsteroids = 0;
        FiredBullets = 0;
        Reloads = 0;
        FiredLasers = 0;
        LaserTime = 0;
        MaxSpeed = 0;
        Travelled = 0;
        SurvivedTime = 0;
    }

    public async UniTask TrackSurvivedTime()
    {
        if (isTrackingTime) return;

        isTrackingTime = true;
        float startTime = Time.time;

        while (isTrackingTime)
        {
            await UniTask.Yield();
            SurvivedTime = Mathf.FloorToInt(Time.time - startTime);
        }
    }

    public void StopTrackingTime()
    {
        isTrackingTime = false;
    }

    public void SaveScoresToFile(string filePath)
    {
        ScoreData data = new ScoreData
        {
            Score = this.Score,
            DestroyedUFO = this.DestroyedUFO,
            DestroyedAsteroids = this.DestroyedAsteroids,
            FiredBullets = this.FiredBullets,
            Reloads = this.Reloads,
            FiredLasers = this.FiredLasers,
            LaserTime = this.LaserTime,
            MaxSpeed = this.MaxSpeed,
            Travelled = this.Travelled,
            SurvivedTime = this.SurvivedTime
        };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
    }

    public void ScoreCounter()
    {
        StopTrackingTime();
        int baseScore = DestroyedUFO * DestroyedUFO_points + DestroyedAsteroids * DestroyedAsteroids_points + FiredBullets * FiredBullets_points +
                        Reloads * Reloads_points + FiredLasers * FiredLasers_points + Mathf.FloorToInt(LaserTime) * LaserTime_points +
                        MaxSpeed * MaxSpeed_points + Travelled * Travelled_points + SurvivedTime * SurvivedTime_points;

        int difficultyMultiplier = _difficultyLevel.CurrentDifficulty switch
        {
            DifficultyProvider.Difficulty.Easy => 1,
            DifficultyProvider.Difficulty.Medium => 2,
            DifficultyProvider.Difficulty.Hard => 3,
            _ => 1
        };

        SetScore(baseScore * difficultyMultiplier);
        SaveScoresToFile();
    }

    private void SaveScoresToFile()
    {
        string folderPath = Path.Combine(Application.streamingAssetsPath, "Record");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".json";
        string filePath = Path.Combine(folderPath, fileName);

        ScoreData data = new ScoreData
        {
            Score = Score,
            DestroyedUFO = DestroyedUFO,
            DestroyedAsteroids = DestroyedAsteroids,
            FiredBullets = FiredBullets,
            Reloads = Reloads,
            FiredLasers = FiredLasers,
            LaserTime = LaserTime,
            MaxSpeed = MaxSpeed,
            Travelled = Travelled,
            SurvivedTime = SurvivedTime
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    [System.Serializable]
    public class ScoreData
    {
        public int Score;
        public int DestroyedUFO;
        public int DestroyedAsteroids;
        public int FiredBullets;
        public int Reloads;
        public int FiredLasers;
        public float LaserTime;
        public int MaxSpeed;
        public int Travelled;
        public int SurvivedTime;
    }
}
