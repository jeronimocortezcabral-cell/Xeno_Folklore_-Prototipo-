using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour 
{
    private Rigidbody2D _rb;
    private Animator _anim;

    [SerializeField] private float _speed = 5f;
    private Vector2 _moveInput;
    private Vector2 _lastNonZeroMove = Vector2.down; // por defecto mirando al frente (abajo)

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

    private void Update() {
        // Actualizo los par�metros del animator (m�s responsivo en Update)
        float speed = _moveInput.magnitude;
        if (_anim != null) 
        {
            // par�metros que usaremos en el Animator
            _anim.SetFloat("MoveX", _moveInput.x);
            _anim.SetFloat("MoveY", _moveInput.y);
            _anim.SetFloat("Speed", speed);

            // Guardar la �ltima direcci�n no nula (para idle facing)
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
        // mover en FixedUpdate con velocity
        _rb.linearVelocity = _moveInput.normalized * _speed;
    }
}
