using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Cysharp.Threading.Tasks;
using TMPro;

[System.Serializable]
public class Options
{
    public float musicVolume;
    public float soundVolume;
    public string difficulty;
}

public class OptionsManager : MonoBehaviour
{
    private static string optionsFilePath => Path.Combine(Application.persistentDataPath, "options.json");
    public Options CurrentOptions { get; private set; } = new Options();
    private bool isEscapePressed = false;
    [Header("For Esc options functional")]
    [SerializeField] public bool enableEscapeFunctionality = true;
    [Header("For enabling\\disabling options menu")]
    [SerializeField] public GameObject OptionsMenu;
    [SerializeField] public GameObject MobileUi;
    [Header("For filling options menu")]
    [SerializeField] private Scrollbar musicSlider;
    [SerializeField] private Scrollbar soundSlider;
    [SerializeField] private TMP_Dropdown difficultyDropdown;

    [SerializeField] public bool IsMobile { get; private set; }

    private void Start()
    {
        IsMobile = Application.isMobilePlatform;
        if (IsMobile && MobileUi!=null)
        {
            MobileUi.SetActive(true);
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
        string json = JsonUtility.ToJson(CurrentOptions, true);
        File.WriteAllText(optionsFilePath, json);
    }

    public void LoadOptions()
    {
        if (File.Exists(optionsFilePath))
        {
            string json = File.ReadAllText(optionsFilePath);
            CurrentOptions = JsonUtility.FromJson<Options>(json);
        }
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

        if (difficultyDropdown != null)
        {
            string[] difficulties = { "Easy", "Medium", "Hard" };
            int index = System.Array.IndexOf(difficulties, CurrentOptions.difficulty);
            if (index != -1)
            {
                difficultyDropdown.value = index;
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

    public void SetIsPressed()
    {
        isEscapePressed = false;
    }

    public void SetDifficulty(int difficultyLevel)
    {
        string[] difficulties = { "Easy", "Medium", "Hard" };
        if (difficultyLevel >= 0 && difficultyLevel < difficulties.Length)
        {
            CurrentOptions.difficulty = difficulties[difficultyLevel];
            SaveOptions();
        }
    }

    public void TogglePauseGame()
    {
        if (Time.timeScale == 1f)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private async UniTaskVoid StartEscapeFunctionality()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OptionsMenu.SetActive(!OptionsMenu.activeSelf);
                isEscapePressed = OptionsMenu.activeSelf;
                TogglePauseGame();
            }
            await UniTask.Yield();
        }
    }
}
