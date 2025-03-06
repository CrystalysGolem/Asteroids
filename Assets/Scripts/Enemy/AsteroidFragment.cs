using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class AsteroidFragment : MonoBehaviour, IEnemy, IHealth
{
    public class Factory : PlaceholderFactory<AsteroidFragment> { }

    // Move logic
    private float rotationSpeed;
    private float moveSpeed;

    // Randomized
    private float minSpeed;
    private float maxSpeed;
    private float minSpeedRotation;
    private float maxSpeedRotation;
    private Vector3 moveDirection;

    // Health logic
    [SerializeField] public int CurrentHealth { get; set; }

    [Inject] private DifficultyManager difficultySettings;

    public void StartUP()
    {
        var config = AsteroidFragmentConfigLoader.LoadConfig();
        if (config == null)
        {
            Debug.LogError("AsteroidFragmentConfig not loaded!");
            return;
        }

        minSpeed = config.minSpeed;
        maxSpeed = config.maxSpeed;
        minSpeedRotation = config.minSpeedRotation;
        maxSpeedRotation = config.maxSpeedRotation;
        rotationSpeed = config.rotationSpeed;

        int health = difficultySettings.CurrentDifficulty switch
        {
            DifficultyManager.Difficulty.Easy => config.healthEasy,
            DifficultyManager.Difficulty.Medium => config.healthMedium,
            DifficultyManager.Difficulty.Hard => config.healthHard,
            _ => config.healthEasy
        };

        CurrentHealth = health;

        gameObject.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        moveDirection = Random.insideUnitCircle.normalized;
        moveSpeed = Random.Range(minSpeed, maxSpeed);
        rotationSpeed = Random.Range(minSpeedRotation, maxSpeedRotation);

        StartMove().Forget();
    }

    private async UniTaskVoid StartMove()
    {
        float lifetime = 60f;
        float startTime = Time.time;

        while (this != null && gameObject.activeSelf)
        {
            if (Time.time - startTime >= lifetime)
            {
                gameObject.SetActive(false);
                break;
            }
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            await UniTask.Yield();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Projectile"))
        {
            this.TakeDamage();
        }
    }
}
