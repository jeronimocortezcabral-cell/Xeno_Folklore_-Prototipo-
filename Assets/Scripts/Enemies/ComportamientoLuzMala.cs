using UnityEngine;

public class WispBehavior : MonoBehaviour
{
    [Header("Movimiento aleatorio")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float changeDirectionInterval = 2f;

    [Header("Daño al jugador")]
    [SerializeField] private float damageAmount = 1f;
    [SerializeField] private float knockbackForce = 2f;

    private Vector2 movementDirection;
    private float timer;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ChooseNewDirection();
    }

    private void Update()
    {
        // Cambia de dirección cada cierto tiempo
        timer += Time.deltaTime;
        if (timer >= changeDirectionInterval)
        {
            ChooseNewDirection();
            timer = 0f;
        }

        // Mueve el fuego fatuo suavemente
        rb.MovePosition(rb.position + movementDirection * moveSpeed * Time.deltaTime);
    }

    private void ChooseNewDirection()
    {
        // Dirección aleatoria 2D normalizada
        movementDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Buscar componente de salud del jugador
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }

            // Opcional: aplicar pequeño empuje (knockback)
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}