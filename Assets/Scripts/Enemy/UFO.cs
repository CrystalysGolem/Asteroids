using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class UFO : MonoBehaviour, IInvincible, IEnemy, IHealth
{
    public int CurrentHealth { get; set; }

    private float moveSpeed;
    private float rotationSpeed;
    private Vector3 targetPosition;

    private UFOConfig config;

    public bool IsInvincible { get; set; }

    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public GameObject GameObject => gameObject;
    private SpriteRenderer spriteRenderer;

    [Inject] private PlayerMove playerMovement;
    [Inject] private DifficultyProvider difficultySettings;
    [Inject] private ScoreProvider scoreManager;
    public class Factory : PlaceholderFactory<UFO> { }


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartUP();
    }

    public void StartUP()
    {
        config = UFOConfigLoader.LoadConfig();
        if (config == null)
        {
            Debug.LogError("UFOConfig not loaded!");
            return;
        }

        moveSpeed = config.moveSpeed;
        rotationSpeed = config.rotationSpeed;

        int health = difficultySettings.CurrentDifficulty switch
        {
            DifficultyProvider.Difficulty.Easy => config.healthEasy,
            DifficultyProvider.Difficulty.Medium => config.healthMedium,
            DifficultyProvider.Difficulty.Hard => config.healthHard,
            _ => config.healthEasy
        };

        CurrentHealth = health;

        Vector3 currentDirection;
        this.SetInitialPosition(playerMovement.movementLogic.Position, out targetPosition, out currentDirection);
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
        if (this == null || !gameObject.activeSelf) return;

        while (this != null && gameObject.activeSelf)
        {
            if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
            {
                targetPosition = playerMovement.movementLogic.Position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0f);
            }

            if (IsInvincible)
            {
                await UniTask.Yield();
                continue;
            }

            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

            await UniTask.Yield();
        }

        if (this != null && gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("Player") || collision.CompareTag("Projectile")) && !IsInvincible)
        {
            this.TakeDamage();
            if (!gameObject.activeSelf)
            {
                scoreManager.AddDestroyedUFO(1);
            }
            this.HandleHitVisualsWithDelay().Forget();
        }
    }

    private async UniTaskVoid HandleHitVisualsWithDelay()
    {
        await UniTask.Delay(75);
        this.HandleHitVisuals().Forget();
    }

}
