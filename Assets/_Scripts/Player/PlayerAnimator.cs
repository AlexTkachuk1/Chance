using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public sealed class PlayerAnimator : MonoBehaviour
{
    #region Properties

    private Animator _animator;
    private PlayerInput _playerInput;

    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void FixedUpdate()
    {
        Vector2 axis = _playerInput.GetVector();

        if (axis.x == 0) _animator.SetFloat("Speed", axis.x);

        Rotate(axis.x, transform);
    }

    /// <summary>
    /// Method that subscribes/unsubscribes to move action and provides Move animation
    /// </summary>
    /// <param name="obj"></param>
    public void PerformMove(InputAction.CallbackContext obj)
    {
        float axis = MathF.Abs(_playerInput.GetVector().x);
        _animator.SetFloat("Speed", axis);
    }

    /// <summary>
    /// Rotate  object based on relative position
    /// </summary>
    /// <param name="x">Direction</param>
    /// <param name="transform">Transform of object</param>
    public static void Rotate(float x, Transform transform)
    {
        if (x == 0) return;

        float angle = x < 0.0f ? 180f : 0f;
        transform.eulerAngles = new Vector2(0f, angle);
    }

    public void PerformJumpPreparing() => _animator.SetTrigger("JumpPreparing");

    public void PerformJump() => _animator.SetTrigger("Jump");

    public void PerformFalling() => _animator.SetBool("Grounded", false);

    public void StopFalling() => _animator.SetBool("Grounded", true);
}
