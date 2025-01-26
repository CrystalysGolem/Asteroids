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

    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public GameObject GameObject => gameObject;

    private SpriteRenderer spriteRenderer;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsInvincible) return;

        if (collider.gameObject.CompareTag("Enemy"))
        {
            var enemy = collider.gameObject.GetComponent<IEnemy>();
            var invincibleEnemy = collider.gameObject.GetComponent<IInvincible>();

            if (enemy != null && (invincibleEnemy == null || !invincibleEnemy.IsInvincible))
            {
                enemy.TakeDamage();
                Debug.Log("Enemy Hit.");
            }
            else
            {
                Debug.Log("Enemy is invincible. No damage dealt.");
            }

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
        playerMove?.ReduceSpeedAndAcceleration();
        gameObject.SetActive(false);
    }

    private void HandleWeaponHit()
    {
        playerHealth.DecreaseHealth();
        playerHealth.ActivateInvincibility().Forget();
        playerShoot?.IncreaseFireRateCooldown();
        gameObject.SetActive(false);
    }

    private void HandleCoreHit()
    {
        playerHealth.DecreaseHealth();
        playerHealth.ActivateInvincibility().Forget();
    }
}
