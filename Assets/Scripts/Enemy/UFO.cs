using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class UFO : MonoBehaviour, IInvincible, IEnemy
{
    public class Factory : PlaceholderFactory<UFO> { }

    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float moveSpeed = 7.5f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private int health;

    [SerializeField] public bool IsInvincible { get; set; }

    [Inject] private PlayerMove playerMovement;
    [Inject] private PlayerTeleport playerTeleport;
    [InjectOptional] private DifficultyManager difficultyLevel;

    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public GameObject GameObject => gameObject;

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
        StartUP();
    }

    public void StartUP()
    {
        SetHealthByDifficulty();
        SetInitialPosition();  
        StartMove(moveSpeed, rotationSpeed);
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

    private void SetInitialPosition()
    {
        float xBoundary = playerTeleport.xBoundary;
        float yBoundary = playerTeleport.yBoundary;
        Vector3 screenTopRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));
        Vector3 screenBottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        float xMax = screenTopRight.x;
        float yMax = screenTopRight.y;
        float xMin = screenBottomLeft.x;
        float yMin = screenBottomLeft.y;
        int side = Random.Range(0, 4);
        switch (side)
        {
            case 0:
                transform.position = new Vector3(Random.Range(xMin, xMax), yMax, 0f);
                break;
            case 1:
                transform.position = new Vector3(Random.Range(xMin, xMax), yMin, 0f);
                break;
            case 2:
                transform.position = new Vector3(xMin, Random.Range(yMin, yMax), 0f);
                break;
            case 3:
                transform.position = new Vector3(xMax, Random.Range(yMin, yMax), 0f);
                break;
        }

        targetPosition = playerMovement.savedPosition + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0f);
    }

    public void StartMove(float moveSpeed, float rotationSpeed)
    {
        this.moveSpeed = moveSpeed;
        this.rotationSpeed = rotationSpeed;
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

        while (this != null && gameObject.activeSelf)
        {
            if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
            {
                targetPosition = playerMovement.savedPosition + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0f);
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
        if (this == null || !gameObject.activeSelf) return;

        if (collision != null && collision.CompareTag("Projectile") && !IsInvincible)
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            this.HandleHitVisuals().Forget();
        }
    }
}
