using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInput;

[CreateAssetMenu(fileName = "InputReader", menuName = "Inputs/InputReader")]
public class InputReader : ScriptableObject, IMovementActions, IMenusActions
{
    public event Action<Vector2> Movement;
    public event Action<Vector2> Aim;
    public event Action Jump;
    public event Action<bool> Sprint;
    public event Action Crouch;
    public event Action<bool> CameraZoom;

    public event Action Throw;
    public event Action Interact;
    public event Action Grab;
    public event Action SecondInteract;

    public event Action PauseClick;

    private PlayerInput controls;

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new PlayerInput();
            controls.Movement.SetCallbacks(this);
            controls.Menus.SetCallbacks(this);
        }

        controls.Movement.Enable();
        controls.Menus.Enable();
    }

    private void OnDisable()
    {
        controls.Movement.Disable();
        controls.Menus.Disable();
    }

    void IMovementActions.OnMovement(InputAction.CallbackContext context)
    {
        Movement?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        Sprint?.Invoke(context.ReadValueAsButton());
    }

    public void OnCameraMovement(InputAction.CallbackContext context)
    {
        Aim?.Invoke(context.ReadValue<Vector2>());
    }

    void IMovementActions.OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) Jump?.Invoke();
    }

    void IMovementActions.OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed) Interact?.Invoke();
    }

    public void OnSecondInteract(InputAction.CallbackContext context)
    {
        if (context.performed) SecondInteract?.Invoke();
    }

    void IMovementActions.OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed) Grab?.Invoke();
    }

    void IMovementActions.OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed) Throw?.Invoke();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed) Crouch?.Invoke();
    }

    public void OnCameraZoom(InputAction.CallbackContext context)
    {
        CameraZoom?.Invoke(context.ReadValueAsButton());
    }

    private string GamepadSchemeName => controls.asset.controlSchemes
        .FirstOrDefault(s => s.name.Contains("Gamepad")).name ?? string.Empty;

    private string KeyboardSchemeName => controls.asset.controlSchemes
        .FirstOrDefault(s => s.name.Contains("Keyboard")).name ?? string.Empty;

    private string CurrentControlScheme
    {
        get
        {
            if (controls == null) return string.Empty;
            return Gamepad.current != null ? GamepadSchemeName : KeyboardSchemeName;
        }
    }

    private string GetBindingDisplayString(InputAction action)
    {
        if (controls == null) return string.Empty;

        string scheme = CurrentControlScheme;
        if (string.IsNullOrEmpty(scheme)) return string.Empty;

        return action.GetBindingDisplayString(InputBinding.MaskByGroup(scheme));
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed) PauseClick?.Invoke();
    }

    public string InteractKey => GetBindingDisplayString(controls?.Movement.Interact);
    public string GrabKey => GetBindingDisplayString(controls?.Movement.Grab);
    public string ThrowKey => GetBindingDisplayString(controls?.Movement.Throw);
}