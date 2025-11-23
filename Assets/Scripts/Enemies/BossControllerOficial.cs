using UnityEngine;
using System.Collections; // Necesario para Coroutines

public class BossControllerOficial : MonoBehaviour
{
    [Header("Referencias")]
    public BossHealth bossHealth;
    public Animator animator;
    public Transform player;

    [Header("Fase 2")]
    public GameObject phase2Visuals;
    public float transformDelay = 1.5f;

    [Header("Movimiento y ataques")]
    public float moveSpeedPhase1 = 2f;
    public float moveSpeedPhase2 = 4f;

    [Header("Ataques")]
    public GameObject projectilePrefab;
    public float attackCooldown = 5f;
    public float chargeDuration = 1f;
    public float shootDelay = 0.3f;
    public float projectileSpeed = 6f;

    [Header("Condiciones de Aparición y Detección")]
    public int itemsRequiredToSpawn = 4;
    public float detectionRange = 5f; // RANGO CLAVE PARA LA MÚSICA

    // ---------------------------------------------------------
    [Header("Recompensa (Loot)")]
    public GameObject itemDropPrefab;
    // ---------------------------------------------------------

    private bool hasSpawned = false;
    private Collider2D bossCollider;
    private SpriteRenderer bossRenderer;

    // REFERENCIA: Script de inventario
    private PlayerInventory playerInventory;

    private Rigidbody2D rb;
    private bool isPhase2 = false;
    private bool isDead = false;
    private bool isAttacking = false;
    private float attackTimer = 0f;

    // **********************************************
    // NUEVA VARIABLE DE AUDIO
    // **********************************************
    private bool combatMusicStarted = false; // Controla que la música de combate solo inicie/detenga una vez

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bossCollider = GetComponent<Collider2D>();
        bossRenderer = GetComponent<SpriteRenderer>();

        if (bossHealth == null)
            bossHealth = GetComponent<BossHealth>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // CONEXIÓN CON INVENTARIO
        if (player != null)
        {
            playerInventory = player.GetComponent<PlayerInventory>();

            if (playerInventory == null)
            {
                Debug.LogError("¡ERROR CRÍTICO! El Player NO tiene el script 'PlayerInventory'.");
            }
        }

        bossHealth.OnDeath += HandleDeathEvent;
    }

    private void Start()
    {
        ToggleBossVisibility(false);
    }

    private void OnDestroy()
    {
        if (bossHealth != null)
            bossHealth.OnDeath -= HandleDeathEvent;
    }

    private void Update()
    {
        if (isDead) return;

        // 1) LÓGICA DE APARICIÓN (Spawn)
        if (!hasSpawned)
        {
            if (GetPlayerItemCount() < itemsRequiredToSpawn)
                return;

            SpawnBoss();
        }

        // Si aún no ha aparecido (hasSpawned=false), el resto del código no se ejecuta.
        if (!hasSpawned) return;

        // 2) LÓGICA DE DETECCIÓN, COMBATE Y MÚSICA
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Si el jugador está en rango de detección:
        if (distanceToPlayer <= detectionRange)
        {
            // **********************************************
            // INTEGRACIÓN DE AUDIO: INICIO DEL COMBATE
            // **********************************************
            if (MusicManager.instance != null && !combatMusicStarted)
            {
                MusicManager.instance.PlayChonchonMusic();
                combatMusicStarted = true;
                Debug.Log("Música de combate del Chonchón iniciada al entrar en rango.");
            }
            // **********************************************

            // COMBATE
            if (isAttacking) return;

            attackTimer += Time.deltaTime;

            if (attackTimer >= attackCooldown)
            {
                StartCoroutine(AttackRoutine());
                return;
            }

            if (!isPhase2)
                MovePhase1();
            else
                MovePhase2();
        }
        else // El jugador está fuera del rango de detección
        {
            // **********************************************
            // INTEGRACIÓN DE AUDIO: FIN DEL COMBATE
            // **********************************************
            if (combatMusicStarted)
            {
                if (MusicManager.instance != null)
                {
                    MusicManager.instance.PlayOutsideMusic(); // Vuelve a la música de nivel
                    combatMusicStarted = false;
                    Debug.Log("Música de combate del Chonchón detenida al salir de rango.");
                }
            }
            // **********************************************

            // Detenemos el movimiento si estamos fuera de rango y no atacando
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("Moving", false);
        }
    }

    // ---------------------- FUNCIONES AUXILIARES (Resto del script sin cambios) ----------------------

    private int GetPlayerItemCount()
    {
        if (playerInventory == null) return 0;
        return playerInventory.GetTotalKeys();
    }

    private void SpawnBoss()
    {
        hasSpawned = true;
        ToggleBossVisibility(true);
    }

    private void ToggleBossVisibility(bool state)
    {
        if (bossRenderer != null) bossRenderer.enabled = state;
        if (bossCollider != null) bossCollider.enabled = state;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    // ---------------------- MOVIMIENTO ----------------------
    private void MovePhase1()
    {
        if (player == null) return;
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeedPhase1;
        animator.SetBool("Moving", true);
    }

    private void MovePhase2()
    {
        if (player == null) return;
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeedPhase2;
        animator.SetBool("Moving", true);
    }

    // ---------------------- ATAQUE ----------------------
    private System.Collections.IEnumerator AttackRoutine()
    {
        isAttacking = true;
        attackTimer = 0f;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("Moving", false);

        animator.SetTrigger("Charge");
        yield return new WaitForSeconds(chargeDuration);

        animator.SetTrigger("Shoot");
        yield return new WaitForSeconds(shootDelay);
        ShootAtPlayer();
        yield return new WaitForSeconds(0.4f);

        isAttacking = false;
    }

    private void ShootAtPlayer()
    {
        if (player == null || projectilePrefab == null) return;
        Vector2 dir = (player.position - transform.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        proj.GetComponent<Rigidbody2D>().linearVelocity = dir * projectileSpeed;
    }

    // -------------------- MANEJO DE MUERTE --------------------
    private void HandleDeathEvent()
    {
        if (isDead) return;

        if (!isPhase2)
            StartCoroutine(TransformToPhase2());
        else
            StartCoroutine(FinalDeath());
    }

    private System.Collections.IEnumerator TransformToPhase2()
    {
        animator.SetTrigger("Phase2Start");
        yield return new WaitForSeconds(transformDelay);
        isPhase2 = true;
        bossHealth.ActivatePhase2();
        if (phase2Visuals != null) phase2Visuals.SetActive(true);
    }

    // ---------------------- MUERTE FINAL Y DROP ----------------------
    private System.Collections.IEnumerator FinalDeath()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Death");

        // **********************************************
        // INTEGRACIÓN DE AUDIO: MUERTE (DEFEATED BOSS)
        // **********************************************
        if (MusicManager.instance != null)
        {
            // Llama a la función para la música de victoria
            MusicManager.instance.DefeatedBoss();
            Debug.Log("Chonchón derrotado. Transición musical de muerte/victoria activada.");
        }
        // **********************************************

        yield return new WaitForSeconds(1.5f);

        if (itemDropPrefab != null)
        {
            Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("No has asignado el 'Item Drop Prefab' en el inspector del Boss.");
        }

        Destroy(gameObject);
    }
}