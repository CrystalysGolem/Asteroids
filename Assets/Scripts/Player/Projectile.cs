using UnityEngine;
using Cysharp.Threading.Tasks;

public class Projectile : MonoBehaviour
{
    public float lifetime = 7.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var invincibleObject = collision.gameObject.GetComponent<IInvincible>();
        if (collision.GetComponent<IEnemy>() != null && (invincibleObject == null || !invincibleObject.IsInvincible))
        {
            Destroy(gameObject);
        }
    }

    public async void Launch(Vector3 direction, float force)
    {
        float timeAlive = 0f;
        float currentForce = force;

        direction = Quaternion.Euler(0f, 0f, 90f) * direction;

        while (this != null && gameObject.activeSelf && timeAlive < lifetime)
        {
            transform.position += direction * currentForce * Time.deltaTime;

            float factor = Mathf.Lerp(1f, 2f / 3f, timeAlive / lifetime);
            currentForce = force * factor;
            timeAlive += Time.deltaTime;
            await UniTask.Yield();
        }

        if (this != null && gameObject.activeSelf)
            Destroy(gameObject);
    }

}
