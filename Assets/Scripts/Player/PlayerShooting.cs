using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float fireRate = 0.5f; // Tiempo entre disparos (en segundos)

    private float nextFireTime = 0f;

    private void Update()
    {
        // Solo dispara si se hace click derecho una vez y ya pasó el cooldown
        if (Mouse.current.rightButton.wasPressedThisFrame && Time.time >= nextFireTime)
        {
            ShootTowardCursor();
            nextFireTime = Time.time + fireRate; // Activa cooldown
        }
    }

    private void ShootTowardCursor()
    {
        if (mainCamera == null)
        {
            Debug.LogWarning("MainCamera no asignada en PlayerShooting.");
            return;
        }

        // Obtener posición del mouse en el mundo
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        // Calcular dirección
        Vector2 direction = (worldPos - firePoint.position).normalized;

        // Instanciar bala
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        rbBullet.linearVelocity = direction * bulletSpeed;
    }
}