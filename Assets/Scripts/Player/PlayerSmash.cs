using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSmash : MonoBehaviour
{
    [Header("Ataque")]
    [SerializeField] private Transform hitPoint;         // Punto donde se origina el ataque
    [SerializeField] private float attackRadius = 0.6f;  // Radio del área de golpe
    [SerializeField] private float damage = 1f;          // Daño infligido
    [SerializeField] private LayerMask enemyMask;        // Capas consideradas enemigos

    [Header("Cámara")]
    [SerializeField] private Camera mainCamera;          // Cámara principal (para calcular dirección del mouse)

    [Header("Cooldown")]
    [SerializeField] private float attackCooldown = 0.5f;
    private float nextAttackTime = 0f;

    [Header("Knockback (opcional)")]
    [SerializeField] private bool useKnockback = false;
    [SerializeField] private float knockbackForce = 5f;

    private void Start()
    {
        // Si no se asignó manualmente la cámara, se usa la principal
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        // Clic izquierdo para atacar con cooldown
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextAttackTime)
        {
            AttackTowardMouse();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void AttackTowardMouse()
    {
        if (mainCamera == null)
        {
            Debug.LogWarning("MainCamera no asignada en PlayerSmash.");
            return;
        }

        // Obtener posición del mouse en el mundo
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        // Calcular dirección del jugador al mouse
        Vector2 direction = (worldPos - transform.position).normalized;

        // Colocar el punto de impacto frente al jugador (hacia el mouse)
        hitPoint.position = transform.position + (Vector3)(direction * 0.6f);

        // Detectar enemigos en el área del golpe
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint.position, attackRadius, enemyMask);

        foreach (var col in hits)
        {
            var enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                if (useKnockback)
                {
                    var rb = col.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        Vector2 knockDir = (col.transform.position - transform.position).normalized;
                        rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (hitPoint == null) return;
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.6f);
        Gizmos.DrawWireSphere(hitPoint.position, attackRadius);
    }
}