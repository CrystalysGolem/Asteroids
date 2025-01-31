using System.ComponentModel;
using UnityEngine;
using Zenject;

public class DifficultyManager : IInitializable
{
    private Difficulty currentDifficulty = Difficulty.Easy;

    public void Initialize()
    {
    }


    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    public Difficulty CurrentDifficulty
    {
        get { return currentDifficulty; }
        set { currentDifficulty = value; }
    }

    public void SetDifficultyToEasy()
    {
        SetDifficulty(Difficulty.Easy);
    }

    public void SetDifficultyToMedium()
    {
        SetDifficulty(Difficulty.Medium);
    }

    public void SetDifficultyToHard()
    {
        SetDifficulty(Difficulty.Hard);
    }

    private void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        Debug.Log("Current Difficulty: " + currentDifficulty);
    }
}
