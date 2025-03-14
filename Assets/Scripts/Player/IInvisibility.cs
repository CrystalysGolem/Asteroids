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
    private const float BlinkDuration = 1f;
    private const float BlinkInterval = 0.1f;
    private const int MillisecondsInSecond = 1000;

    public static async UniTask HandleHitVisuals(this IInvincible invincible)
    {
        if (invincible == null || invincible.GameObject == null || !invincible.GameObject.activeSelf) return;

        invincible.IsInvincible = true;
        float elapsedTime = 0f;

        while (elapsedTime < BlinkDuration)
        {
            if (invincible == null || invincible.GameObject == null || !invincible.GameObject.activeSelf) return;

            invincible.SpriteRenderer.enabled = !invincible.SpriteRenderer.enabled;
            await UniTask.Delay((int)(BlinkInterval * MillisecondsInSecond));
            elapsedTime += BlinkInterval;
        }

        if (invincible != null && invincible.GameObject != null && invincible.GameObject.activeSelf)
            invincible.SpriteRenderer.enabled = true;

        invincible.IsInvincible = false;
    }
}
