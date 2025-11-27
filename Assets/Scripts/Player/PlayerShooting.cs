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
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;

    [Header("Bullet Settings")]
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float fireRate = 0.5f;

    [Header("Magazine Settings")]
    [SerializeField] private int maxAmmo = 3;
    [SerializeField] private float reloadDelayPerBullet = 1f; // 1 bala por segundo

    private int currentAmmo;
    private bool isReloading = false;
    private float nextReloadTime = 0f;

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

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        // --- DISPARAR ---
        if (Mouse.current.rightButton.wasPressedThisFrame && Time.time >= nextFireTime)
        {
            TryShoot();
        }

        // --- RECARGA AUTOMÁTICA ---
        HandleReload();
    }

    private void TryShoot()
    {
        if (currentAmmo <= 0)
            return; // no hay balas, esperar recarga

        ShootTowardCursor();
        nextFireTime = Time.time + fireRate;

        currentAmmo--;

        // Si el cargador llegó a 0 → empieza la recarga
        if (currentAmmo <= 0)
        {
            isReloading = true;
            nextReloadTime = Time.time + reloadDelayPerBullet;
        }
    }

    private void HandleReload()
    {
        if (!isReloading)
            return;

        if (Time.time >= nextReloadTime)
        {
            currentAmmo++;
            nextReloadTime = Time.time + reloadDelayPerBullet;

            // Cuando el cargador vuelve al máximo → deja de recargar
            if (currentAmmo >= maxAmmo)
            {
                currentAmmo = maxAmmo;
                isReloading = false;
            }
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

        // --- ANIMACIONES ---
        if (animator != null)
        {
            animator.SetFloat(hashAimX, direction.x);
            animator.SetFloat(hashAimY, direction.y);
            animator.SetTrigger(hashShoot);
        }

        // --- SONIDO ---
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // --- ROTAR BALA ---
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion bulletRotation = Quaternion.Euler(0, 0, angle);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();

        if (rbBullet != null)
        {
            rbBullet.linearVelocity = direction * bulletSpeed;
        }
    }
}
