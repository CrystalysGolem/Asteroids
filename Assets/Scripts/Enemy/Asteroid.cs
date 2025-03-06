using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class Asteroid : MonoBehaviour, IInvincible, IEnemy, IHealth
{
    public class Factory : PlaceholderFactory<Asteroid> { }

    // Health logic
    [SerializeField] public int CurrentHealth { get; set; }

    // Visual invisibility
    public bool IsInvincible { get; set; }

    // Move logic
    private float moveSpeed;
    private float rotationSpeed;
    private Vector3 targetPosition;
    private Vector3 currentDirection;

    // Spawned prefab after destruction
    private GameObject _fragmentPrefab;
    private int minFragments;
    private int maxFragments;

    // Config from JSON
    private AsteroidConfig config;



    // Minor logic

    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public GameObject GameObject => gameObject;
    private SpriteRenderer spriteRenderer;

    [Inject] private PlayerMove playerMovement;
    [Inject] private PrefabFactory prefabFactory;
    [Inject] private ScoreManager scoreManager;
    [Inject(Optional = true)] private DifficultyManager difficultySettings;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetFragmentPrefab(GameObject fragmentPrefab)
    {
        _fragmentPrefab = fragmentPrefab;
    }

    public void StartUP()
    {
        config = AsteroidConfigLoader.LoadConfig();
        if (config == null)
        {
            Debug.LogError("AsteroidConfig not loaded!");
            return;
        }

        moveSpeed = config.moveSpeed;
        rotationSpeed = config.rotationSpeed;
        minFragments = config.minFragments;
        maxFragments = config.maxFragments;

        int health = difficultySettings.CurrentDifficulty switch
        {
            DifficultyManager.Difficulty.Easy => config.healthEasy,
            DifficultyManager.Difficulty.Medium => config.healthMedium,
            DifficultyManager.Difficulty.Hard => config.healthHard,
            _ => config.healthEasy
        };

        CurrentHealth = health;

        this.SetInitialPosition(playerMovement.movementLogic, out targetPosition, out currentDirection);
        StartMove(moveSpeed, rotationSpeed);
    }

    public void StartMove(float moveSpeed, float rotationSpeed)
    {
        this.moveSpeed = moveSpeed;
        this.rotationSpeed = rotationSpeed;
        MoveToTarget().Forget();
    }

    private async UniTaskVoid MoveToTarget()
    {
        while (this != null && gameObject.activeSelf)
        {
            transform.position += currentDirection * moveSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            await UniTask.Yield();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("Player") || collision.CompareTag("Projectile")) && !IsInvincible)
        {
            this.TakeDamage();
            if (!gameObject.activeSelf)
            {
                SpawnFragments();
                scoreManager.AddDestroyedAsteroids(1);
            }
            HandleHitVisualsWithDelay().Forget();
        }
    }

    private async UniTaskVoid HandleHitVisualsWithDelay()
    {
        await UniTask.Delay(75);
        this.HandleHitVisuals().Forget();
    }

    private void SpawnFragments()
    {
        int fragmentCount = Random.Range(minFragments, maxFragments + 1);

        for (int i = 0; i < fragmentCount; i++)
        {
            prefabFactory.SpawnPrefabInstantly(_fragmentPrefab, transform.position);
        }
    }
}
