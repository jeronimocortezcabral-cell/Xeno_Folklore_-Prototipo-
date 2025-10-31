using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSmash : MonoBehaviour {
    [Header("Ataque")]
    [Tooltip("Si dej�s vac�o, el hit se har� desde la posici�n del jugador.")]
    [SerializeField] private Transform hitPoint;         // opcional: child vac�o; si es null se usa transform.position
    [SerializeField] private float attackRadius = 0.6f;  // Radio del �rea de golpe
    [SerializeField] private float damage = 1f;          // Da�o infligido
    [SerializeField] private LayerMask enemyMask;        // Capas consideradas enemigos

    [Header("C�mara")]
    [SerializeField] private Camera mainCamera;          // solo si necesit�s mouse (no es necesario para 360�)

    [Header("Cooldown y timing")]
    [SerializeField] private float attackCooldown = 0.5f;
    [Tooltip("Si no us�s Animation Event, el hit se har� despu�s de este delay (segundos).")]
    [SerializeField] private float attackDelay = 0.12f;
    [SerializeField] private float smashDuration = 0.5f; // tiempo total de la animaci�n (fallback)
    private float nextAttackTime = 0f;

    [Header("Knockback (opcional)")]
    [SerializeField] private bool useKnockback = false;
    [SerializeField] private float knockbackForce = 5f;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    private const string PARAM_SMASH = "Smash";         // trigger
    private const string PARAM_IS_ATTACKING = "IsAttacking"; // bool (opcional para bloquear inputs)

    private void Start() {
        if (mainCamera == null) mainCamera = Camera.main;
        if (animator == null) animator = GetComponent<Animator>();
        if (hitPoint == null) {
            // no es obligatorio, pero podemos crear un punto visual en tiempo de edici�n si quer�s
        }
    }

    private void Update() {
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextAttackTime) {
            StartSmash();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void StartSmash() {
        if (animator != null) {
            animator.SetBool(PARAM_IS_ATTACKING, true);
            animator.SetTrigger(PARAM_SMASH);
        }

        // Si us�s Animation Events en el clip, no necesit�s el Invoke; el evento debe llamar a OnAttackHit().
        // Fallback: si no us�s Animation Event, ejecutamos el hit tras attackDelay y reset luego de smashDuration.
        if (attackDelay <= 0f) {
            OnAttackHit();
            StartCoroutine(ResetAttackingAfter(smashDuration));
        }
        else {
            Invoke(nameof(OnAttackHit), attackDelay);
            StartCoroutine(ResetAttackingAfter(smashDuration));
        }
    }

    // M�todo p�blico que puede llamarse desde Animation Event exactamente cuando quer�s que aplique el hit
    public void OnAttackHit() {
        Vector2 center = hitPoint != null ? (Vector2)hitPoint.position : (Vector2)transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackRadius, enemyMask);

        foreach (var col in hits) {
            var enemy = col.GetComponent<Enemy>();
            if (enemy != null) {
                enemy.TakeDamage(damage);

                if (useKnockback) {
                    var rb = col.GetComponent<Rigidbody2D>();
                    if (rb != null) {
                        Vector2 knockDir = (col.transform.position - transform.position).normalized;
                        rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    private IEnumerator ResetAttackingAfter(float seconds) {
        yield return new WaitForSeconds(seconds);
        if (animator != null) animator.SetBool(PARAM_IS_ATTACKING, false);
    }

    private void OnDrawGizmosSelected() {
        Vector3 center = hitPoint != null ? hitPoint.position : transform.position;
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.6f);
        Gizmos.DrawWireSphere(center, attackRadius);
    }
}