using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 5f;
    private float currentHealth;
    private PlayerRespawn playerRespawn;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        playerRespawn = GetComponent<PlayerRespawn>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Jugador recibió daño. Salud actual: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
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

    // Nuevo: usado cuando respawnea
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}
