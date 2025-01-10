using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health = 3;
    [SerializeField] private List<GameObject> playerParts = new List<GameObject>();
    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

    public int Health => health;

    private void Awake()
    {
        foreach (var part in playerParts)
        {
            if (part != null)
            {
                var renderers = part.GetComponentsInChildren<SpriteRenderer>();
                spriteRenderers.AddRange(renderers);
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
        var invincibleParts = new List<IInvincible>();

        foreach (var part in playerParts)
        {
            if (part != null && part.TryGetComponent(out IInvincible invinciblePart))
            {
                invincibleParts.Add(invinciblePart);
                invinciblePart.IsInvincible = true;
            }
        }

        if (this != null && gameObject.activeSelf)
        {
            await HandleHitVisuals(duration);
        }

        foreach (var invinciblePart in invincibleParts)
        {
            if (this != null && gameObject.activeSelf)
            {
                invinciblePart.IsInvincible = false;
            }
        }
    }

    private async UniTask HandleHitVisuals(float duration)
    {
        float elapsedTime = 0f;
        float blinkInterval = 0.1f;

        while (elapsedTime < duration && this != null && gameObject.activeSelf)
        {
            foreach (var renderer in spriteRenderers)
            {
                if (renderer != null)
                {
                    renderer.enabled = !renderer.enabled;
                }
            }

            await UniTask.Delay((int)(blinkInterval * 1000));
            elapsedTime += blinkInterval;
        }

        if (this != null && gameObject.activeSelf)
        {
            foreach (var renderer in spriteRenderers)
            {
                if (renderer != null)
                {
                    renderer.enabled = true;
                }
            }
        }
    }
}
