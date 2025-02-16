using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IInvincible
{
    bool IsInvincible { get; set; }
    SpriteRenderer SpriteRenderer { get; }
    GameObject GameObject { get; }
}

public static class IInvincibleExtensions
{
    public static async UniTask HandleHitVisuals(this IInvincible invincible)
    {
        if (invincible == null || invincible.GameObject == null || !invincible.GameObject.activeSelf) return;

        invincible.IsInvincible = true;
        float blinkDuration = 1f;
        float blinkInterval = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            if (invincible == null || invincible.GameObject == null || !invincible.GameObject.activeSelf) return;

            invincible.SpriteRenderer.enabled = !invincible.SpriteRenderer.enabled;
            await UniTask.Delay((int)(blinkInterval * 1000));
            elapsedTime += blinkInterval;
        }

        if (invincible != null && invincible.GameObject != null && invincible.GameObject.activeSelf)
            invincible.SpriteRenderer.enabled = true;

        invincible.IsInvincible = false;
    }
}
