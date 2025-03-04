using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class Asteroid : MonoBehaviour, IInvincible, IEnemy, IHealth
{
    public class Factory : PlaceholderFactory<Asteroid> { }

    [SerializeField] public int CurrentHealth { get; set; }
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Vector3 currentDirection;
    [SerializeField] public bool IsInvincible { get; set; }

    private float moveSpeed;
    private float rotationSpeed;
    private int minFragments;
    private int maxFragments;

    [Inject] private PlayerTeleport playerTeleport;
    [Inject] private PlayerMove playerMovement;
    [Inject] private PrefabFactory prefabFactory;
    [Inject] private ScoreManager scoreManager;
    [Inject(Optional = true)] private DifficultyManager difficultySettings;

    private GameObject _fragmentPrefab;
    private AsteroidConfig config;

    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public GameObject GameObject => gameObject;

    private SpriteRenderer spriteRenderer;

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

        this.SetInitialPosition(playerTeleport, playerMovement, out targetPosition, out currentDirection);
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
        if (collision.CompareTag("Player") || collision.CompareTag("Projectile") && !IsInvincible)
        {
            this.TakeDamage();
            if (gameObject.activeSelf == false)
            {
                SpawnFragments();
                scoreManager.AddDestroyedAsteroids(1);
            }
            this.HandleHitVisuals().Forget();
        }
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
