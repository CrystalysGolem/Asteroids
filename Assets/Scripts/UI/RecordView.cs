using UnityEngine;
using TMPro;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class RecordView : MonoBehaviour
{
    public GameObject[] dateTextObjects = new GameObject[3];
    public GameObject[] scoreTextObjects = new GameObject[3];

    private string recordsPath;

    private void Awake()
    {
        recordsPath = Path.Combine(Application.streamingAssetsPath, "Record");
    }

    public void LoadAndSetScores()
    {
        if (!Directory.Exists(recordsPath))
        {
            Debug.LogWarning("Record directory does not exist.");
            return;
        }

        var files = Directory.GetFiles(recordsPath, "*.json")
                             .Select(f => new FileInfo(f))
                             .OrderByDescending(f => f.CreationTime)
                             .Take(3)
                             .ToArray();

        List<(DateTime, int)> scores = new List<(DateTime, int)>();

        foreach (var file in files)
        {
            string json = File.ReadAllText(file.FullName);
            ScoreData data = JsonUtility.FromJson<ScoreData>(json);
            scores.Add((file.CreationTime, data.Score));
        }

        SetScores(scores.ToArray());
    }

    public void SetScores((DateTime date, int score)[] scores)
    {
        Array.Sort(scores, (a, b) => b.score.CompareTo(a.score)); 

        for (int i = 0; i < scores.Length && i < 3; i++)
        {
            if (dateTextObjects[i] != null && dateTextObjects[i].TryGetComponent(out TMP_Text dateText))
            {
                dateText.text = $"Date: {scores[i].date:dd-MM-yyyy}";
            }

            if (scoreTextObjects[i] != null && scoreTextObjects[i].TryGetComponent(out TMP_Text scoreText))
            {
                scoreText.text = $"Score: {scores[i].score}";
            }
        }
    }
}

[Serializable]
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