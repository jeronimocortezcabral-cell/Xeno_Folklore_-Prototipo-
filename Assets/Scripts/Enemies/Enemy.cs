using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float health = 3f;
    private bool isFlashing = false;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color flashColor = new Color(1f, 0f, 1f, 1f); // fucsia
    private Color originalColor;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;       // sonido al recibir daño
    [SerializeField] private AudioClip deathSound;     // sonido al morir

    [Header("VFX")]
    [SerializeField] private GameObject deathEffect;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        PlayHitSound();
        StartCoroutine(FlashDamage());

        if (health <= 0)
        {
            Die();
        }
    }

    // -------------------------------------------
    //     FLASH FUCISA (FEEDBACK DE DAÑO)
    // -------------------------------------------
    private IEnumerator FlashDamage()
    {
        if (spriteRenderer == null) yield break;

        if (isFlashing) yield break;
        isFlashing = true;

        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;

        isFlashing = false;
    }

    // -------------------------------------------
    //     SONIDO AL RECIBIR DAÑO
    // -------------------------------------------
    private void PlayHitSound()
    {
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    // -------------------------------------------
    //     MUERTE DEL ENEMIGO
    // -------------------------------------------
    private void Die()
    {
        // sonido de muerte
        if (audioSource != null && deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position, 1f);
        }

        // efecto visual (opcional)
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
