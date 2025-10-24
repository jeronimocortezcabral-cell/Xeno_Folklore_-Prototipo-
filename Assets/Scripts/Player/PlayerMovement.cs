using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _anim;

    [Header("Movimiento")]
    [SerializeField] private float _walkSpeed = 5f;         // Velocidad normal
    [SerializeField] private float _runMultiplier = 1.6f;   // Multiplicador de velocidad al correr

    private Vector2 _moveInput;
    private Vector2 _lastNonZeroMove = Vector2.down;
    private bool _isRunning = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        if (_anim == null) Debug.LogWarning("No Animator found on Player.");
    }

    private void OnMove(InputValue inputValue)
    {
        _moveInput = inputValue.Get<Vector2>();
    }

    private void Update()
    {
        // Detectar si se mantiene presionado Shift
        _isRunning = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;

        float speed = _moveInput.magnitude;
        if (_anim != null)
        {
            _anim.SetFloat("MoveX", _moveInput.x);
            _anim.SetFloat("MoveY", _moveInput.y);
            _anim.SetFloat("Speed", speed);

            if (_moveInput.sqrMagnitude > 0.001f)
            {
                _lastNonZeroMove = _moveInput.normalized;
                _anim.SetFloat("LastMoveX", _lastNonZeroMove.x);
                _anim.SetFloat("LastMoveY", _lastNonZeroMove.y);
            }
        }
    }

    private void FixedUpdate()
    {
        // Aplicar velocidad normal o de carrera
        float currentSpeed = _isRunning ? _walkSpeed * _runMultiplier : _walkSpeed;
        _rb.linearVelocity = _moveInput.normalized * currentSpeed;
    }
}
