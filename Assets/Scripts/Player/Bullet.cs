using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float damage = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        BossHealth boss = other.GetComponentInParent<BossHealth>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        // 3. Búsqueda de WerewolfHealth (para el lobizón)
        WerewolfHealth werewolf = other.GetComponentInParent<WerewolfHealth>();
        if (werewolf != null)
        {
            werewolf.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
    }
}
