using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class Asteroid : MonoBehaviour, IInvincible, IEnemy
{
    public class Factory : PlaceholderFactory<Asteroid> { }

    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Vector3 currentDirection;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private int health = 1;

    [SerializeField] public bool IsInvincible { get; set; }

    [Inject] private PlayerTeleport playerTeleport;
    [Inject] private PlayerMove playerMovement;
    [Inject] private PrefabFactory prefabFactory;

    private GameObject _fragmentPrefab; // Фрагмент теперь передаётся фабрикой
    [Inject(Optional = true)] private DifficultyManager difficultySettings;

    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public GameObject GameObject => gameObject;

    private SpriteRenderer spriteRenderer;

    public void SetFragmentPrefab(GameObject fragmentPrefab)
    {
        _fragmentPrefab = fragmentPrefab;
    }

    public void StartUP()
    {
        SetInitialPosition();
        ApplyDifficulty();
        StartMove(moveSpeed, rotationSpeed);
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

        Vector3 playerPos = playerMovement.movementLogic.Position;
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
        while (this != null && gameObject.activeSelf)
        {
            transform.position += currentDirection * moveSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            await UniTask.Yield();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile") && !IsInvincible)
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            SpawnFragments();
            gameObject.SetActive(false);
        }
    }

    private void SpawnFragments()
    {
        int fragmentCount = Random.Range(2, 5);

        for (int i = 0; i < fragmentCount; i++)
        {
            prefabFactory.SpawnPrefabInstantly(_fragmentPrefab, transform.position);
        }
    }
}
