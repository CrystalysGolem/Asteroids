using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class UFO : MonoBehaviour
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
        }
    }

    public void StartMove(float moveSpeed, float rotationSpeed)
    {
        this.moveSpeed = moveSpeed;
        this.rotationSpeed = rotationSpeed;
        targetPosition = playerMovement != null ? playerMovement.savedPosition : Vector3.zero;
        MoveToTarget().Forget();
    }

    private async UniTaskVoid MoveToTarget()
    {
        float xBoundary = playerTeleport.xBoundary;
        float yBoundary = playerTeleport.yBoundary;

        int side = Random.Range(0, 4);
        switch (side)
        {
            case 0: // Top
                transform.position = new Vector3(Random.Range(-xBoundary, xBoundary), yBoundary, 0f);
                break;
            case 1: // Bottom
                transform.position = new Vector3(Random.Range(-xBoundary, xBoundary), -yBoundary, 0f);
                break;
            case 2: // Left
                transform.position = new Vector3(-xBoundary, Random.Range(-yBoundary, yBoundary), 0f);
                break;
            case 3: // Right
                transform.position = new Vector3(xBoundary, Random.Range(-yBoundary, yBoundary), 0f);
                break;
        }

        while (true)
        {
            targetPosition = playerMovement != null ? playerMovement.savedPosition : Vector3.zero;
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            await UniTask.Yield();
        }
    }

    private async UniTaskVoid StartLifeCountdown()
    {
        await UniTask.Delay((int)(lifetime * 1000));
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PP") && !GetInvincibility())
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
        SetInvincibility(true);
        float blinkDuration = 0.5f;
        float blinkInterval = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            await UniTask.Delay((int)(blinkInterval * 1000));
            elapsedTime += blinkInterval;
        }

        spriteRenderer.enabled = true;
        SetInvincibility(false);
    }

    public void SetInvincibility(bool state)
    {
        IsInvincible = state;
    }

    public bool GetInvincibility()
    {
        return IsInvincible;
    }

}
