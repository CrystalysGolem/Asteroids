using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class UFO : MonoBehaviour, IInvincible
{
    private Vector3 targetPosition;
    private float moveSpeed = 7.5f;
    private float rotationSpeed = 50f;
    private float lifetime = 15f;
    private int health;

    public bool IsInvincible { get; set; }

    [Inject] private PlayerMove playerMovement;
    [Inject] private PlayerTeleport playerTeleport;
    [InjectOptional] private DifficultyManager difficultyLevel;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (playerMovement == null || playerTeleport == null)
        {
            Debug.LogError("Missing required dependencies: PlayerMove or PlayerTeleport.");
            gameObject.SetActive(false);
            return;
        }

        SetHealthByDifficulty();
        StartMove(moveSpeed, rotationSpeed);
        StartLifeCountdown().Forget();
    }

    private void SetHealthByDifficulty()
    {
        switch (difficultyLevel?.CurrentDifficulty ?? DifficultyManager.Difficulty.Easy)
        {
            case DifficultyManager.Difficulty.Easy:
                health = 1;
                break;
            case DifficultyManager.Difficulty.Medium:
                health = 2;
                break;
            case DifficultyManager.Difficulty.Hard:
                health = 3;
                break;
            default:
                Debug.LogWarning("Unknown difficulty level. Defaulting to Easy.");
                health = 1;
                break;
        }
    }

    public void StartMove(float moveSpeed, float rotationSpeed)
    {
        this.moveSpeed = moveSpeed;
        this.rotationSpeed = rotationSpeed;
        targetPosition = playerMovement != null ? playerMovement.savedPosition : Vector3.zero;
        MoveToTarget().Forget();
    }

    private bool IsWithinBounds()
    {
        float xBoundary = playerTeleport.xBoundary;
        float yBoundary = playerTeleport.yBoundary;

        return Mathf.Abs(transform.position.x) <= xBoundary && Mathf.Abs(transform.position.y) <= yBoundary;
    }

    private async UniTaskVoid MoveToTarget()
    {
        if (this == null || !gameObject.activeSelf) return;

        float xBoundary = playerTeleport.xBoundary;
        float yBoundary = playerTeleport.yBoundary;

        int side = Random.Range(0, 4);
        switch (side)
        {
            case 0: transform.position = new Vector3(Random.Range(-xBoundary, xBoundary), yBoundary, 0f); break;
            case 1: transform.position = new Vector3(Random.Range(-xBoundary, xBoundary), -yBoundary, 0f); break;
            case 2: transform.position = new Vector3(-xBoundary, Random.Range(-yBoundary, yBoundary), 0f); break;
            case 3: transform.position = new Vector3(xBoundary, Random.Range(-yBoundary, yBoundary), 0f); break;
        }

        while (this != null && gameObject.activeSelf && IsWithinBounds())
        {
            targetPosition = playerMovement != null ? playerMovement.savedPosition : Vector3.zero;
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            await UniTask.Yield();
        }

        if (this != null && gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    private async UniTaskVoid StartLifeCountdown()
    {
        if (this == null || !gameObject.activeSelf) return;

        await UniTask.Delay((int)(lifetime * 1000));
        if (this != null && gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this == null || !gameObject.activeSelf) return;

        if (collision != null && collision.CompareTag("Projectile") && !IsInvincible)
        {
            health--;
            if (health <= 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                HandleHitVisuals().Forget();
            }
        }
    }

    private async UniTaskVoid HandleHitVisuals()
    {
        if (this == null || !gameObject.activeSelf) return;

        IsInvincible = true;
        float blinkDuration = 0.5f;
        float blinkInterval = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            if (this == null || !gameObject.activeSelf) return;

            spriteRenderer.enabled = !spriteRenderer.enabled;
            await UniTask.Delay((int)(blinkInterval * 1000));
            elapsedTime += blinkInterval;
        }

        if (this != null && gameObject.activeSelf)
            spriteRenderer.enabled = true;
        IsInvincible = false;
    }
}
