using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using Zenject;
using UnityEngine.SceneManagement;

public class PlayerShoot : MonoBehaviour
{
    public event Action<int> OnAmmoChanged;
    public event Action<bool> OnReloadStateChanged;
    public event Action<bool> OnObjectActivationChanged;
    public event Action<bool> OnObjectCooldownChanged;
    public event Action<float> OnObjectCooldownTimeChanged;
    public event Action<float> OnObjectActiveTimeChanged;

    public GameObject bulletPrefab;
    public float shootingForce;
    public float shootCooldown;
    public int maxAmmo;
    public float reloadTime;

    public GameObject activeObject;
    public float objectActiveDuration;
    public float objectCooldown;

    public AudioSource shootAudioSource;
    public AudioClip shootSound;

    private int currentAmmo;
    private bool canShoot = true;
    private bool ShootLeft = true;
    private bool isReloading = false;
    private bool isObjectActive = false;
    private bool isObjectOnCooldown = false;
    public float objectActiveTime = 0f;
    public float cooldownDuration = 0f;

    [Inject] private OptionsManager optionsManager;
    [Inject] private ScoreManager scoreManager;

    private void Start()
    {
        var config = PlayerConfigLoader.LoadConfig();
        shootingForce = config.shootingForce;
        shootCooldown -= config.shootCooldown;
        maxAmmo = config.maxAmmo;
        reloadTime = config.reloadTime;
        objectActiveDuration = config.objectActiveDuration;
        objectCooldown = config.objectCooldown;
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
                Vector3 spawnPosition = ShootLeft ? transform.position + transform.right * 0.35f : transform.position + transform.right * -0.35f;
                ShootLeft = !ShootLeft;

                GameObject bullet = Instantiate(bulletPrefab, spawnPosition, transform.rotation);
                bullet.GetComponent<Projectile>().Launch(transform.right, shootingForce);
                currentAmmo--;
                OnAmmoChanged?.Invoke(currentAmmo);

                if (shootAudioSource != null && shootSound != null)
                {
                    shootAudioSource.volume = optionsManager.CurrentOptions.soundVolume * 0.2f;
                    shootAudioSource.PlayOneShot(shootSound);
                }

                if (currentAmmo == 0)
                {
                    await Reload();
                }

                scoreManager.AddFiredBullets(1);

                int delay = Mathf.Max(1, (int)(shootCooldown * 1000));
                await UniTask.Delay(delay);
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
                scoreManager.AddFiredLasers(1);
            }

            if (isObjectActive)
            {
                objectActiveTime += Time.deltaTime;
                OnObjectActiveTimeChanged?.Invoke(objectActiveTime);
                
                if (Input.GetMouseButtonUp(1) || objectActiveTime >= objectActiveDuration)
                {
                    DeactivateObject();
                    float usedRatio = objectActiveTime / objectActiveDuration;
                    cooldownDuration = objectCooldown * usedRatio;
                    await StartObjectCooldown(cooldownDuration);
                }
            }

            await UniTask.Delay(10);
            scoreManager.AddLaserTime(0.01f);
        }
    }

    private void ActivateObject()
    {
        isObjectActive = true;
        OnObjectActivationChanged?.Invoke(isObjectActive);
        objectActiveTime = 0f;
        activeObject.SetActive(true);
    }

    private void DeactivateObject()
    {
        isObjectActive = false;
        OnObjectActivationChanged?.Invoke(isObjectActive);
        activeObject.SetActive(false);
    }

    private async UniTask StartObjectCooldown(float cooldownDuration)
    {
        isObjectOnCooldown = true;
        OnObjectCooldownChanged?.Invoke(isObjectOnCooldown);

        float remainingCooldown = cooldownDuration;
        OnObjectCooldownTimeChanged?.Invoke(remainingCooldown);
        while (remainingCooldown > 0)
        {
            await UniTask.Delay(1000);
            remainingCooldown -= 1f;
        }
        isObjectOnCooldown = false;
        OnObjectCooldownChanged?.Invoke(isObjectOnCooldown);
    }

    private async UniTask Reload()
    {
        if (this != null && gameObject.activeSelf)
        {
            isReloading = true;
            OnReloadStateChanged?.Invoke(isReloading);
            await UniTask.Delay((int)(reloadTime * 1000));
            currentAmmo = maxAmmo;
            OnAmmoChanged?.Invoke(currentAmmo);
            isReloading = false;
            OnReloadStateChanged?.Invoke(isReloading);
            scoreManager.AddReloads(1);
        }
    }

    public void IncreaseFireRateCooldown()
    {
        shootCooldown = Mathf.Max(0.1f, shootCooldown * 1.5f);
    }
}
