using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PlayerHealthView : MonoBehaviour
{
    [Header("For HealthUI")]
    [SerializeField] private Image Health1UI;
    [SerializeField] private Image Health2UI;
    [SerializeField] private Image Health3UI;
    [Header("For PartsUI")]
    [SerializeField] private Image CoreUI;
    [SerializeField] private Image Engine1UI;
    [SerializeField] private Image Engine2UI;
    [SerializeField] private Image WeaponUI;

    [Inject] private PlayerHealth playerHealth;

    private void Start()
    {
        UpdateHealthUI(playerHealth.CurrentHealth);
        playerHealth.OnHealthChanged += UpdateHealthUI;
        playerHealth.OnWeaponDestroyed += UpdateWeaponUI;
        playerHealth.OnEngine1Destroyed += UpdateEngine1UI;
        playerHealth.OnEngine2Destroyed += UpdateEngine2UI;
    }

    private void OnDestroy()
    {
        playerHealth.OnHealthChanged -= UpdateHealthUI;
        playerHealth.OnWeaponDestroyed -= UpdateWeaponUI;
        playerHealth.OnEngine1Destroyed -= UpdateEngine1UI;
        playerHealth.OnEngine2Destroyed -= UpdateEngine2UI;
    }

    private void UpdateHealthUI(int health)
    {
        Health3UI.enabled = health >= 3;
        Health2UI.enabled = health >= 2;
        Health1UI.enabled = health >= 1;

        CoreUI.color = health == 3 ? Color.green : health > 0 ? Color.yellow : Color.red;
    }

    private void UpdateEngine1UI(bool destroyed) => Engine1UI.color = destroyed ? Color.red : Color.green;
    private void UpdateEngine2UI(bool destroyed) => Engine2UI.color = destroyed ? Color.red : Color.green;
    private void UpdateWeaponUI(bool destroyed) => WeaponUI.color = destroyed ? Color.red : Color.green;
}
