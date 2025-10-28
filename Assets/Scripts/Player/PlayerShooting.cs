using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour {
    [Header("Referencias")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Animator animator; // Asignar en el inspector

    [Header("Configuración del disparo")]
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float fireRate = 0.5f; // Tiempo entre disparos (segundos)

    private float nextFireTime = 0f;

    // Parámetros del Animator (hash para mejor rendimiento)
    private readonly int hashAimX = Animator.StringToHash("AimX");
    private readonly int hashAimY = Animator.StringToHash("AimY");
    private readonly int hashShoot = Animator.StringToHash("Shoot");

    private void Awake() {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (firePoint == null)
            Debug.LogWarning("⚠️ PlayerShooting: firePoint no asignado en el inspector.");

        if (animator == null)
            Debug.LogWarning("⚠️ PlayerShooting: animator no asignado en el inspector.");
    }

    private void Update() {
        // Dispara con click derecho y respeta el cooldown
        if (Mouse.current.rightButton.wasPressedThisFrame && Time.time >= nextFireTime) {
            ShootTowardCursor();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void ShootTowardCursor() {
        if (mainCamera == null) {
            Debug.LogWarning("⚠️ MainCamera no asignada en PlayerShooting.");
            return;
        }

        if (firePoint == null || bulletPrefab == null) {
            Debug.LogWarning("⚠️ PlayerShooting: firePoint o bulletPrefab no asignado.");
            return;
        }

        // Obtener posición del mouse en el mundo
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        // Calcular dirección normalizada
        Vector2 direction = (worldPos - firePoint.position).normalized;

        // Actualizar animator para Blend Tree
        if (animator != null) {
            animator.SetFloat(hashAimX, direction.x);
            animator.SetFloat(hashAimY, direction.y);
            animator.SetTrigger(hashShoot);
        }

        // Instanciar la bala
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Aplicar velocidad usando linearVelocity
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null) {
            rbBullet.linearVelocity = direction * bulletSpeed;
        }
        else {
            Debug.LogWarning("⚠️ El prefab de la bala no tiene un Rigidbody2D asignado.");
        }
    }
}