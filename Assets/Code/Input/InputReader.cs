using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInput;

// SETUP NECESSÁRIO NO INPUT ACTIONS ASSET:
// 1. No Action Map "Movement", adiciona:
//    - Action "Inspect" (Action Type: Button)
//      Keyboard binding: ex. F  |  Gamepad binding: ex. North Button (Y/Triangle)
//    - Action "InspectLook" (Action Type: Value, Control Type: Vector2)
//      Keyboard+Mouse binding: Delta [Mouse]  |  Gamepad binding: Right Stick [Gamepad]
// 2. Garante que ambas as actions estão no Control Scheme correto (Keyboard&Mouse e Gamepad)
// 3. No generated C# (IMovementActions), os métodos OnInspect e OnInspectLook serão gerados automaticamente

[CreateAssetMenu(fileName = "InputReader", menuName = "Inputs/InputReader")]
public class InputReader : ScriptableObject, IMovementActions, IMenusActions
{
    public event Action<Vector2> Movement;
    public event Action<Vector2> Aim;
    public event Action Jump;
    public event Action<bool> Sprint;
    public event Action Crouch;
    public event Action<bool> CrouchPerformed;
    public event Action<bool> CameraZoom;

    public event Action Throw;
    public event Action Interact;
    public event Action Grab;
    public event Action Inspect;
    public event Action SecondInteract;
    public event Action<bool> MouseIsDown;

    public event Action PauseClick;

    private Vector2 _inspectLookDelta;
    public Vector2 LookDelta => _inspectLookDelta;

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
        Debug.Log("potato");
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
        CrouchPerformed?.Invoke(context.ReadValueAsButton());
        if (context.performed) Crouch?.Invoke();
    }

    public void OnCameraZoom(InputAction.CallbackContext context)
    {
        CameraZoom?.Invoke(context.ReadValueAsButton());
    }

    void IMovementActions.OnInspect(InputAction.CallbackContext context)
    {
        if (context.performed) Inspect?.Invoke();
    }

    void IMovementActions.OnInspectLook(InputAction.CallbackContext context)
    {
        _inspectLookDelta = context.ReadValue<Vector2>();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed) PauseClick?.Invoke();
    }

    public void OnMouseDown(InputAction.CallbackContext context)
    {
        MouseIsDown?.Invoke(context.performed);
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



    public string InteractKey => GetBindingDisplayString(controls?.Movement.Interact);
    public string GrabKey => GetBindingDisplayString(controls?.Movement.Grab);
    public string ThrowKey => GetBindingDisplayString(controls?.Movement.Throw);
    public string InspectKey => GetBindingDisplayString(controls?.Movement.Inspect);
}