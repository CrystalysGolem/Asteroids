using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PlayerHealthView : MonoBehaviour
{
    [Header("For HealthUI")]
    [SerializeField] private Image H1;
    [SerializeField] private Image H2;
    [SerializeField] private Image H3;
    [Header("For PartsUI")]
    [SerializeField] private Image CO;
    [SerializeField] private Image ENG1;
    [SerializeField] private Image ENG2;
    [SerializeField] private Image WPN;

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
        H3.enabled = health >= 3;
        H2.enabled = health >= 2;
        H1.enabled = health >= 1;

        CO.color = health == 3 ? Color.green : health > 0 ? Color.yellow : Color.red;
    }

    private void UpdateEngine1UI(bool destroyed) => ENG1.color = destroyed ? Color.red : Color.green;
    private void UpdateEngine2UI(bool destroyed) => ENG2.color = destroyed ? Color.red : Color.green;
    private void UpdateWeaponUI(bool destroyed) => WPN.color = destroyed ? Color.red : Color.green;
}
