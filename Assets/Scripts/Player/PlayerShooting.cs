using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private Camera mainCamera;

    private void Update()
    {
        // Botón derecho del mouse (click derecho)
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            ShootTowardCursor();
        }
    }

    private void ShootTowardCursor()
    {
        // Seguridad: evita errores si la cámara no está asignada
        if (mainCamera == null)
        {
            Debug.LogWarning("MainCamera no asignada en PlayerShooting.");
            return;
        }

        // Obtener posición del mouse y convertirla a coordenadas del mundo
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f; // Importante en 2D: mantiene todo en el mismo plano

        // Calcular dirección del disparo
        Vector2 direction = (worldPos - firePoint.position).normalized;

        // Instanciar bala
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        rbBullet.linearVelocity = direction * bulletSpeed;
    }
}