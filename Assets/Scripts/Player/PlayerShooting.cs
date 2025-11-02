using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float fireRate = 0.4f;

    [Header("Munición")]
    [SerializeField] private int maxAmmo = 3;
    [SerializeField] private float reloadInterval = 1f;

    private int currentAmmo;
    private bool isReloading = false;
    private bool canShoot = true;

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            TryShoot();
        }
    }

    private void TryShoot()
    {
        if (!canShoot || isReloading)
        {
            Debug.Log("No puedes disparar ahora (recargando o cooldown activo).");
            return;
        }

        if (currentAmmo > 0)
        {
            ShootTowardCursor();
            currentAmmo--;
            Debug.Log($"Disparo realizado. Balas restantes: {currentAmmo}");

            StartCoroutine(ShootCooldown());

            if (currentAmmo <= 0)
                StartCoroutine(ReloadRoutine());
        }
        else
        {
            Debug.Log("Sin munición. Esperando recarga...");
        }
    }

    private void ShootTowardCursor()
    {
        if (mainCamera == null)
        {
            Debug.LogWarning("MainCamera no asignada en PlayerShooting.");
            return;
        }

        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        Vector2 direction = (worldPos - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        rbBullet.linearVelocity = direction * bulletSpeed;
    }

    private IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log("Comenzando recarga...");

        while (currentAmmo < maxAmmo)
        {
            yield return new WaitForSeconds(reloadInterval);
            currentAmmo++;
            Debug.Log($"Recargando... Balas actuales: {currentAmmo}");
        }

        Debug.Log("Recarga completa.");
        isReloading = false;
    }
}
