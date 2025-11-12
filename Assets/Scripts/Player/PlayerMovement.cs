using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    private Rigidbody2D _rb;
    private Animator _anim;

    [Header("Movimiento")]
    [SerializeField] private float _walkSpeed = 5f;         // Velocidad normal
    [SerializeField] private float _runMultiplier = 1.6f;   // Multiplicador de velocidad al correr

    private Vector2 _moveInput;
    private Vector2 _lastNonZeroMove = Vector2.down;
    private bool _isRunning = false;

    // NUEVO: control de input (para pausar movimiento durante diálogos)
    private bool _inputEnabled = true;
    public bool InputEnabled => _inputEnabled;

    // NUEVO: propiedad pública para que otros scripts (ej. DialogueTrigger) consulten la dirección "facing"
    public Vector2 Facing => _lastNonZeroMove;

    private void Start() {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        if (_anim == null) Debug.LogWarning("No Animator found on Player.");
    }

    // OnMove es llamado por el New Input System
    private void OnMove(InputValue inputValue) {
        if (!_inputEnabled) {
            // si el input está deshabilitado, ignoramos la entrada
            _moveInput = Vector2.zero;
            return;
        }

        _moveInput = inputValue.Get<Vector2>();
    }

    private void Update() {
        // Detectar si se mantiene presionado Shift
        _isRunning = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;

        float speed = _moveInput.magnitude;
        if (_anim != null) {
            _anim.SetFloat("MoveX", _moveInput.x);
            _anim.SetFloat("MoveY", _moveInput.y);
            _anim.SetFloat("Speed", speed);

            if (_moveInput.sqrMagnitude > 0.001f) {
                _lastNonZeroMove = _moveInput.normalized;
                _anim.SetFloat("LastMoveX", _lastNonZeroMove.x);
                _anim.SetFloat("LastMoveY", _lastNonZeroMove.y);
            }
        }
    }

    private void FixedUpdate() {
        // Aplicar velocidad normal o de carrera
        if (!_inputEnabled) {
            // detener físicamente al jugador si el input está deshabilitado
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        float currentSpeed = _isRunning ? _walkSpeed * _runMultiplier : _walkSpeed;
        _rb.linearVelocity = _moveInput.normalized * currentSpeed;
    }

    // MÉTODO PÚBLICO para habilitar/deshabilitar el input (lo llama DialogueManager)
    public void SetInputEnabled(bool enabled) {
        _inputEnabled = enabled;
        if (!enabled) {
            // limpiar input actual y detener velocidad inmediatamente
            _moveInput = Vector2.zero;
            _rb.linearVelocity = Vector2.zero;
            if (_anim != null) {
                _anim.SetFloat("Speed", 0f);
            }
        }
    }
}