using UnityEngine;

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

    private Rigidbody2D rb;
    private bool isPhase2 = false;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (bossHealth == null)
            bossHealth = GetComponent<BossHealth>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        bossHealth.OnDeath += HandleDeathEvent;
    }

    private void OnDestroy()
    {
        if (bossHealth != null)
            bossHealth.OnDeath -= HandleDeathEvent;
    }

    private void Update()
    {
        if (isDead) return;

        if (!isPhase2)
            MovePhase1();
        else
            MovePhase2();
    }

    // ---------------------- MOVIMIENTO ----------------------
    private void MovePhase1()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeedPhase1;
    }

    private void MovePhase2()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeedPhase2;
    }

    // -------------------- MANEJO DE MUERTE --------------------
    private void HandleDeathEvent()
    {
        if (isDead) return; // evita dobles disparos

        if (!isPhase2)
        {
            // PRIMERA VEZ llegando a 0 → transformar a fase 2
            StartCoroutine(TransformToPhase2());
        }
        else
        {
            // YA ESTABA EN FASE 2 → muerte real
            StartCoroutine(FinalDeath());
        }
    }

    // ---------------------- TRANSFORMACIÓN ----------------------
    private System.Collections.IEnumerator TransformToPhase2()
    {
        animator.SetTrigger("Phase2Start");

        yield return new WaitForSeconds(transformDelay);

        isPhase2 = true;
        bossHealth.ActivatePhase2();

        if (phase2Visuals != null)
            phase2Visuals.SetActive(true);
    }

    // ---------------------- MUERTE FINAL ----------------------
    private System.Collections.IEnumerator FinalDeath()
    {
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Death");

        yield return new WaitForSeconds(1.5f);

        Destroy(gameObject);
    }
}
