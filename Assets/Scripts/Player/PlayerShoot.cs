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
    private bool ShootLeft = true;
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
                Vector3 spawnPosition;
                if (ShootLeft == true)
                {
                    spawnPosition = transform.position + transform.right * 0.35f;
                    ShootLeft = false;
                }
                else
                {
                    spawnPosition = transform.position + transform.right * -0.35f;
                    ShootLeft = true;
                }

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
    }

    private void DeactivateObject()
    {
        isObjectActive = false;
        activeObject.SetActive(false);
    }

    private async UniTask StartObjectCooldown(float cooldownDuration)
    {
        if (this != null && gameObject.activeSelf)
        {
            isObjectOnCooldown = true;
            await UniTask.Delay((int)(cooldownDuration * 1000));
            isObjectOnCooldown = false;
        }
    }

    private async UniTask Reload()
    {
        if (this != null && gameObject.activeSelf)
        {
            isReloading = true;
            await UniTask.Delay((int)(reloadTime * 1000));
            currentAmmo = maxAmmo;
            isReloading = false;
        }
    }

    public void IncreaseFireRateCooldown()
    {
        shootCooldown *= 1.5f;
    }
}
