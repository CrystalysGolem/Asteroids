using UnityEngine;
using TMPro;
using Zenject;

public class ScoreView : MonoBehaviour
{
    [Header("For Score view in EndGame")]
    [SerializeField] private GameObject scoreObject;
    [SerializeField] private TextMeshProUGUI scoreText;
    [Inject] private ScoreManager _scoreManager;

    [Inject]
    public void Construct(ScoreManager scoreManager)
    {
        _scoreManager = scoreManager;
        _scoreManager.OnScoreChanged += ScoreDraw; 
    }

    private void OnDestroy()
    {
        if (_scoreManager != null)
        {
            _scoreManager.OnScoreChanged -= ScoreDraw; 
        }
    }

    private void ScoreDraw(int newScore)
    {
        scoreObject.SetActive(true);
        scoreText.text = $"Score: {newScore}";
    }
}
