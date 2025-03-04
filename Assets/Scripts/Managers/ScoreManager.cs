using UnityEngine;
using Cysharp.Threading.Tasks;
using System.IO;
using Zenject;
using System;

public class ScoreManager : IInitializable
{
    [SerializeField] private int Score;
    [SerializeField] private int DestroyedUFO;
    [SerializeField] private int DestroyedAsteroids;
    [SerializeField] private int FiredBullets;
    [SerializeField] private int Reloads;
    [SerializeField] private int FiredLasers;
    [SerializeField] private float LaserTime;
    [SerializeField] private int MaxSpeed;
    [SerializeField] private int Travelled;
    [SerializeField] private int SurvivedTime;

    [Inject] private DifficultyManager _difficultyLevel;

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
        Debug.Log("Scores saved to file.");
    }

    public void LoadScoresFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            ScoreData data = JsonUtility.FromJson<ScoreData>(json);
            this.Score = data.Score;
            this.DestroyedUFO = data.DestroyedUFO;
            this.DestroyedAsteroids = data.DestroyedAsteroids;
            this.FiredBullets = data.FiredBullets;
            this.Reloads = data.Reloads;
            this.FiredLasers = data.FiredLasers;
            this.LaserTime = data.LaserTime;
            this.MaxSpeed = data.MaxSpeed;
            this.Travelled = data.Travelled;
            this.SurvivedTime = data.SurvivedTime;

            Debug.Log("Scores loaded from file.");
        }
        else
        {
            Debug.LogWarning("No score file found.");
        }
    }

    public void ScoreCounter()
    {
        StopTrackingTime();
        int baseScore = 0;

        // Подсчет очков
        baseScore += DestroyedUFO * 1500;
        baseScore += DestroyedAsteroids * 500;
        baseScore += FiredBullets * 50;
        baseScore += Reloads * 250;
        baseScore += FiredLasers * 100;
        baseScore += Mathf.FloorToInt(LaserTime) * 100;
        baseScore += MaxSpeed * 100;
        baseScore += Travelled * 10;
        baseScore += SurvivedTime * 75;

        int difficultyMultiplier = _difficultyLevel.CurrentDifficulty switch
        {
            DifficultyManager.Difficulty.Easy => 1,
            DifficultyManager.Difficulty.Medium => 2,
            DifficultyManager.Difficulty.Hard => 3,
            _ => 1
        };

        SetScore(baseScore * difficultyMultiplier);
        Debug.Log($"Score calculated: {Score}");
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
