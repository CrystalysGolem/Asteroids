using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class SceneLoader : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
{
    [SerializeField] private string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] private string _iOsAdUnitId = "Interstitial_iOS";
    [SerializeField] private string _gameIdAndroid = "5812853";
    [SerializeField] private bool _testMode = true;

    public string _adUnitId;
    public string _sceneToLoad;
    public int _sceneIndexToLoad = -1;

    void Awake()
    {
        _adUnitId = Application.platform == RuntimePlatform.IPhonePlayer ? _iOsAdUnitId : _androidAdUnitId;
        InitializeAds();
        Advertisement.Load(_adUnitId, this);
    }

    public void InitializeAds()
    {
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameIdAndroid, _testMode, this);
        }
    }

    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name not right!");
            return;
        }
        _sceneToLoad = sceneName;
        _sceneIndexToLoad = -1;
        ShowAdOrLoadScene();
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("Wrong scene index!");
            return;
        }
        _sceneIndexToLoad = sceneIndex;
        _sceneToLoad = null;
        ShowAdOrLoadScene();
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad loaded: " + adUnitId);
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Ad load error {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Ad show error {adUnitId}: {error.ToString()} - {message}");
        LoadScene();
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        Debug.Log("Ad started: " + adUnitId);
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
        Debug.Log("Ad click: " + adUnitId);
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("Ad complete: " + adUnitId + ", state: " + showCompletionState);
        LoadScene();
    }


    private void ShowAdOrLoadScene()
    {
        if (Advertisement.isInitialized)
        {
            Debug.Log("Show ad.");
            Advertisement.Show(_adUnitId, this);
        }
        else
        {
            Debug.Log("Ad not initialized.");
            LoadScene();
        }
    }

    private void LoadScene()
    {
        if (!string.IsNullOrEmpty(_sceneToLoad))
        {
            SceneManager.LoadScene(_sceneToLoad, LoadSceneMode.Single);
        }
        else if (_sceneIndexToLoad != -1)
        {
            SceneManager.LoadScene(_sceneIndexToLoad, LoadSceneMode.Single);
        }
    }
}
