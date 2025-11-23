using UnityEngine;
using System.Collections;

// Definimos los posibles estados del enemigo
public enum WerewolfState
{
    Patrol,
    Wait,
    Chase,
    Attack,
    Dormant // Nuevo estado: Inactivo, esperando activación
}

public class WerewolfBehavior : MonoBehaviour
{
    // VARIABLES DE CONFIGURACIÓN DEL LOBO (Se asumen configuradas en el Inspector)
    [Header("Configuración de Movimiento")]
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float patrolRange = 10f;
    [SerializeField] private float nextPatrolActionTime = 2f;

    [Header("Configuración de Combate")]
    [SerializeField] private float chaseRange = 6f; // RANGO CLAVE PARA LA MÚSICA
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float damageAmount = 1f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float attackTimer;

    // --- VARIABLES DE ACTIVACIÓN ---
    [Header("Activación")]
    [SerializeField] private int activationKeysRequired = 5;
    private bool isActivated = false;
    private PlayerInventory playerInventory; // Referencia al inventario del jugador

    // --- REFERENCIAS INTERNAS ---
    private Transform player;
    private Rigidbody2D rb;
    private WerewolfState currentState = WerewolfState.Dormant; // INICIA EN DORMANT
    private Vector2 initialPosition;
    private Vector2 patrolTarget;
    private float stateTimer;
    private SpriteRenderer spriteRenderer;
    private Collider2D entityCollider;
    public Animator anim;

    // **********************************************
    // NUEVA VARIABLE DE AUDIO
    // **********************************************
    private bool combatMusicStarted = false; // Controla que la música de combate solo inicie una vez

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        entityCollider = GetComponent<Collider2D>();

        // 1. Buscar al jugador y su inventario
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerInventory = playerObj.GetComponent<PlayerInventory>();
        }
        else
        {
            Debug.LogError("Objeto 'Player' no encontrado. La activación no funcionará.");
        }

        initialPosition = transform.position;
        patrolTarget = initialPosition;
        stateTimer = nextPatrolActionTime;

        // 2. Desactivar componentes al inicio
        SetWerewolfActive(false);
    }

    private void Update()
    {
        // PRIORIDAD: Monitorear la activación si está inactivo
        if (!isActivated)
        {
            CheckActivationCondition();
            return; // No ejecutamos la lógica de movimiento/ataque
        }

        if (player == null || currentState == WerewolfState.Dormant)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // --- Lógica de Combate y Patrulla (Solo si está activado) ---

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Lógica de Ataque
        if (distanceToPlayer <= attackRange)
        {
            SetState(WerewolfState.Attack);
            StartWerewolfMusic(); // Asegura que la música esté activa
        }
        // Lógica de Persecución
        else if (distanceToPlayer <= chaseRange)
        {
            SetState(WerewolfState.Chase);
            StartWerewolfMusic(); // Asegura que la música esté activa
        }
        // Lógica de Patrulla / Jugador fuera de rango
        else
        {
            // **********************************************
            // NUEVO: DETENER MÚSICA AL SALIR DE RANGO CHASE
            // **********************************************
            StopWerewolfMusic();
            // **********************************************

            // Mantiene el estado de Wait/Patrol si no estaba atacando o persiguiendo
            if (currentState == WerewolfState.Chase || currentState == WerewolfState.Attack)
            {
                SetState(WerewolfState.Wait);
            }
        }

        // Manejar el temporizador de patrulla/espera
        if (currentState == WerewolfState.Wait || currentState == WerewolfState.Patrol)
        {
            HandlePatrolTimer();
        }

        // Manejar el temporizador de ataque
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    // ----------------------------------------------------
    // FUNCIÓN DE GESTIÓN DE AUDIO (INICIO)
    // ----------------------------------------------------
    private void StartWerewolfMusic()
    {
        if (MusicManager.instance != null && !combatMusicStarted)
        {
            MusicManager.instance.PlayWerewolfMusic();
            combatMusicStarted = true; // Asegura que solo se inicie una vez hasta que se detenga
            Debug.Log("Música de combate del Lobizón iniciada.");
        }
    }

    // ----------------------------------------------------
    // FUNCIÓN DE GESTIÓN DE AUDIO (FIN)
    // ----------------------------------------------------
    private void StopWerewolfMusic()
    {
        if (MusicManager.instance != null && combatMusicStarted)
        {
            // Asumiendo que PlayOutsideMusic() vuelve a la pista de fondo del nivel
            MusicManager.instance.PlayOutsideMusic();
            combatMusicStarted = false;
            Debug.Log("Música de combate del Lobizón detenida, volviendo a música de nivel.");
        }
    }

    // ----------------------------------------------------
    // FUNCIÓN DE ACTIVACIÓN
    // ----------------------------------------------------
    private void CheckActivationCondition()
    {
        if (playerInventory == null) return;

        // Condición de activación: 5 "keys" (Ship Parts)
        if (playerInventory.GetTotalKeys() >= activationKeysRequired)
        {
            Debug.Log("¡El Lobizón se ha activado! El jugador recolectó 5 piezas.");
            SetWerewolfActive(true);
        }
    }

    private void SetWerewolfActive(bool active)
    {
        isActivated = active;

        // 1. Control de Visibilidad e Interacción
        if (spriteRenderer != null) spriteRenderer.enabled = active;
        if (entityCollider != null) entityCollider.enabled = active;

        // 2. Control de Física
        if (rb != null) rb.bodyType = active ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;

        // 3. Establecer estado inicial tras la activación
        if (active)
        {
            SetState(WerewolfState.Wait); // Empieza a esperar/patrullar en el lugar
            // NOTA: La música se inicia en Update() cuando el jugador entra en el chaseRange por primera vez.
        }
        else
        {
            SetState(WerewolfState.Dormant);
        }
    }

    // ----------------------------------------------------
    // LÓGICA DE MOVIMIENTO, ESTADOS Y ATAQUE (Resto del script sin cambios)
    // ----------------------------------------------------

    private void FixedUpdate()
    {
        if (!isActivated) return; // No se mueve si no está activo

        if (currentState == WerewolfState.Chase)
        {
            Move(player.position, chaseSpeed);
        }
        else if (currentState == WerewolfState.Patrol)
        {
            Move(patrolTarget, patrolSpeed);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        UpdateAnimation();
    }

    private void Move(Vector2 target, float speed)
    {
        Vector2 direction = (target - rb.position).normalized;
        rb.linearVelocity = direction * speed;

        // Lógica de Giro (Flip)
        if (direction.x != 0)
        {
            float targetSign = Mathf.Sign(direction.x) * -1f;

            transform.localScale = new Vector3(targetSign * Mathf.Abs(transform.localScale.x),
                                                 transform.localScale.y,
                                                 transform.localScale.z);
        }
    }

    private void SetState(WerewolfState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }

    private void HandlePatrolTimer()
    {
        // Lógica de Patrulla y Espera
        // ... (Tu lógica de temporizador de patrulla aquí) ...
    }

    private void SetNewPatrolTarget()
    {
        // Lógica para establecer nuevo punto de patrulla
        // ... (Tu lógica de objetivo de patrulla aquí) ...
    }

    private void UpdateAnimation()
    {
        // Lógica de animación
        bool isMoving = currentState == WerewolfState.Chase || currentState == WerewolfState.Patrol;
        anim.SetBool("IsWalking", isMoving);

        if (currentState == WerewolfState.Attack)
        {
            rb.linearVelocity = Vector2.zero;
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (attackTimer <= 0)
        {
            anim.SetTrigger("Attack");
            attackTimer = attackCooldown;
            StartCoroutine(ApplyDamageAfterDelay(0.3f));
        }
    }

    private IEnumerator ApplyDamageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange * 1.1f)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damageAmount);
            }
        }
    }
}