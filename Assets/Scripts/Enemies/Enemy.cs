using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health = 3f;
    [SerializeField] private GameObject deathEffect;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
