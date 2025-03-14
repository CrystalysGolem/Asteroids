using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class Asteroid : MonoBehaviour, IInvincible, IEnemy, IHealth
{
    [Inject] private AsteroidConfigLoader configLoader;
    [Inject] private PlayerMove playerMovement;
    [Inject] private PrefabFactory prefabFactory;
    [Inject] private ScoreProvider scoreManager;
    [Inject] private DifficultyProvider difficultySettings;

    [SerializeField] private int TimeBeforeInvisibility = 75;

    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public GameObject GameObject => gameObject;
    public int CurrentHealth { get; set; }
    public bool IsInvincible { get; set; }

    private float moveSpeed;
    private float rotationSpeed;
    private Vector3 targetPosition;
    private Vector3 currentDirection;
    private GameObject _fragmentPrefab;
    private int minFragments;
    private int maxFragments;
    private AsteroidConfig config;
    private SpriteRenderer spriteRenderer;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.GetComponent<PlayerPart>() || collision.GetComponent<Projectile>()) && !IsInvincible)
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

    public void StartUP()
    {
        config = configLoader.LoadConfig();
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
            DifficultyProvider.Difficulty.Easy => config.healthEasy,
            DifficultyProvider.Difficulty.Medium => config.healthMedium,
            DifficultyProvider.Difficulty.Hard => config.healthHard,
            _ => config.healthEasy
        };

        CurrentHealth = health;

        this.SetInitialPosition(playerMovement.movementLogic.Position, out targetPosition, out currentDirection);
        StartMove(moveSpeed, rotationSpeed);
    }

    public void StartMove(float moveSpeed, float rotationSpeed)
    {
        this.moveSpeed = moveSpeed;
        this.rotationSpeed = rotationSpeed;
        MoveToTarget().Forget();
    }

    public void SetFragmentPrefab(GameObject fragmentPrefab)
    {
        _fragmentPrefab = fragmentPrefab;
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

    private async UniTaskVoid HandleHitVisualsWithDelay()
    {
        await UniTask.Delay(TimeBeforeInvisibility);
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
