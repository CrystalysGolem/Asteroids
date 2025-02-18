using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class PlayerHealth : MonoBehaviour
{
    public event Action<int> OnHealthChanged;
    public event Action<bool> OnWeaponDestroyed;
    public event Action<bool> OnEngine1Destroyed;
    public event Action<bool> OnEngine2Destroyed;

    private int currentHealth = 3;
    public int CurrentHealth => currentHealth;
    [SerializeField] private List<GameObject> playerParts = new List<GameObject>();

    private List<IInvincible> invincibleParts = new List<IInvincible>();

    private void Awake()
    {
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
