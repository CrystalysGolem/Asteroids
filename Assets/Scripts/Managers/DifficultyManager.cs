using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class DifficultyManager : IInitializable
{
    // Deafult Difficulty
    private Difficulty currentDifficulty = Difficulty.Easy;

    // Minor
    [Inject] private OptionsManager optionsManager;

    public void Initialize()
    {
        LoadDifficultyFromOptions();
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

    public void SetDifficultyToEasy() => SetDifficulty(Difficulty.Easy);
    public void SetDifficultyToMedium() => SetDifficulty(Difficulty.Medium);
    public void SetDifficultyToHard() => SetDifficulty(Difficulty.Hard);

    private void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        Debug.Log("Current Difficulty: " + currentDifficulty);
    }

    private async void LoadDifficultyFromOptions()
    {
        switch (optionsManager.CurrentOptions.difficulty)
        {
            case "Easy":
                SetDifficulty(Difficulty.Easy);
                break;
            case "Medium":
                SetDifficulty(Difficulty.Medium);
                break;
            case "Hard":
                SetDifficulty(Difficulty.Hard);
                break;
            default:
                await UniTask.Delay(100);
                LoadDifficultyFromOptions();
                break;
        }
    }
}
