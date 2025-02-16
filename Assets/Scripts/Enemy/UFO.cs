using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class UFO : MonoBehaviour, IInvincible, IEnemy, IHealth
{
    public class Factory : PlaceholderFactory<UFO> { }

    [SerializeField] public int CurrentHealth { get; set; }
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float moveSpeed = 7.5f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] public bool IsInvincible { get; set; }
    [Inject] private PlayerMove playerMovement;
    [Inject] private PlayerTeleport playerTeleport;
    [InjectOptional] private DifficultyManager difficultySettings;

    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public GameObject GameObject => gameObject;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartUP();
    }

    public void StartUP()
    {
        this.ApplyDifficulty(1,2,3, difficultySettings.CurrentDifficulty);
        Vector3 currentDirection;
        this.SetInitialPosition(playerTeleport, playerMovement, out targetPosition, out currentDirection);
        StartMove(moveSpeed, rotationSpeed);
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
        if (this == null || !gameObject.activeSelf) return;

        if (collision != null && collision.CompareTag("Projectile") && !IsInvincible)
        {
            this.TakeDamage();
        }
    }
}
