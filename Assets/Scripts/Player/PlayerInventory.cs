using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int healingItems;
    public TextMeshProUGUI healingItemsText;
    public float healAmountPerItem = 1f;
    private PlayerHealth playerHealth;
    private bool canUseHeal = true;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        UpdateUI();
    }

    public void AddHealingItem(float amount)
    {
        healingItems++;
        UpdateUI();
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0 && healingItems > 0 && canUseHeal)
        {
            UseHealingItem();
        }
    }

    private void UseHealingItem()
    {
        healingItems--;
        playerHealth.Heal(healAmountPerItem);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healingItemsText != null)
            healingItemsText.text = healingItems.ToString();
    }
}
