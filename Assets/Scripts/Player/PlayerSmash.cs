using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSmash : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private Transform hitPoint;         // Punto donde se origina el ataque melee
    [SerializeField] private float attackRadius = 0.6f;  // Radio de impacto del golpe
    [SerializeField] private float damage = 1f;          // Daño infligido al enemigo
    [SerializeField] private LayerMask enemyMask;        // Capas que se consideran enemigos

    [Header("Timing")]
    [SerializeField] private float attackCooldown = 0.5f; // Tiempo entre ataques
    private float nextAttackTime = 0f;

    [Header("Optional")]
    [SerializeField] private Animator animator;          // (Opcional) para animación de ataque
    [SerializeField] private string animatorTriggerName = "Smash"; // Nombre del trigger en el Animator
    [SerializeField] private bool useKnockback = false;  // Aplicar empuje (knockback) a los enemigos golpeados
    [SerializeField] private float knockbackForce = 5f;  // Intensidad del empuje

    private void Reset()
    {
        // Si no hay un hitPoint asignado, se crea uno automáticamente como hijo del jugador
        if (hitPoint == null)
        {
            var hp = new GameObject("HitPoint");
            hp.transform.SetParent(transform);
            hp.transform.localPosition = Vector3.right * 0.6f;
            hitPoint = hp.transform;
        }
    }

    private void Update()
    {
        // Clic izquierdo del ratón -> ataque melee
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void Attack()
    {
        // Reproduce la animación si hay un Animator asignado
        if (animator != null && !string.IsNullOrEmpty(animatorTriggerName))
        {
            animator.SetTrigger(animatorTriggerName);
        }

        // Detecta colisionadores dentro del radio de ataque que estén en la capa enemyMask
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint.position, attackRadius, enemyMask);

        foreach (var col in hits)
        {
            if (col == null) continue;

            // Busca el componente Enemy y aplica daño si lo encuentra
            var enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                // Aplica empuje (knockback) si está activado
                if (useKnockback)
                {
                    var rb = col.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        Vector2 knockDir = (col.transform.position - hitPoint.position).normalized;
                        rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    // Dibuja una esfera de color en el editor para visualizar el rango del ataque
    private void OnDrawGizmosSelected()
    {
        if (hitPoint == null) return;
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.6f);
        Gizmos.DrawWireSphere(hitPoint.position, attackRadius);
    }
}