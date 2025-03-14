using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Zenject;

public class PlayerShoot : MonoBehaviour
{
    [Inject] private OptionsProvider optionsManager;
    [Inject] private ScoreProvider scoreManager;
    [Inject] private PlayerConfigLoader configLoader;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject activeObject;
    [SerializeField] private JoyStick AttackStick;
    [SerializeField] private AudioSource shootAudioSource;
    [SerializeField] private AudioClip shootSound;

    public event Action<int> OnAmmoChanged;
    public event Action<bool> OnReloadStateChanged;
    public event Action<bool> OnObjectActivationChanged;
    public event Action<bool> OnObjectCooldownChanged;
    public event Action<float> OnObjectCooldownTimeChanged;
    public event Action<float> OnObjectActiveTimeChanged;

    public float shootingForce;
    public float shootCooldown;
    public int maxAmmo;
    public float reloadTime;
    public float objectActiveDuration;
    public float objectCooldown;
    public float objectActiveTime = 0f;
    public float cooldownDuration = 0f;
    public float spawnOffset = 0.35f;
    public int degree = -90;
    public float angle;

    private int currentAmmo;
    private int MillisecondsInSecond = 1000;
    private int scriptDelay = 10;
    private float baseSoundVolume = 0.2f;
    private float minimalAttackStickSpeed = 0.2f;
    private bool canShoot = true;
    private bool ShootLeft = true;
    private bool isReloading = false;
    private bool isObjectActive = false;
    private bool isObjectOnCooldown = false;
    private bool IsMobile = false;
    private CancellationTokenSource cancellationTokenSource;

    private void Start()
    {
        cancellationTokenSource = new CancellationTokenSource();

        IsMobile = optionsManager.IsMobile;
        var config = configLoader.LoadConfig();
        shootingForce = config.shootingForce;
        shootCooldown -= config.shootCooldown;
        maxAmmo = config.maxAmmo;
        reloadTime = config.reloadTime;
        objectActiveDuration = config.objectActiveDuration;
        objectCooldown = config.objectCooldown;
        currentAmmo = maxAmmo;
        if (IsMobile)
            spawnOffset = 0f;

        ShootingInputLoop(cancellationTokenSource.Token).Forget();
        ObjectActivationInputLoop(cancellationTokenSource.Token).Forget();
    }
    private void OnDestroy()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }

    public void IncreaseFireRateCooldown()
    {
        shootCooldown = Mathf.Max(0.1f, shootCooldown * 1.5f);
    }

    public async void ToggleObjectActivation()
    {
        if (!IsMobile)
            return;
        if (!isObjectActive && !isObjectOnCooldown)
        {
            ActivateObject();
            scoreManager.AddFiredLasers(1);
        }
        else if (isObjectActive)
        {
            DeactivateObject();
            float usedRatio = objectActiveTime / objectActiveDuration;
            cooldownDuration = objectCooldown * usedRatio;
            await StartObjectCooldown(cooldownDuration);
        }
    }

    private async UniTaskVoid ShootingInputLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested && this != null && gameObject.activeSelf)
        {
            if (IsMobile)
            {
                if (AttackStick != null &&
                    AttackStick.Speed > minimalAttackStickSpeed &&
                    canShoot && currentAmmo > 0 && !isReloading)
                {
                    await HandleShootingAction();
                }
            }
            else
            {
                if (Input.GetMouseButton(0) && canShoot && currentAmmo > 0 && !isReloading)
                {
                    await HandleShootingAction();
                }
            }
            if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !isReloading)
            {
                await Reload();
            }
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

    private async UniTask HandleShootingAction()
    {
        if (!canShoot) return;

        canShoot = false;
        if (IsMobile)
        {
            Vector2 shootDirection = AttackStick.Direction.normalized;
            Vector2 rotatedDirection = Quaternion.Euler(0f, 0f, degree) * shootDirection;
            angle = Mathf.Atan2(rotatedDirection.y, rotatedDirection.x) * Mathf.Rad2Deg;
            Vector3 spawnPosition = transform.position + (Vector3)rotatedDirection * spawnOffset;
            Quaternion bulletRotation = Quaternion.Euler(0f, 0f, angle);
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, bulletRotation);
            bullet.GetComponent<Projectile>().Launch((Vector3)rotatedDirection, shootingForce);
        }
        else
        {
            Vector3 spawnPosition = ShootLeft ? transform.position + transform.right * spawnOffset : transform.position + transform.right * -spawnOffset;
            ShootLeft = !ShootLeft;
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, transform.rotation);
            bullet.GetComponent<Projectile>().Launch(transform.right, shootingForce);
        }
        currentAmmo--;
        OnAmmoChanged?.Invoke(currentAmmo);

        if (shootAudioSource != null && shootSound != null)
        {
            shootAudioSource.volume = optionsManager.CurrentOptions.soundVolume * baseSoundVolume;
            shootAudioSource.PlayOneShot(shootSound);
        }

        if (currentAmmo == 0)
        {
            await Reload();
        }
        scoreManager.AddFiredBullets(1);

        int delay = Mathf.Max(1, (int)(shootCooldown * MillisecondsInSecond));
        await UniTask.Delay(delay);
        canShoot = true;
    }

    private async UniTaskVoid ObjectActivationInputLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested && this != null && gameObject.activeSelf)
        {
            if (!IsMobile)
            {
                if (Input.GetMouseButtonDown(1) && !isObjectActive && !isObjectOnCooldown)
                {
                    ActivateObject();
                    scoreManager.AddFiredLasers(1);
                }
            }
            if (isObjectActive)
            {
                objectActiveTime += Time.deltaTime;
                OnObjectActiveTimeChanged?.Invoke(objectActiveTime);
                if ((!IsMobile && Input.GetMouseButtonUp(1)) || objectActiveTime >= objectActiveDuration)
                {
                    DeactivateObject();
                    float usedRatio = objectActiveTime / objectActiveDuration;
                    cooldownDuration = objectCooldown * usedRatio;
                    await StartObjectCooldown(cooldownDuration);
                }
            }
            await UniTask.Yield(PlayerLoopTiming.Update, token);
            scoreManager.AddLaserTime(scriptDelay / (float)MillisecondsInSecond);
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
            await UniTask.Delay(MillisecondsInSecond);
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
            await UniTask.Delay((int)(reloadTime * MillisecondsInSecond));
            currentAmmo = maxAmmo;
            OnAmmoChanged?.Invoke(currentAmmo);
            isReloading = false;
            OnReloadStateChanged?.Invoke(isReloading);
            scoreManager.AddReloads(1);
        }
    }
}
