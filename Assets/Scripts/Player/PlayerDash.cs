using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerDash : MonoBehaviour
{
    [Header("Componentes")]
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 12f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("I-Frames")]
    [SerializeField] private float iFrameDuration = 0.2f;

    private bool isDashing = false;
    private bool canDash = true;
    private bool isInvulnerable = false;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();

        if (rb == null) Debug.LogError("Falta Rigidbody2D en el jugador.");
        if (playerMovement == null) Debug.LogError("Falta PlayerMovement en el jugador.");
        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Falta Animator en el jugador.");
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && canDash)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        // Desactiva temporalmente el movimiento normal
        playerMovement.enabled = false;

        // Dirección del dash
        Vector2 dashDir = GetDashDirection();

        // Aplicar velocidad
        rb.linearVelocity = dashDir * dashForce;
        animator.SetBool("IsDashing", true);

        // Activar i-frames
        StartCoroutine(Invulnerability());

        yield return new WaitForSeconds(dashDuration);

        // Detener dash
        rb.linearVelocity = Vector2.zero;
        playerMovement.enabled = true;
        isDashing = false;
        animator.SetBool("IsDashing", false);

        // Esperar cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
       
    }

    private Vector2 GetDashDirection()
    {
        var moveInputField = typeof(PlayerMovement).GetField("_moveInput", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var lastNonZeroMoveField = typeof(PlayerMovement).GetField("_lastNonZeroMove", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Vector2 moveInput = (Vector2)moveInputField.GetValue(playerMovement);
        Vector2 lastDir = (Vector2)lastNonZeroMoveField.GetValue(playerMovement);

        return moveInput.sqrMagnitude > 0.001f ? moveInput.normalized : lastDir;
    }

    private IEnumerator Invulnerability()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(iFrameDuration);
        isInvulnerable = false;
    }

    public bool IsInvulnerable() => isInvulnerable;
}