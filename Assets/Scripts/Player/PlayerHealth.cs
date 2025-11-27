using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float maxHealth = 5f;

    [Header("Efectos de Daño")]
    [SerializeField] private float damageFlashDuration = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private float currentHealth;
    private PlayerRespawn playerRespawn;
    private bool isDead = false;
    private bool isFlashing = false;

    private bool isInvulnerable = false;
    public bool IsInvulnerable => isInvulnerable;

    public void StartMeleeIFrames() => isInvulnerable = true;
    public void EndMeleeIFrames() => isInvulnerable = false;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
        else
            Debug.LogWarning("PlayerHealth: No se encontró SpriteRenderer.");

        currentHealth = maxHealth;
        playerRespawn = GetComponent<PlayerRespawn>();
        isDead = false;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        if (isInvulnerable) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Jugador recibió {amount} daño. Salud: {currentHealth}");

        if (!isFlashing && spriteRenderer != null)
            StartCoroutine(DamageFlashRoutine());

        if (currentHealth <= 0)
            Die();
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
        Debug.Log("Jugador murió");

        if (playerRespawn != null)
            playerRespawn.Respawn();
        else
            Debug.LogWarning("PlayerRespawn no encontrado.");
    }

    public void ResetHealth()
    {
        isDead = false;
        currentHealth = maxHealth;
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
    }

    private IEnumerator DamageFlashRoutine()
    {
        isFlashing = true;

        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(damageFlashDuration);
        spriteRenderer.color = originalColor;

        isFlashing = false;
    }
}
