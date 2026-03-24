using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragAstrolabio : MonoBehaviour
{
    #region Variables
    private Vector3 offset;
    private Camera camera;
    #endregion

    private void Start()
    {
        camera = Camera.main;
    }

    private void OnMouseDown()
    {
        Debug.Log("MouseDown");
        offset = transform.position - MouseWorldPosition();
        Debug.Log("transform.position: " + transform.position);
    }

    private void OnMouseDrag()
    {
        transform.position = MouseWorldPosition() + offset;
        Debug.Log("transform.position: " + transform.position);
    }

    protected virtual Vector3 MouseWorldPosition()
    {
        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.x = camera.WorldToScreenPoint(transform.position).x;
        return camera.ScreenToWorldPoint(mouseScreenPos);
    }

   /* #region Input Actions
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
    private Vector3 curScreenPos;

    private Camera camera;

    private Vector3 WorldPos
    {
        get
        {
            float z = camera.WorldToScreenPoint(transform.position).z;
            return camera.ScreenToWorldPoint(curScreenPos + new Vector3(0, 0, z));
        }
    }

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
        Debug.Log("MouseDown");
        if (context.started || context.performed)
        {
            moveAllowed = true;
            offset = transform.position - GetMouseLookInput();
            Debug.Log("offset: " + offset);
            //Cursor.lockState = CursorLockMode.Locked;
        }
        else if (context.canceled)
        {
            moveAllowed = false;
            //Cursor.lockState = CursorLockMode.None;
        }
    }

    protected virtual Vector3 GetMouseLookInput()
    {
        var mouseScreenPos = Input.mousePosition;
        Debug.Log("mouseScreenPos: " + mouseScreenPos);
        mouseScreenPos.z = camera.WorldToScreenPoint(transform.position).z;
        return camera.ScreenToWorldPoint(mouseScreenPos);
    }

    private void Update()
    {
        if (!moveAllowed) return;

        transform.position = WorldPos + offset;
    }

    private IEnumerator Drag()
    {
        moveAllowed = true;
        Vector3 offset = transform.position - WorldPos;
        // grab
        GetComponent<Rigidbody>().useGravity = false;
        while (moveAllowed)
        {
            // dragging
            transform.position = WorldPos + offset;
            yield return null;
        }
        // drop
        GetComponent<Rigidbody>().useGravity = true;
    }*/
}