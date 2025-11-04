using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour {
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Animator animator; // asignar en inspector

    [Header("Bullet Settings")]
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float fireRate = 0.5f; // segundos entre disparos

    private float nextFireTime = 0f;

    // Animator parameter hashes (opcional para rendimiento)
    private readonly int hashAimX = Animator.StringToHash("AimX");
    private readonly int hashAimY = Animator.StringToHash("AimY");
    private readonly int hashShoot = Animator.StringToHash("Shoot");

    private void Awake() {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (firePoint == null)
            Debug.LogWarning("PlayerShooting: firePoint no asignado.");

        if (animator == null)
            Debug.LogWarning("PlayerShooting: animator no asignado.");
    }

    private void Update() {
        // Disparo con click derecho y cooldown
        if (Mouse.current.rightButton.wasPressedThisFrame && Time.time >= nextFireTime) {
            ShootTowardCursor();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void ShootTowardCursor() {
        if (mainCamera == null) {
            Debug.LogWarning("MainCamera no asignada en PlayerShooting.");
            return;
        }

        if (firePoint == null || bulletPrefab == null) {
            Debug.LogWarning("PlayerShooting: firePoint o bulletPrefab no asignado.");
            return;
        }

        // Posición del mouse en mundo
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        // Dirección desde el firePoint hacia el mouse
        Vector2 direction = (worldPos - firePoint.position).normalized;

        // ---- Actualizo animator antes de disparar ----
        if (animator != null) {
            // Puedes normalizar los valores para evitar magnitudes distintas
            animator.SetFloat(hashAimX, direction.x);
            animator.SetFloat(hashAimY, direction.y);

            // Trigger para reproducir la animación de disparo
            animator.SetTrigger(hashShoot);
        }

        // Instanciar y dar velocidad a la bala
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null) {
            rbBullet.linearVelocity = direction * bulletSpeed;
        }
        else {
            Debug.LogWarning("Bullet prefab no tiene Rigidbody2D.");
        }
    }
}