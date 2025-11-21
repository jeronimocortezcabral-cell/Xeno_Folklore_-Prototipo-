using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float damage = 1f;
    public float lifetime = 4f;

    private void Start()
    {
        // Destruir el proyectil automáticamente después de un tiempo si no choca con nada
        Destroy(gameObject, lifetime);
    }

    // ------------------------------------------------------------------
    // ACTUALIZADO: Usamos OnCollisionEnter2D (Igual que el Wisp)
    // ------------------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Ignorar al propio Boss para que no se suicide con su bala
        if (collision.gameObject.CompareTag("Boss")) return;

        // 2. Lógica si golpea al Player
        if (collision.collider.CompareTag("Player"))
        {
            // Buscamos la vida del player igual que en el script del Wisp
            PlayerHealth ph = collision.collider.GetComponent<PlayerHealth>();

            if (ph != null)
            {
                ph.TakeDamage(damage);
            }

            // Destruimos el proyectil al impactar
            Destroy(gameObject);
            return;
        }

        // 3. Si golpea con cualquier otra cosa (Paredes, Obstáculos)
        // que NO sea el Player ni el Boss, se destruye.
        Destroy(gameObject);
    }
}