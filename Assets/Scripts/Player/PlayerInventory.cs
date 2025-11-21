using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI healingItemsText;
    [SerializeField] private TextMeshProUGUI keysText;

    [Header("Curación")]
    [SerializeField] private int healingItems = 0;
    [SerializeField] private int maxHealingItems = 9;
    [SerializeField] private float healAmountPerItem = 1f;
    [SerializeField] private float useCooldown = 0.5f;
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

        healingItems--;
        playerHealth.Heal(healAmountPerItem);
        UpdateUI();

        nextUseTime = Time.time + useCooldown;
        Debug.Log($"Usó una cura. Curas restantes: {healingItems}");
    }

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

    private void UpdateUI()
    {
        if (healingItemsText != null)
            healingItemsText.text = healingItems.ToString();

        if (keysText != null)
            keysText.text = keys.ToString();
    }
    public int GetTotalKeys()
    {
        return keys;
    }
}
