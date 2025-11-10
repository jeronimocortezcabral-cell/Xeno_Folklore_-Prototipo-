using UnityEngine;

public class HealingItem : MonoBehaviour
{
    [SerializeField] private float healAmount = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerInventory playerInventory = collision.GetComponent<PlayerInventory>();
        if (playerInventory != null)
        {
            playerInventory.AddHealingItem(healAmount);
            Destroy(gameObject);
        }
    }
}
