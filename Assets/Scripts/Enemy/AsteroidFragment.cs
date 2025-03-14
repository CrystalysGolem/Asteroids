using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class AsteroidFragment : MonoBehaviour, IEnemy, IHealth
{
    [Inject] private AsteroidFragmentConfigLoader configLoader;
    [Inject] private DifficultyProvider difficultySettings;

    public int CurrentHealth { get; set; }

    private float rotationSpeed;
    private float moveSpeed;
    private float minSpeed;
    private float maxSpeed;
    private float minSpeedRotation;
    private float maxSpeedRotation;
    private Vector3 moveDirection;
    private AsteroidFragmentConfig config;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerPart>() || collision.GetComponent<Projectile>())
        {
            this.TakeDamage();
        }
    }

    public void StartUP()
    {
        config = configLoader.LoadConfig();
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
            DifficultyProvider.Difficulty.Easy => config.healthEasy,
            DifficultyProvider.Difficulty.Medium => config.healthMedium,
            DifficultyProvider.Difficulty.Hard => config.healthHard,
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
}
