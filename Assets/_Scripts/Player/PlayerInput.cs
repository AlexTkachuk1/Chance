using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerAnimator))]
public sealed class PlayerInput : MonoBehaviour
{
    #region Properties

    [SerializeField] private InputActionReference _move, _jump, _pointerPosition;
    [SerializeField] private Camera _mainCamera;

    private PlayerAnimator _playerAnimator;
    private PlayerMover _playerMover;

    #endregion

    private void Awake()
    {
        _playerAnimator = GetComponent<PlayerAnimator>();
        _playerMover = GetComponent<PlayerMover>();
    }

    private void Update()
    {
        print(GetPointerPosition());
    }

    /// <summary>
    /// OnEnable invokes on object creation , adds methods references to InpusActions
    /// </summary>
    private void OnEnable()
    {
        _move.action.performed += _playerAnimator.PerformMove;
        _jump.action.started += _playerMover.StartIncreseJumpForceModifier;
        _jump.action.canceled += _playerMover.SetYVelosity;
    }

    /// <summary>
    /// OnDisable invokes on object destruction , removes methods references from InpusActions
    /// </summary>
    private void OnDisable()
    {
        _move.action.performed -= _playerAnimator.PerformMove;
        _jump.action.started -= _playerMover.StartIncreseJumpForceModifier;
        _jump.action.canceled -= _playerMover.SetYVelosity;
    }

    /// <summary>
    /// Read value from <see cref="_move"/>
    /// </summary>
    /// <returns><see cref="Vector2"/> Vector in which player moves</returns>
    public Vector2 GetVector() => _move.action.ReadValue<Vector2>();

    public Vector2 GetPointerPosition()
    {
        Vector2 mousePosition = _pointerPosition.action.ReadValue<Vector2>();
        Vector2 worldPos = _mainCamera.ScreenToWorldPoint(mousePosition);
        return worldPos;
    }
}
