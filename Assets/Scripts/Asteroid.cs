using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class Asteroid : MonoBehaviour
{
    private Vector3 targetPosition;
    private Vector3 currentDirection;
    private float moveSpeed = 10f;
    private float rotationSpeed = 50f;
    private float lifetime = 15f;
    private int health = 1;
    public bool IsInvincible { get; set; }

    [Inject] private PlayerTeleport playerTeleport;
    [Inject] private PlayerMove playerMovement;

    [Inject(Optional = true)] private DifficultyManager difficultySettings;

    private SpriteRenderer spriteRenderer;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetInitialPosition();
        ApplyDifficulty();
        StartMove(moveSpeed, rotationSpeed);
        StartLifeCountdown().Forget();
    }

    private void ApplyDifficulty()
    {
        if (difficultySettings != null)
        {
            switch (difficultySettings.CurrentDifficulty)
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
                    health = 1;
                    break;
            }
        }
    }

    private void SetInitialPosition()
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

        Vector3 playerPos = playerMovement.savedPosition;
        Vector3 offset = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0f);
        targetPosition = playerPos + offset;

        currentDirection = (targetPosition - transform.position).normalized;
    }

    public void StartMove(float moveSpeed, float rotationSpeed)
    {
        this.moveSpeed = moveSpeed;
        this.rotationSpeed = rotationSpeed;
        MoveToTarget().Forget();
    }

    private async UniTaskVoid MoveToTarget()
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
            {
                targetPosition += currentDirection * 5f;
            }
            currentDirection = (targetPosition - transform.position).normalized;
            transform.position += currentDirection * moveSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            await UniTask.Yield();
        }
    }

    private async UniTaskVoid StartLifeCountdown()
    {
        await UniTask.Delay((int)(lifetime * 1000));
        gameObject.SetActive(false);
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
        SetInvincibility(true);
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

    public void SetInvincibility(bool state)
    {
        IsInvincible = state;
    }

    public bool GetInvincibility()
    {
        return IsInvincible;
    }

}
