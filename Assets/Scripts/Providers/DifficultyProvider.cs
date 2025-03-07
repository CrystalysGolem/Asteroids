using UnityEngine;
using Zenject;

public class DifficultyProvider : IInitializable
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
        private set { currentDifficulty = value; }
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        Debug.Log("Current Difficulty: " + currentDifficulty);
    }

    public void SetDifficultyToEasy() => SetDifficulty(Difficulty.Easy);
    public void SetDifficultyToMedium() => SetDifficulty(Difficulty.Medium);
    public void SetDifficultyToHard() => SetDifficulty(Difficulty.Hard);
}
