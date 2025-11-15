using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSmash : MonoBehaviour
{
    [Header("Ataque")]
    [Tooltip("Si dejás vacío, el hit se hará desde la posición del jugador.")]
    [SerializeField] private Transform hitPoint;
    [SerializeField] private float attackRadius = 0.6f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private LayerMask enemyMask;

    [Header("Cámara")]
    [SerializeField] private Camera mainCamera;

    [Header("Cooldown y timing")]
    [SerializeField] private float attackCooldown = 0.5f;
    [Tooltip("Si no usás Animation Event, el hit se hará después de este delay (segundos).")]
    [SerializeField] private float attackDelay = 0.12f;
    [SerializeField] private float smashDuration = 0.5f;
    private float nextAttackTime = 0f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 5f;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    private const string PARAM_SMASH = "Smash";
    private const string PARAM_IS_ATTACKING = "IsAttacking";

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;      // Debe estar en el Player
    [SerializeField] private AudioClip smashSound;         // Sonido del golpe

    private void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (animator == null) animator = GetComponent<Animator>();
        if (hitPoint == null)
            Debug.LogWarning("No se asignó un hitPoint. Se usará la posición del jugador.");

        if (audioSource == null)
            Debug.LogWarning("PlayerSmash: No se asignó un AudioSource.");
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextAttackTime)
        {
            StartSmash();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void StartSmash()
    {
        if (animator != null)
        {
            animator.SetBool(PARAM_IS_ATTACKING, true);
            animator.SetTrigger(PARAM_SMASH);
        }

        // 🔊 Reproducir sonido del ataque
        PlaySmashSound();

        // Manejo del hit
        if (attackDelay <= 0f)
        {
            OnAttackHit();
            StartCoroutine(ResetAttackingAfter(smashDuration));
        }
        else
        {
            Invoke(nameof(OnAttackHit), attackDelay);
            StartCoroutine(ResetAttackingAfter(smashDuration));
        }
    }

    private void PlaySmashSound()
    {
        if (audioSource != null && smashSound != null)
        {
            audioSource.PlayOneShot(smashSound);
        }
    }

    public void OnAttackHit()
    {
        Vector2 center = hitPoint != null ? (Vector2)hitPoint.position : (Vector2)transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackRadius, enemyMask);

        foreach (var col in hits)
        {
            // Enemigos normales
            var enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                ApplyKnockback(col);
                continue;
            }

            // Boss
            var boss = col.GetComponent<BossHealth>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                ApplyKnockback(col);
                continue;
            }
        }
    }

    private void ApplyKnockback(Collider2D col)
    {
        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 knockDir = ((Vector2)col.transform.position - (Vector2)transform.position).normalized;
            rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);
        }
    }

    private IEnumerator ResetAttackingAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (animator != null) animator.SetBool(PARAM_IS_ATTACKING, false);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = hitPoint != null ? hitPoint.position : transform.position;
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.6f);
        Gizmos.DrawWireSphere(center, attackRadius);
    }
}
