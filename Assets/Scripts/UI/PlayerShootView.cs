using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Collections;

public class PlayerShootView : MonoBehaviour
{
    [Inject] private PlayerShoot playerShoot;

    [Header("For Ammo UI")]
    [SerializeField] private Image ammoFill;
    [SerializeField] private Image laserFill;

    private Coroutine fillAnimationLaserCoroutine;
    private Coroutine fillAnimationAmmoCoroutine;
    private float fillAnimationDuration = 0.5f;

    private Coroutine reloadAnimationCoroutine; 
    private Coroutine objectCooldownAnimationCoroutine;

    private void Start()
    {
        ammoFill.fillAmount = 1f;
        UpdateAmmoUI(playerShoot.maxAmmo);
        playerShoot.OnAmmoChanged += UpdateAmmoUI;
        playerShoot.OnReloadStateChanged += UpdateReloadUI;
        playerShoot.OnObjectActiveTimeChanged += UpdateObjectActiveUI;
        playerShoot.OnObjectCooldownChanged += UpdateObjectCooldownStateChangedUI;
        playerShoot.OnObjectCooldownTimeChanged += UpdateReloadObjectUI;
    }

    private void OnDestroy()
    {
        playerShoot.OnAmmoChanged -= UpdateAmmoUI;
        playerShoot.OnReloadStateChanged -= UpdateReloadUI;
        playerShoot.OnObjectActiveTimeChanged -= UpdateObjectActiveUI;
        playerShoot.OnObjectCooldownChanged -= UpdateObjectCooldownStateChangedUI;
        playerShoot.OnObjectCooldownTimeChanged -= UpdateReloadObjectUI;
    }

    private void UpdateAmmoUI(int currentAmmo)
    {
        float targetFill = (float)currentAmmo / playerShoot.maxAmmo;
        if (fillAnimationAmmoCoroutine != null)
            StopCoroutine(fillAnimationAmmoCoroutine);
        fillAnimationAmmoCoroutine = StartCoroutine(AnimateAmmoFill(targetFill));
    }

    private IEnumerator AnimateAmmoFill(float targetFill)
    {
        float startFill = ammoFill.fillAmount;
        float elapsed = 0f;
        while (elapsed < fillAnimationDuration)
        {
            elapsed += Time.deltaTime;
            ammoFill.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / fillAnimationDuration);
            yield return null;
        }
        ammoFill.fillAmount = targetFill;
    }

    private void UpdateReloadUI(bool isReloading)
    {
        if (isReloading)
        {
            if (reloadAnimationCoroutine != null)
                StopCoroutine(reloadAnimationCoroutine);
            ammoFill.color = Color.yellow;
            reloadAnimationCoroutine = StartCoroutine(AnimateReload(playerShoot.reloadTime));
        }
        else
        {
            if (reloadAnimationCoroutine != null)
            {
                StopCoroutine(reloadAnimationCoroutine);
                reloadAnimationCoroutine = null;
            }
            ammoFill.color = Color.white;
            UpdateAmmoUI(playerShoot.maxAmmo);
        }
    }

    private IEnumerator AnimateReload(float reloadDuration)
    {
        ammoFill.color = new Color(1f, 0.7f, 0f, 1f);
        float elapsed = 0f;
        while (elapsed < reloadDuration)
        {
            elapsed += Time.deltaTime;
            ammoFill.fillAmount = Mathf.Lerp(0f, 1f, elapsed / reloadDuration);
            yield return null;
        }
        ammoFill.fillAmount = 1f;
    }

    private void UpdateObjectActiveUI(float objectActiveTime)
    {
        if (objectActiveTime >= 0.1f)
        {
            float targetFill = (playerShoot.objectCooldown - (objectActiveTime * (playerShoot.objectCooldown / playerShoot.objectActiveDuration))) / playerShoot.objectCooldown;
            if (fillAnimationLaserCoroutine != null)
                StopCoroutine(fillAnimationLaserCoroutine);
            fillAnimationLaserCoroutine = StartCoroutine(AnimateObjectFill(targetFill));
        }
    }

    private IEnumerator AnimateObjectFill(float targetFill)
    {
        float startFill = laserFill.fillAmount;
        float elapsed = 0f;
        while (elapsed < fillAnimationDuration)
        {
            elapsed += Time.deltaTime;
            laserFill.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / fillAnimationDuration);
            yield return null;
        }
        laserFill.fillAmount = targetFill;
    }

    private void UpdateObjectCooldownStateChangedUI(bool isOnCooldown)
    {
        if (isOnCooldown)
        {
            laserFill.color = Color.red;
        }
        else
        {
            if (objectCooldownAnimationCoroutine != null)
            {
                StopCoroutine(objectCooldownAnimationCoroutine);
                objectCooldownAnimationCoroutine = null;
            }
            laserFill.fillAmount = 1f;
            laserFill.color = Color.green;
        }
    }

    private void UpdateReloadObjectUI(float cooldownTime)
    {
        if (fillAnimationLaserCoroutine != null)
        {
            StopCoroutine(fillAnimationLaserCoroutine);
            fillAnimationLaserCoroutine = null;
        }
        if (objectCooldownAnimationCoroutine != null)
        {
            StopCoroutine(objectCooldownAnimationCoroutine);
            objectCooldownAnimationCoroutine = null;
        }
        objectCooldownAnimationCoroutine = StartCoroutine(AnimateObjectCooldown(cooldownTime));
    }

    private IEnumerator AnimateObjectCooldown(float cooldownTime)
    {
        float startFill = laserFill.fillAmount;
        float elapsed = 0f;
        while (elapsed < cooldownTime)
        {
            elapsed += Time.deltaTime;
            laserFill.fillAmount = Mathf.Lerp(startFill, 1f, elapsed / cooldownTime);
            yield return null;
        }
        laserFill.fillAmount = 1f;
    }
}
