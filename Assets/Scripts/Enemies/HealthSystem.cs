using UnityEngine;
using System;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealthPhase1 = 10f;
    public float maxHealthPhase2 = 15f;

    private float currentHealth;
    private bool isPhase2 = false;
    private bool isDead = false;

    [Header("Feedback de daño")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color flashColor = new Color(1f, 0f, 1f, 1f); // Fucsia
    [SerializeField] private float flashDuration = 0.12f;
    private Color originalColor;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;

    // Evento que el BossController escucha
    public event Action OnDeath;

    private void Awake()
    {
        currentHealth = maxHealthPhase1;

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    // =====================================================
    //                  RECIBIR DAÑO
    // =====================================================
    public void TakeDamage(float dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;

        PlayHitSound();
        StartCoroutine(Flash());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            HandleDeath();
        }
    }

    // =====================================================
    //                  SONIDO DE DAÑO
    // =====================================================
    private void PlayHitSound()
    {
        if (audioSource != null && hitSound != null)
            audioSource.PlayOneShot(hitSound);
    }

    // =====================================================
    //                  FLASH Fucsia
    // =====================================================
    private IEnumerator Flash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    // =====================================================
    //                  MUERTE / FASE 2
    // =====================================================
    private void HandleDeath()
    {
        if (!isPhase2)
        {
            // Notifica muerte de fase 1 ? bossController convierte a fase 2
            OnDeath?.Invoke();
        }
        else
        {
            // Muerte final
            isDead = true;
            OnDeath?.Invoke();
        }
    }

    // =====================================================
    //                  ACTIVAR FASE 2
    // =====================================================
    public void ActivatePhase2()
    {
        isPhase2 = true;
        currentHealth = maxHealthPhase2;
    }
}
