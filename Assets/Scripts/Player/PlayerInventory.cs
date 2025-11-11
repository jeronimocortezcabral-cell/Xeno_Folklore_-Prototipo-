using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI healingItemsText;
    [SerializeField] private TextMeshProUGUI keysText; // muestra cuántas llaves/partes tenés

    [Header("Curación")]
    [SerializeField] private int healingItems = 0;
    [SerializeField] private int maxHealingItems = 9;
    [SerializeField] private float healAmountPerItem = 1f;
    [SerializeField] private float useCooldown = 0.5f; // evita spam al usar curas
    private float nextUseTime = 0f;

    [Header("Keys / Ship Parts")]
    [SerializeField] private int keys = 0;
    [SerializeField] private int maxKeys = 99;

    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        UpdateUI();
    }

    private void Update()
    {
        // Usar una cura al PRESIONAR la rueda del mouse (botón medio)
        if (Mouse.current != null && Mouse.current.middleButton.wasPressedThisFrame && Time.time >= nextUseTime)
        {
            TryUseHealingItem();
        }
    }

    // ----------------------
    // Curación
    // ----------------------
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

        // cooldown para evitar spam
        nextUseTime = Time.time + useCooldown;
        Debug.Log($"Usó una cura. Curas restantes: {healingItems}");
    }

    // ----------------------
    // Keys / Ship Parts
    // ----------------------
    public void AddKey(int amount)
    {
        keys += amount;
        keys = Mathf.Clamp(keys, 0, maxKeys);
        UpdateUI();
        Debug.Log($"Parts added: {amount}. Total parts: {keys}");
    }

    public bool HasEnoughKeys(int amount)
    {
        return keys >= amount;
    }

    public void UseKeys(int amount)
    {
        if (keys >= amount)
        {
            keys -= amount;
            UpdateUI();
        }
    }

    // ----------------------
    // UI
    // ----------------------
    private void UpdateUI()
    {
        if (healingItemsText != null)
            healingItemsText.text = healingItems.ToString();

        if (keysText != null)
            keysText.text = keys.ToString();
    }
}
