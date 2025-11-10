using UnityEngine;

public class HealingItem : MonoBehaviour
{
    [SerializeField] private int amount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerInventory inventory = collision.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddHealingItem(amount);
            Destroy(gameObject);
        }
    }
}
