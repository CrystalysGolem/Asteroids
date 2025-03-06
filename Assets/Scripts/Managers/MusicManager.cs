using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using System.IO;

[System.Serializable]
public class MusicSaveData
{
    public int trackIndex = 0;
    public string trackName = "";
    public float trackTime = 0f;
}

public class MusicManager : MonoBehaviour
{

    [Header("Music player and music itself")]
    public AudioSource musicSource;
    public AudioClip[] musicTracks;

    //Minor logic
    private int currentTrackIndex = 0;
    private float trackTime = 0f;
    [Inject] private OptionsManager optionsManager;
    private static string saveFilePath => Path.Combine(Application.persistentDataPath, "music_save.json");

    private void Start()
    {
        LoadMusicState();

        if (musicTracks.Length > 0)
        {
            PlayTrack(currentTrackIndex, trackTime);
        }

        UpdateMusicVolumeLoop().Forget();
        SaveMusicStateLoop().Forget();
    }

    public void PlayTrack(int index, float startTime = 0f)
    {
        if (index >= 0 && index < musicTracks.Length)
        {
            musicSource.clip = musicTracks[index];
            musicSource.time = startTime;
            musicSource.Play();
            currentTrackIndex = index;
        }
    }

    public void PlayNextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Length;
        PlayTrack(currentTrackIndex);
    }

    public void PlayPreviousTrack()
    {
        currentTrackIndex = (currentTrackIndex - 1 + musicTracks.Length) % musicTracks.Length;
        PlayTrack(currentTrackIndex);
    }

    public void TogglePause()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
        }
        else
        {
            musicSource.Play();
        }
    }

    private async UniTaskVoid UpdateMusicVolumeLoop()
    {
        while (true && this != null)
        {
            musicSource.volume = 0.2f * optionsManager.CurrentOptions.musicVolume;
            await UniTask.Yield();
        }
    }

    private async UniTaskVoid SaveMusicStateLoop()
    {
        while (true && this != null)
        {
            SaveMusicState();
            await UniTask.Delay(1000);
        }
    }

    private void SaveMusicState()
    {
        MusicSaveData saveData = new MusicSaveData
        {
            trackIndex = currentTrackIndex,
            trackName = musicSource.clip != null ? musicSource.clip.name : "",
            trackTime = musicSource.time
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);
    }

    private void LoadMusicState()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            MusicSaveData saveData = JsonUtility.FromJson<MusicSaveData>(json);

            if (saveData.trackIndex >= 0 && saveData.trackIndex < musicTracks.Length &&
                musicTracks[saveData.trackIndex].name == saveData.trackName)
            {
                currentTrackIndex = saveData.trackIndex;
                trackTime = saveData.trackTime;
            }
            else
            {
                currentTrackIndex = 0;
                trackTime = 0f;
            }
        }
    }
}
