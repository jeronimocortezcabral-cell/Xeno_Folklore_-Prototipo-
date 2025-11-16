using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    private Rigidbody2D _rb;
    private Animator _anim;

    [Header("Movimiento")]
    [SerializeField] private float _walkSpeed = 5f;         
    [SerializeField] private float _runMultiplier = 1.6f;

    private Vector2 _moveInput;
    private Vector2 _lastNonZeroMove = Vector2.down;
    private bool _isRunning = false;

    private bool _inputEnabled = true;
    public bool InputEnabled => _inputEnabled;

    public Vector2 Facing => _lastNonZeroMove;

    private void Start() {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        if (_anim == null) Debug.LogWarning("No Animator found on Player.");
    }

    private void OnMove(InputValue inputValue) 
    {
        if (!_inputEnabled) 
        {
            _moveInput = Vector2.zero;
            return;
        }

        _moveInput = inputValue.Get<Vector2>();
    }

    private void Update() 
    {
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
        if (!_inputEnabled) 
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        float currentSpeed = _isRunning ? _walkSpeed * _runMultiplier : _walkSpeed;
        _rb.linearVelocity = _moveInput.normalized * currentSpeed;
    }

    public void SetInputEnabled(bool enabled) 
    {
        _inputEnabled = enabled;
        if (!enabled)
        {
            _moveInput = Vector2.zero;
            _rb.linearVelocity = Vector2.zero;
            if (_anim != null) 
            {
                _anim.SetFloat("Speed", 0f);
            }
        }
    }
}