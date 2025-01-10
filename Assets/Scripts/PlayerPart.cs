using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class PlayerPart : MonoBehaviour, IInvincible
{
    public enum PartType { Engine, Weapon, Core }

    [SerializeField] private PartType partType;
    [SerializeField] private PlayerHealth playerHealth;

    public bool IsInvincible { get; set; }

    [Inject] private PlayerMove playerMove;
    [Inject] private PlayerShoot playerShoot;


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsInvincible) return;
        if (collider.gameObject.CompareTag("Enemy"))
        {
            switch (partType)
            {
                case PartType.Engine:
                    HandleEngineHit();
                    break;
                case PartType.Weapon:
                    HandleWeaponHit();
                    break;
                case PartType.Core:
                    HandleCoreHit();
                    break;
            }
        }
    }

    private void HandleEngineHit()
    {
        playerHealth.DecreaseHealth();
        playerHealth.ActivateInvincibility().Forget();

        // Логика уменьшения скорости разгона через PlayerMove
        Debug.Log("Engine hit! Decreasing acceleration.");
        playerMove?.ReduceSpeedAndAcceleration();
        gameObject.SetActive(false);
    }

    private void HandleWeaponHit()
    {
        playerHealth.DecreaseHealth();
        playerHealth.ActivateInvincibility().Forget();

        // Логика уменьшения скорости стрельбы через PlayerShoot
        Debug.Log("Weapon hit! Increasing fire rate cooldown.");
        playerShoot?.IncreaseFireRateCooldown();
        gameObject.SetActive(false);
    }

    private void HandleCoreHit()
    {
        playerHealth.DecreaseHealth();
        playerHealth.ActivateInvincibility().Forget();
        Debug.Log("Core hit! Health decreased.");
    }
}
