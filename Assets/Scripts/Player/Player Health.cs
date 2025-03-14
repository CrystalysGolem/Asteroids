using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using Zenject;

public class PlayerHealth : MonoBehaviour
{
    [Inject] private ScoreProvider _scoreManager;
    [Inject] private PlayerConfigLoader configLoader;

    [SerializeField] private List<GameObject> playerParts = new List<GameObject>();

    public event Action<int> OnHealthChanged;
    public event Action<bool> OnWeaponDestroyed;
    public event Action<bool> OnEngine1Destroyed;
    public event Action<bool> OnEngine2Destroyed;

    public int CurrentHealth => currentHealth;

    private int currentHealth;
    private List<IInvincible> invincibleParts = new List<IInvincible>();

    private void Awake()
    {
        var config = configLoader.LoadConfig();
        currentHealth = config.currentHealth;

        foreach (var part in playerParts)
        {
            if (part != null && part.TryGetComponent(out IInvincible invinciblePart))
            {
                invincibleParts.Add(invinciblePart);
            }
        }
    }

    public void DecreaseHealth()
    {
        if (currentHealth > 0)
        {
            currentHealth--;
            OnHealthChanged?.Invoke(currentHealth);
            if (currentHealth <= 0)
            {
                gameObject.SetActive(false);
                Debug.Log("Player is dead!");
                _scoreManager.ScoreCounter();
                Time.timeScale = 0f;
            }
        }
    }

    public void IncreaseHealth()
    {
        currentHealth++;
        Debug.Log($"Health increased: {currentHealth}");
    }

    public void ActivateInvincibility()
    {
        foreach (var part in invincibleParts)
        {
            part.HandleHitVisuals().Forget();
        }
    }

    public void NotifyWeaponDestroyed()
    {
        OnWeaponDestroyed?.Invoke(true);
    }

    public void NotifyEngine1Destroyed()
    {
        OnEngine1Destroyed?.Invoke(true);
    }

    public void NotifyEngine2Destroyed()
    {
        OnEngine2Destroyed?.Invoke(true);
    }
}
