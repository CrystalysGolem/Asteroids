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
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] public bool IsInvincible { get; set; }

    [Inject] private PlayerTeleport playerTeleport;
    [Inject] private PlayerMove playerMovement;
    [Inject] private PrefabFactory prefabFactory;

    private GameObject _fragmentPrefab; 
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
        this.SetInitialPosition(playerTeleport, playerMovement,out targetPosition,out currentDirection);
        this.ApplyDifficulty(1,2,3, difficultySettings.CurrentDifficulty);
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
            if(gameObject.activeSelf == false)
            {
                SpawnFragments();
            }
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
