using UnityEngine;
using System.Collections; // Necesario para Coroutines

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float maxHealth = 5f;

    // --- NUEVAS VARIABLES PARA EFECTO DE DAÑO ---
    [Header("Efectos de Daño")]
    [SerializeField] private float damageFlashDuration = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    // ----------------------------------------------

    private float currentHealth;
    private PlayerRespawn playerRespawn;
    private bool isDead = false;
    private bool isFlashing = false; // Bandera para evitar que se pisen los flashes

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    private void Start()
    {
        // Obtener componentes al inicio
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        else
        {
            Debug.LogWarning("PlayerHealth: No se encontró SpriteRenderer. El parpadeo no funcionará.");
        }

        currentHealth = maxHealth;
        playerRespawn = GetComponent<PlayerRespawn>();
        isDead = false;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Jugador recibió {amount} de daño. Salud actual: {currentHealth}");

        // --- LLAMADA AL EFECTO DE DAÑO ---
        if (!isFlashing && spriteRenderer != null)
        {
            StartCoroutine(DamageFlashRoutine());
        }
        // ----------------------------------

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Jugador se curó. Salud actual: {currentHealth}");
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Jugador ha muerto");

        if (playerRespawn != null)
        {
            playerRespawn.Respawn();
        }
        else
        {
            Debug.LogWarning("No se encontró PlayerRespawn en el jugador.");
        }
    }

    public void ResetHealth()
    {
        isDead = false;
        currentHealth = maxHealth;
        // Restaurar el color original al revivir
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
    }

    // ----------------------------------------------------
    // FUNCIÓN DE PARPADEO (FLASH)
    // ----------------------------------------------------

    private IEnumerator DamageFlashRoutine()
    {
        isFlashing = true;

        // 1. Cambiar a color rojo
        spriteRenderer.color = Color.red;

        // 2. Esperar un momento
        yield return new WaitForSeconds(damageFlashDuration);

        // 3. Volver al color original
        spriteRenderer.color = originalColor;

        isFlashing = false;
    }
}