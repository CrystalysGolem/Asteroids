using UnityEngine;
using Cysharp.Threading.Tasks;

public class Projectile : MonoBehaviour
{
    public float lifetime = 7.5f;

    public async void Launch(Vector3 direction, float force)
    {
        float timeAlive = 0f;
        float currentForce = force;

        while (timeAlive < lifetime)
        {
            transform.position += direction * currentForce * Time.deltaTime;

            float factor = Mathf.Lerp(1f, 2f / 3f, timeAlive / lifetime);
            currentForce = force * factor;

            timeAlive += Time.deltaTime;
            await UniTask.Yield();
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var invincibleObject = collision.gameObject.GetComponent<IInvincible>();
        if (collision.CompareTag("Enemy") && (invincibleObject == null || !invincibleObject.IsInvincible))
        {
            Destroy(gameObject);
        }
    }
}
