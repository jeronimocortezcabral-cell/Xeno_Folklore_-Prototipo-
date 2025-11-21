using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float maxHealth = 5f;

    private float currentHealth;
    private PlayerRespawn playerRespawn;
    private bool isDead = false; // Para evitar llamadas dobles a Die()

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        playerRespawn = GetComponent<PlayerRespawn>();
        isDead = false;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return; // Si ya está muerto, ignorar daño extra

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Jugador recibió {amount} de daño. Salud actual: {currentHealth}");

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
    }
}