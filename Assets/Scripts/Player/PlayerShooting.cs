using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;  // debe estar en el player
    [SerializeField] private AudioClip shootSound;     // sonido del disparo

    [Header("Bullet Settings")]
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float fireRate = 0.5f;

    private float nextFireTime = 0f;

    private readonly int hashAimX = Animator.StringToHash("AimX");
    private readonly int hashAimY = Animator.StringToHash("AimY");
    private readonly int hashShoot = Animator.StringToHash("Shoot");

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (firePoint == null)
            Debug.LogWarning("PlayerShooting: firePoint no asignado.");

        if (animator == null)
            Debug.LogWarning("PlayerShooting: animator no asignado.");

        if (audioSource == null)
            Debug.LogWarning("PlayerShooting: audioSource no asignado.");
    }

    private void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame && Time.time >= nextFireTime)
        {
            ShootTowardCursor();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void ShootTowardCursor()
    {
        if (mainCamera == null || firePoint == null || bulletPrefab == null)
            return;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        Vector2 direction = (worldPos - firePoint.position).normalized;

        if (animator != null)
        {
            animator.SetFloat(hashAimX, direction.x);
            animator.SetFloat(hashAimY, direction.y);
            animator.SetTrigger(hashShoot);
        }

        // 🔊 REPRODUCIR SONIDO
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();

        if (rbBullet != null)
        {
            rbBullet.linearVelocity = direction * bulletSpeed;
        }
        else
        {
            Debug.LogWarning("Bullet prefab no tiene Rigidbody2D.");
        }
    }
}
