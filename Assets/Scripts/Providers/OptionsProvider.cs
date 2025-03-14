using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Cysharp.Threading.Tasks;
using TMPro;
using Zenject;

[System.Serializable]
public class Options
{
    public float musicVolume;
    public float soundVolume;
    public int difficulty;
}

public class OptionsProvider : MonoBehaviour
{
    [Inject] public DifficultyProvider difficultyProvider;

    [SerializeField] private Scrollbar musicSlider;
    [SerializeField] private Scrollbar soundSlider;
    [SerializeField] private TMP_Dropdown difficultyDropdown;

    public Options CurrentOptions { get; private set; } = new Options();
    public bool enableEscapeFunctionality = true;
    public GameObject OptionsMenu;
    public GameObject MobileUi;
    public bool IsMobile { get; private set; }

    private static string optionsFilePath => Path.Combine(Application.persistentDataPath, "options.json");

    private void Start()
    {
        IsMobile = Application.isMobilePlatform;
        if (MobileUi != null)
        {
            MobileUi.SetActive(IsMobile);
        }
        LoadOptions();
        ApplySettings();

        if (enableEscapeFunctionality)
        {
            StartEscapeFunctionality().Forget();
        }
    }

    public void SaveOptions()
    {
        CurrentOptions.difficulty = (int)difficultyProvider.CurrentDifficulty;
        string json = JsonUtility.ToJson(CurrentOptions, true);
        File.WriteAllText(optionsFilePath, json);
    }

    public void LoadOptions()
    {
        if (File.Exists(optionsFilePath))
        {
            string json = File.ReadAllText(optionsFilePath);
            CurrentOptions = JsonUtility.FromJson<Options>(json);
            if (System.Enum.IsDefined(typeof(DifficultyProvider.Difficulty), CurrentOptions.difficulty))
            {
                difficultyProvider.SetDifficulty((DifficultyProvider.Difficulty)CurrentOptions.difficulty);
            }
            else
            {
                CurrentOptions.difficulty = (int)DifficultyProvider.Difficulty.Easy;
            }
        }
    }

    public void SetMusicVolume(float volume)
    {
        CurrentOptions.musicVolume = Mathf.Clamp01(volume);
        SaveOptions();
    }

    public void SetSoundVolume(float volume)
    {
        CurrentOptions.soundVolume = Mathf.Clamp01(volume);
        SaveOptions();
    }

    public void SetDifficulty(int difficultyLevel)
    {
        if (difficultyProvider != null && System.Enum.IsDefined(typeof(DifficultyProvider.Difficulty), difficultyLevel))
        {
            difficultyProvider.SetDifficulty((DifficultyProvider.Difficulty)difficultyLevel);
            SaveOptions();
        }
    }

    public void TogglePauseGame()
    {
        Time.timeScale = (Time.timeScale == 1f) ? 0f : 1f;
    }

    private void ApplySettings()
    {
        if (musicSlider != null)
        {
            musicSlider.value = CurrentOptions.musicVolume;
        }

        if (soundSlider != null)
        {
            soundSlider.value = CurrentOptions.soundVolume;
        }

        if (difficultyDropdown != null && difficultyProvider != null)
        {
            string[] difficulties = System.Enum.GetNames(typeof(DifficultyProvider.Difficulty));
            difficultyDropdown.ClearOptions();
            difficultyDropdown.AddOptions(new System.Collections.Generic.List<string>(difficulties));

            difficultyDropdown.value = (int)difficultyProvider.CurrentDifficulty;
        }
    }

    private async UniTaskVoid StartEscapeFunctionality()
    {
        var cancellationToken = this.GetCancellationTokenOnDestroy();
        while (!cancellationToken.IsCancellationRequested)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OptionsMenu.SetActive(!OptionsMenu.activeSelf);
                TogglePauseGame();
            }
            await UniTask.Yield(cancellationToken);
        }
    }
}
