using UnityEngine;

public class WispBehavior : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Knockback recibido")]
    [SerializeField] private float knockbackForce = 4f;
    [SerializeField] private float knockbackDuration = 0.15f;

    [Header("Daño al jugador")]
    [SerializeField] private float damageAmount = 1f;
    [SerializeField] private float knockbackToPlayer = 4f;

    private Transform player;
    private Rigidbody2D rb;
    private bool isKnockback = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = false; // asegurado

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void FixedUpdate()
    {
        if (player == null) return;
        if (isKnockback) return;

        // movimiento normal persiguiendo al jugador
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }

    // -----------------------------------------
    // KNOCKBACK RECIBIDO POR DISPARO O ATAQUE
    // -----------------------------------------
    public void TakeDamage(float dmg, Vector3 hitSource)
    {
        StartCoroutine(DoKnockback(hitSource));
    }

    private System.Collections.IEnumerator DoKnockback(Vector3 hitSource)
    {
        isKnockback = true;

        Vector2 dir = (transform.position - hitSource).normalized;

        rb.linearVelocity = dir * knockbackForce;

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector2.zero;
        isKnockback = false;
    }

    // -----------------------------------------
    // DAÑO AL PLAYER + KNOCKBACK AL PLAYER
    // -----------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerHealth ph = collision.collider.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damageAmount);
            }

            Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 dir = (collision.collider.transform.position - transform.position).normalized;
                playerRb.linearVelocity = dir * knockbackToPlayer;
            }
        }
    }
}
