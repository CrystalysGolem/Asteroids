using UnityEngine;
using Zenject;

public class Validator : MonoBehaviour
{
    [Inject] private PlayerMove _playerMovement;
    [Inject] private PlayerShoot _playerShooting;
    [Inject] private PlayerTeleport _playerTeleport;
    [Inject] private ScoreManager _scoreManager;
    [Inject] private DifficultyManager _difficultyManager;
    [Inject] private DifficultyManager _enemyFabric;

    private void Start()
    {
        ValidateDependencies();
    }

    private void ValidateDependencies()
    {
        if (_playerMovement == null)
            ThrowMissingDependency(nameof(PlayerMove));

        if (_playerShooting == null)
            ThrowMissingDependency(nameof(PlayerShoot));

        if (_playerTeleport == null)
            ThrowMissingDependency(nameof(PlayerTeleport));

        if (_scoreManager == null)
            ThrowMissingDependency(nameof(ScoreManager));

        if (_difficultyManager == null)
            ThrowMissingDependency(nameof(DifficultyManager));

        Debug.Log("All dependencies have been successfully validated!");
    }

    private void ThrowMissingDependency(string dependencyName)
    {
        throw new MissingReferenceException($"Missing dependency: {dependencyName}. Ensure it is properly bound in Zenject.");
    }
}
