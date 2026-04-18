using UnityEngine;
using UnityEngine.InputSystem;

public class DragAstrolabio : MonoBehaviour
{
    #region Variables
    private Vector3 offset;
    private Camera cam;
    private bool isDragging = false;
    private GameObject draggedObject;
    private Vector3 originalPosition;
    private Transform originalParent;
    private Collider dragCollider;
    #endregion

    private void Start()
    {
        cam = Camera.main;
        dragCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePos);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.CompareTag("Draggable"))
                {
                    draggedObject = hit.collider.gameObject;
                    originalPosition = draggedObject.transform.position;
                    originalParent = draggedObject.transform.parent;
                    isDragging = true;
                    offset = draggedObject.transform.position - MouseWorldPosition(mousePos, draggedObject);
                }
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging && draggedObject != null)
        {
            Ray releaseRay = cam.ScreenPointToRay(mousePos);
            bool snapped = false;

            if (Physics.Raycast(releaseRay, out RaycastHit releaseHit))
            {
                if (releaseHit.collider.gameObject.CompareTag("SnapZone"))
                {
                    SnapZone snapZone = releaseHit.collider.gameObject.GetComponent<SnapZone>();
                    DraggableID draggableID = draggedObject.GetComponent<DraggableID>();

                    if (snapZone != null && draggableID != null && snapZone.acceptedID == draggableID.id)
                    {
                        // Correct snap zone
                        draggedObject.transform.SetParent(releaseHit.collider.gameObject.transform);
                        draggedObject.transform.position = releaseHit.collider.gameObject.transform.position;
                        Debug.Log(draggedObject.name + " snapped correctly to " + releaseHit.collider.gameObject.name);
                        snapped = true;

                        dragCollider.enabled = false;
                    }
                }
            }

            if (!snapped)
            {
                ReturnToOriginal();
            }

            isDragging = false;
            draggedObject = null;
        }

        if (isDragging && draggedObject != null)
        {
            draggedObject.transform.position = MouseWorldPosition(mousePos, draggedObject) + offset;
        }
    }

    private void ReturnToOriginal()
    {
        Debug.Log(draggedObject.name + " returned to original position");
        draggedObject.transform.position = originalPosition;
        draggedObject.transform.SetParent(originalParent);
    }

    private Vector3 MouseWorldPosition(Vector2 mouseScreenPos, GameObject target)
    {
        Vector3 screenPos = new(
            mouseScreenPos.x,
            mouseScreenPos.y,
            cam.WorldToScreenPoint(target.transform.position).z
        );
        return cam.ScreenToWorldPoint(screenPos);
    }
}