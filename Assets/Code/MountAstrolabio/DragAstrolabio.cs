using UnityEngine;
using UnityEngine.InputSystem;

public class DragAstrolabio : MonoBehaviour
{
    #region Input Actions
    [SerializeField]
    private InputActionAsset inputActions;

    public InputActionAsset actions
    {
        get => inputActions;
        set => inputActions = value;
    }

    protected InputAction leftClickInputAction { get; set; }

    protected InputAction mouseLookInputAction { get; set; }

    #endregion

    #region Variables

    private bool moveAllowed;

    private Camera camera;

    [SerializeField] private float speed;

    [SerializeField] private bool inverted;

    #endregion

    private void Awake()
    {
        InitializeInputSystem();
    }

    private void Start()
    {
        camera = Camera.main;
    }

    private void InitializeInputSystem()
    {
        leftClickInputAction = actions.FindAction("Left Click");
        if (leftClickInputAction != null)
        {
            leftClickInputAction.started += OnLeftClickPressed;
            leftClickInputAction.performed += OnLeftClickPressed;
            leftClickInputAction.canceled += OnLeftClickPressed;
        }

        mouseLookInputAction = actions.FindAction("Mouse Look");

        actions.Enable();
    }

    protected virtual void OnLeftClickPressed(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            moveAllowed = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (context.canceled)
        {
            moveAllowed = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    protected virtual Vector2 GetMouseLookInput()
    {
        if (mouseLookInputAction != null)
        {
            return mouseLookInputAction.ReadValue<Vector2>();
        }

        return Vector2.zero;
    }

    private void Update()
    {
        //if (!moveAllowed) return;

        //Vector2 MouseDelta = GetMouseLookInput();

        //MouseDelta *= speed * Time.deltaTime;
        
        //transform.Translate(Vector3.right * (inverted ? 1 : -1), Space.Self);
        //transform.Translate(Vector3.up * (inverted ? 1 : -1), Space.Self);
    }
}