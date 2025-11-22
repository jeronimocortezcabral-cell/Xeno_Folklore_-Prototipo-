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

    // --- NUEVA SECCIÓN DE AUDIO ---
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [Tooltip("Sonido al recolectar Piezas (Keys)")]
    [SerializeField] private AudioClip keyCollectSound;
    [Tooltip("Sonido al recolectar Ítems de Curación")]
    [SerializeField] private AudioClip healingCollectSound;
    // -------------------------------

    private PlayerHealth playerHealth;

    private void Start()
    {
        // Si el AudioSource no está asignado, lo buscamos en el mismo objeto.
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

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

    // ------------------------------------
    // LÓGICA DE COLECCIÓN (CON SONIDO)
    // ------------------------------------

    public void AddHealingItem(int amount)
    {
        healingItems += amount;
        healingItems = Mathf.Clamp(healingItems, 0, maxHealingItems);

        PlayCollectSound(healingCollectSound); // Reproducir sonido de Curación

        UpdateUI();
    }

    public void AddKey(int amount)
    {
        keys += amount;
        keys = Mathf.Clamp(keys, 0, maxKeys);

        PlayCollectSound(keyCollectSound); // Reproducir sonido de Pieza/Key

        UpdateUI();
        Debug.Log($"Parts added: {amount}. Total parts: {keys}");
    }

    // ------------------------------------
    // FUNCIÓN DE AUDIO
    // ------------------------------------
    private void PlayCollectSound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // ------------------------------------
    // RESTO DE FUNCIONES (SIN CAMBIOS)
    // ------------------------------------

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