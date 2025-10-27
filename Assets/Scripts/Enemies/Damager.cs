using UnityEngine;

public class Damager : MonoBehaviour
{
    [SerializeField] private float damage = 1f; // Daño configurable desde el inspector

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica si el objeto con el que colisiona tiene el componente PlayerHealth
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            // Aplica el daño usando el método actual del PlayerHealth
            playerHealth.TakeDamage(damage);
        }
    }
}
