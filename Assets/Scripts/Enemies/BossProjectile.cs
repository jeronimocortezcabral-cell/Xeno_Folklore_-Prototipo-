using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float damage = 1f;
    public float lifetime = 4f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Evitar que el proyectil golpee al boss o sus hijos
        if (other.CompareTag("Boss")) return;

        // Golpea al Player
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph == null)
                ph = other.GetComponentInParent<PlayerHealth>();

            if (ph != null)
                ph.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }

        // Golpea pared / mundo
        if (other.gameObject.layer == LayerMask.NameToLayer("Solid"))
        {
            Destroy(gameObject);
        }
    }
}
