using UnityEngine;
using UnityEngine.InputSystem;

public class RotateAstrolabio : MonoBehaviour
{
    #region Variables
    [SerializeField] private float speed;
    [SerializeField] private bool inverted;
    [SerializeField] private float snapAngle = 45f;

    private bool isDragging = false;
    private float currentAngle = 0f;
    #endregion

    private void Start()
    {
        // Initialize currentAngle from the object's existing Y rotation
        currentAngle = transform.eulerAngles.y;
    }

    private void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isDragging = true;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (isDragging)
            {
                // Snap to nearest 45 degree increment on release
                currentAngle = Mathf.Round(currentAngle / snapAngle) * snapAngle;

                // Preserve X and Z rotation, only change Y
                transform.rotation = Quaternion.Euler(
                    transform.eulerAngles.x,
                    currentAngle,
                    transform.eulerAngles.z
                );
            }

            isDragging = false;
            Cursor.lockState = CursorLockMode.None;
        }

        if (isDragging)
        {
            float mouseX = Mouse.current.delta.x.ReadValue();
            float rotation = mouseX * speed * Time.deltaTime;
            currentAngle += inverted ? -rotation : rotation;

            // Keep angle between 0 and 360
            currentAngle = (currentAngle % 360f + 360f) % 360f;

            // Preserve X and Z rotation, only change Y
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                currentAngle,
                transform.eulerAngles.z
            );
        }
    }
}