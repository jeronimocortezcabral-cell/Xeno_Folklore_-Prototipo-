using UnityEngine;
using System.Collections;

public class WerewolfHealth : MonoBehaviour
{
    [Header("Estadísticas")]
    [SerializeField] private float currentHealth = 10f;
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float flashDuration = 0.1f;
    private bool isDead = false;

    [Header("Recompensa y Muerte")]
    [Tooltip("El Prefab que debe aparecer al morir (ej: una pieza de nave, llave).")]
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private int rewardAmount = 1;

    // --- NUEVAS VARIABLES DE AUDIO ---
    [Header("Audio")]
    [SerializeField] private AudioClip damageSound;
    private AudioSource audioSource;
    // ----------------------------------

    // Referencias a otros componentes (para efectos visuales y lógica de comportamiento)
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private WerewolfBehavior behavior;
    private Color originalColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        behavior = GetComponent<WerewolfBehavior>();
        audioSource = GetComponent<AudioSource>(); // Obtener el AudioSource

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        if (audioSource == null)
        {
            Debug.LogWarning("WerewolfHealth: No se encontró AudioSource. Los sonidos de daño no funcionarán.");
        }
    }

    // ----------------------------------------------------------------------
    // 1. MÉTODO DE DAÑO (BUSCADO POR LOS SCRIPTS DEL JUGADOR)
    // ----------------------------------------------------------------------

    /// <summary>
    /// Reduce la vida del lobizón. Este método es llamado por el PlayerSmash y por las balas.
    /// </summary>
    /// <param name="damage">Cantidad de daño recibido.</param>
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} recibió {damage} de daño. Vida restante: {currentHealth}");

        // Efecto visual de haber sido golpeado
        StartCoroutine(DamageFlash());

        // --- REPRODUCIR SONIDO DE DAÑO ---
        PlayDamageSound();
        // ----------------------------------

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // --- NUEVA FUNCIÓN PARA REPRODUCIR SONIDO ---
    private void PlayDamageSound()
    {
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
    }
    // --------------------------------------------

    // ----------------------------------------------------------------------
    // 2. LÓGICA DE MUERTE
    // ----------------------------------------------------------------------

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // **********************************************
        // INTEGRACIÓN DE AUDIO: NOTIFICAR MUERTE AL MANAGER
        // **********************************************
        if (MusicManager.instance != null)
        {
            MusicManager.instance.DefeatedBoss();
            Debug.Log("WerewolfHealth: Notificando derrota al MusicManager.");
        }
        // **********************************************

        // Desactivar el comportamiento y el collider para que deje de atacar
        if (behavior != null)
        {
            behavior.enabled = false;
        }
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static; // Detener completamente
        }

        // Desactivar los colliders para que el jugador no choque con el cadáver
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        DropReward();

        // Animación de muerte y destrucción (desaparecer)
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Opcional: Podrías añadir animaciones de muerte aquí (anim.SetTrigger("Die"))

        // Espera un breve momento antes de desaparecer
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

    // ----------------------------------------------------------------------
    // 3. RECOMPENSA
    // ----------------------------------------------------------------------

    private void DropReward()
    {
        if (rewardPrefab == null)
        {
            Debug.LogWarning("No se asignó un prefab de recompensa en WerewolfHealth.");
            return;
        }

        // Instancia la recompensa en la posición del lobizón
        for (int i = 0; i < rewardAmount; i++)
        {
            Vector3 spawnPosition = transform.position + (Vector3)Random.insideUnitCircle * 0.5f;
            Instantiate(rewardPrefab, spawnPosition, Quaternion.identity);
        }
    }

    // ----------------------------------------------------------------------
    // 4. EFECTOS VISUALES
    // ----------------------------------------------------------------------

    private IEnumerator DamageFlash()
    {
        if (spriteRenderer != null)
        {
            // Cambia el color a rojo/blanco para indicar daño
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            // Vuelve al color original
            spriteRenderer.color = originalColor;
        }
    }
}