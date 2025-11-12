using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceShip : MonoBehaviour
{
    [Header("Configuración de salida")]
    [SerializeField] private int requiredParts = 4;
    [SerializeField] private bool consumePartsOnUse = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.GetComponent<PlayerInventory>();

            if (inventory == null)
            {
                Debug.LogWarning("No se encontró PlayerInventory en el jugador.");
                return;
            }

            if (inventory.HasEnoughKeys(requiredParts))
            {
                Debug.Log("Jugador tiene suficientes partes. Iniciando secuencia de salida...");

                if (consumePartsOnUse)
                    inventory.UseKeys(requiredParts);

                SceneManager.LoadScene("EndScene");
            }
            else
            {
                Debug.Log($"Faltan partes. Necesitas {requiredParts} para acceder a la nave.");
            }
        }
    }
}
