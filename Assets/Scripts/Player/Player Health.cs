using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health = 3;
    [SerializeField] private List<GameObject> playerParts = new List<GameObject>();

    private List<IInvincible> invincibleParts = new List<IInvincible>();

    public int Health => health;

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
        if (health > 0)
        {
            health--;
            if (health <= 0)
            {
                Debug.Log("Player is dead!");
            }
        }
    }

    public void IncreaseHealth()
    {
        health++;
        Debug.Log($"Health increased: {health}");
    }

    public async UniTask ActivateInvincibility(float duration = 3f)
    {
        foreach (var part in invincibleParts)
        {
            part.HandleHitVisuals();
        }
    }
}
