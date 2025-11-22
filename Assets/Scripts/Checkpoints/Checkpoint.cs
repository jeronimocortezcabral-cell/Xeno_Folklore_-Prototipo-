using UnityEngine;
using System.Collections; // Necesario para Coroutines

public class Checkpoint : MonoBehaviour
{
    [Header("Efectos")]
    [SerializeField] private AudioClip activationSound;
    [SerializeField] private float flashDuration = 0.2f;

    // Referencias internas
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool activated = false;
    private Color originalColor;

    private void Awake()
    {
        // Obtener componentes
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        else
        {
            Debug.LogWarning("Checkpoint: Falta SpriteRenderer.");
        }
        if (audioSource == null)
        {
            Debug.LogWarning("Checkpoint: Falta AudioSource.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !activated)
        {
            // 1. Marcar como activado
            activated = true;
            Debug.Log("Checkpoint alcanzado en: " + transform.position);

            // 2. Notificar al sistema de respawn del jugador
            PlayerRespawn playerRespawn = collision.GetComponent<PlayerRespawn>();
            if (playerRespawn != null)
            {
                playerRespawn.ReachedCheckPoint(transform.position);
            }

            // 3. Ejecutar efectos
            StartCoroutine(FlashAndSound());
        }
    }

    private IEnumerator FlashAndSound()
    {
        // A. Sonido de activación
        if (audioSource != null && activationSound != null)
        {
            audioSource.PlayOneShot(activationSound);
        }

        // B. Efecto de parpadeo (Amarillo)
        if (spriteRenderer != null)
        {
            // Cambiar a amarillo brillante
            spriteRenderer.color = Color.yellow;

            // Esperar la duración del flash
            yield return new WaitForSeconds(flashDuration);

            // Devolver al color original
            spriteRenderer.color = originalColor;
        }
    }
}