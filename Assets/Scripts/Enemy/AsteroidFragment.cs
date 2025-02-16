using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class AsteroidFragment : MonoBehaviour, IEnemy, IHealth
{
    public class Factory : PlaceholderFactory<AsteroidFragment> { }

    [SerializeField] private float minSpeed = 2f;
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private float minSpeedRotation = 60f;
    [SerializeField] private float maxSpeedRotation = 120f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] public int CurrentHealth { get; set; }

    private Vector3 moveDirection;
    private float moveSpeed;
    private Vector3 spawnPosition;


    [Inject] private DifficultyManager difficultySettings;

    public void StartUP()
    {
        gameObject.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        moveDirection = Random.insideUnitCircle.normalized;
        moveSpeed = Random.Range(minSpeed, maxSpeed);
        rotationSpeed = Random.Range(minSpeedRotation, maxSpeedRotation);
        this.ApplyDifficulty(1,1,2,difficultySettings.CurrentDifficulty);
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
        if (collision != null && collision.CompareTag("Projectile"))
        {
            this.TakeDamage();
        }
    }
}
