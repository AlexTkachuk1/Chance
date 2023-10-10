using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public sealed class PlayerMover : MonoBehaviour
{
    #region Properties

    [SerializeField] private float _minGroundNormalY = .65f;
    [SerializeField] private float _gravityModifier = 1f;
    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private float _jumpForceMultiplier = .02f;
    [SerializeField] private Vector2 _velocity;
    [SerializeField] private LayerMask _layerMask;

    private float _jumpForceModifier = 0f;
    private bool _increseJumpForceModifier = false;
    private bool _canMove = true;
    private PlayerInput _playerInput;
    private PlayerAnimator _playerAnimator;
    private Vector2 _targetVelocity;
    private bool _grounded;
    private Vector2 _groundNormal;
    private Rigidbody2D _rb2d;
    private ContactFilter2D _contactFilter;
    private RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
    private List<RaycastHit2D> _hitBufferList = new List<RaycastHit2D>(16);
    private AngleController _angleController;

    private const float _minMoveDistance = 0.001f;
    private const float _shellRadius = 0.01f;
    #endregion

    private void OnEnable()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _playerAnimator = GetComponent<PlayerAnimator>();
        _angleController = GetComponentInChildren<AngleController>();
    }

    private void Start()
    {
        _contactFilter.useTriggers = false;
        _contactFilter.SetLayerMask(_layerMask);
        _contactFilter.useLayerMask = true;
    }

    private void Update()
    {
        _targetVelocity = new Vector2(_playerInput.GetVector().x, 0);
        if (_increseJumpForceModifier) IncreseJumpForceModifier();
        _angleController.PointerPosition = _playerInput.GetPointerPosition();
    }

    private void FixedUpdate()
    {
        _velocity += _gravityModifier * Time.deltaTime * Physics2D.gravity;
        _velocity.x = _targetVelocity.x * _speed;

        _grounded = false;

        Vector2 deltaPosition = _velocity * Time.deltaTime;
        Vector2 moveAlongGround = new(_groundNormal.y, -_groundNormal.x);
        Vector2 move = moveAlongGround * deltaPosition.x;

        if (_canMove) Movement(move, false);

        move = Vector2.up * deltaPosition.y;

        Movement(move, true);

        if (_grounded) _playerAnimator.StopFalling();
        else _playerAnimator.PerformFalling();
    }

    private void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > _minMoveDistance)
        {
            int count = _rb2d.Cast(move, _contactFilter, _hitBuffer, distance + _shellRadius);

            _hitBufferList.Clear();

            for (int i = 0; i < count; i++)
            {
                _hitBufferList.Add(_hitBuffer[i]);
            }

            for (int i = 0; i < count; i++)
            {
                Vector2 currentNormal = _hitBuffer[i].normal;
                if (currentNormal.y > _minGroundNormalY)
                {
                    _grounded = true;
                    if (yMovement)
                    {
                        _groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(_velocity, currentNormal);
                if (projection < 0)
                {
                    _velocity -= projection * currentNormal;
                }

                float modifiedDistance = _hitBufferList[i].distance - _shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }

        _rb2d.position += move.normalized * distance;
    }

    public void SetYVelosity(InputAction.CallbackContext obj)
    {
        if (_grounded)
        {
            _velocity.y = _jumpForce * _jumpForceModifier;
            _playerAnimator.PerformJump();
            _canMove = true;
        }
        _increseJumpForceModifier = false;
        _jumpForceModifier = 0f;
    }

    public void StartIncreseJumpForceModifier(InputAction.CallbackContext obj)
    {
        if (_grounded)
        {
            _playerAnimator.PerformJumpPreparing();
            _increseJumpForceModifier = true;
            _canMove = false;
        }
    }

    private void IncreseJumpForceModifier()
    {
        if (_jumpForceModifier < 1f) _jumpForceModifier += _jumpForceMultiplier;
        else
        {
            _jumpForceModifier = 1;
            _increseJumpForceModifier = false;
        }
    }
}
