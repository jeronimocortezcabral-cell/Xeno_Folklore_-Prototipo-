using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;

    private Vector2 shootDirection = Vector2.right;
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 moveVelocity = _rb.linearVelocity;
        if (moveVelocity != Vector2.zero)
            shootDirection = moveVelocity.normalized;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }


    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        rbBullet.linearVelocity = shootDirection * bulletSpeed;
    }
}