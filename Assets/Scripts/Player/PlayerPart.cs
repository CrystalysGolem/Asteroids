using UnityEngine;
using Zenject;

public class PlayerPart : MonoBehaviour, IInvincible
{
    [Inject] private PlayerMove playerMove;
    [Inject] private PlayerShoot playerShoot;

    [SerializeField] private PartType partType;
    [SerializeField] private PlayerHealth playerHealth;

    public enum PartType { Engine1, Engine2, Weapon, Core }

    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public GameObject GameObject => gameObject;
    public bool IsInvincible { get; set; }

    private bool wpnDestroyed;
    private bool eng1Destroyed;
    private bool eng2Destroyed;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsInvincible) return;
        var GetEnemy = collider.gameObject.GetComponent<IEnemy>();
        if (GetEnemy == null) return;
        var invincibleEnemy = collider.GetComponent<IInvincible>();
        if (invincibleEnemy != null && invincibleEnemy.IsInvincible) return;

        switch (partType)
        {
            case PartType.Engine1:
                HandleEngineHit(ref eng1Destroyed, true);
                break;
            case PartType.Engine2:
                HandleEngineHit(ref eng2Destroyed, false);
                break;
            case PartType.Weapon:
                HandleWeaponHit();
                break;
            case PartType.Core:
                HandleCoreHit();
                break;
        }
    }

    private void HandleEngineHit(ref bool engineDestroyed, bool isEngine1)
    {
        if (engineDestroyed) return;
        engineDestroyed = true;
        playerHealth.DecreaseHealth();
        playerHealth.ActivateInvincibility();
        playerMove?.ReduceSpeedAndAcceleration();
        if (isEngine1)
            playerHealth.NotifyEngine1Destroyed();
        else
            playerHealth.NotifyEngine2Destroyed();
        gameObject.SetActive(false);
    }

    private void HandleWeaponHit()
    {
        if (wpnDestroyed) return;
        wpnDestroyed = true;
        playerHealth.DecreaseHealth();
        playerHealth.ActivateInvincibility();
        playerShoot?.IncreaseFireRateCooldown();
        playerHealth.NotifyWeaponDestroyed();
        gameObject.SetActive(false);
    }

    private void HandleCoreHit()
    {
        playerHealth.DecreaseHealth();
        playerHealth.ActivateInvincibility();
    }
}
