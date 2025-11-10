using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI healingItemsText;

    [Header("Curación")]
    [SerializeField] private int healingItems = 0;
    [SerializeField] private int maxHealingItems = 9;
    [SerializeField] private float healAmountPerItem = 1f;
    [SerializeField] private float useCooldown = 0.5f; // opcional, evita spam

    private PlayerHealth playerHealth;
    private float nextUseTime = 0f;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        UpdateUI();
    }

    private void Update()
    {
        // Usa una cura al PRESIONAR la rueda del mouse (botón medio)
        if (Mouse.current != null && Mouse.current.middleButton.wasPressedThisFrame && Time.time >= nextUseTime)
        {
            TryUseHealingItem();
        }
    }

    public void AddHealingItem(int amount)
    {
        healingItems += amount;
        healingItems = Mathf.Clamp(healingItems, 0, maxHealingItems);
        UpdateUI();
    }

    private void TryUseHealingItem()
    {
        if (healingItems <= 0)
        {
            Debug.Log("No hay curas disponibles.");
            return;
        }

        if (playerHealth == null)
        {
            Debug.LogWarning("PlayerHealth no encontrado en el jugador.");
            return;
        }

        // Usar la cura
        healingItems--;
        playerHealth.Heal(healAmountPerItem);
        UpdateUI();

        // cooldown
        nextUseTime = Time.time + useCooldown;
        Debug.Log($"Usó una cura. Curas restantes: {healingItems}");
    }

    private void UpdateUI()
    {
        if (healingItemsText != null)
            healingItemsText.text = healingItems.ToString();
    }
}
