using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float shootingForce = 20f;
    public float shootCooldown = 0.25f;
    public int maxAmmo = 10;
    public float reloadTime = 2f;

    public GameObject activeObject;
    public float objectActiveDuration = 3f;
    public float objectCooldown = 30f;

    private int currentAmmo;
    private bool canShoot = true;
    private bool isReloading = false;
    private bool isObjectActive = false;
    private bool isObjectOnCooldown = false;
    private float objectActiveTime = 0f;

    private void Start()
    {
        currentAmmo = maxAmmo;
        HandleShooting().Forget();
        HandleObjectActivation().Forget();
    }

    private async UniTaskVoid HandleShooting()
    {
        while (this != null && gameObject.activeSelf)
        {
            if (Input.GetMouseButton(0) && canShoot && currentAmmo > 0 && !isReloading)
            {
                canShoot = false;

                Vector3 spawnPosition = transform.position + transform.right * 0.5f;
                GameObject bullet = Instantiate(bulletPrefab, spawnPosition, transform.rotation);
                bullet.GetComponent<Projectile>().Launch(transform.right, shootingForce);
                currentAmmo--;

                if (currentAmmo == 0)
                {
                    await Reload();
                }

                await UniTask.Delay((int)(shootCooldown * 1000));
                canShoot = true;
            }

            if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !isReloading)
            {
                await Reload();
            }

            await UniTask.Delay(10);
        }
    }

    private async UniTaskVoid HandleObjectActivation()
    {
        while (this != null && gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(1) && !isObjectActive && !isObjectOnCooldown)
            {
                ActivateObject();
            }

            if (isObjectActive)
            {
                objectActiveTime += Time.deltaTime;

                if (Input.GetMouseButtonUp(1) || objectActiveTime >= objectActiveDuration)
                {
                    DeactivateObject();
                    float usedRatio = objectActiveTime / objectActiveDuration;
                    float cooldownDuration = objectCooldown * usedRatio;
                    await StartObjectCooldown(cooldownDuration);
                }
            }

            await UniTask.Delay(10);
        }
    }

    private void ActivateObject()
    {
        isObjectActive = true;
        objectActiveTime = 0f;
        activeObject.SetActive(true);
        Debug.Log("Object Activated!");
    }

    private void DeactivateObject()
    {
        isObjectActive = false;
        activeObject.SetActive(false);
        Debug.Log("Object Deactivated!");
    }

    private async UniTask StartObjectCooldown(float cooldownDuration)
    {
        if (this != null && gameObject.activeSelf)
        {
            isObjectOnCooldown = true;
            Debug.Log($"Object cooldown started for {cooldownDuration:F1} seconds.");
            await UniTask.Delay((int)(cooldownDuration * 1000));
            isObjectOnCooldown = false;
            Debug.Log("Object cooldown complete!");
        }
    }

    private async UniTask Reload()
    {
        if (this != null && gameObject.activeSelf)
        {
            isReloading = true;
            Debug.Log("Reloading...");
            await UniTask.Delay((int)(reloadTime * 1000));
            currentAmmo = maxAmmo;
            isReloading = false;
            Debug.Log("Reload complete!");
        }
    }

    public void IncreaseFireRateCooldown()
    {
        shootCooldown *= 1.5f;
        Debug.Log($"Fire rate cooldown increased. Current cooldown: {shootCooldown}");
    }
}
