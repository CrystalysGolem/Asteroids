using UnityEngine;
using TMPro;
using Zenject;

public class ScoreView : MonoBehaviour
{
    [Inject] private ScoreProvider _scoreManager;
    [SerializeField] private GameObject scoreObject;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Inject]
    public void Construct(ScoreProvider scoreManager)
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
