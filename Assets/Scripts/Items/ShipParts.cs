using UnityEngine;

public class ShipParts : MonoBehaviour
{
    [SerializeField] private int amount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerInventory inventory = collision.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddKey(amount); // Usa el sistema de llaves existente
            Destroy(gameObject);
        }
    }
}
