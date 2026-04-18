using UnityEngine;
using UnityEngine.InputSystem;

namespace AstrolabeSystem
{
    public class DragInputHandler : MonoBehaviour
    {
        public static DragInputHandler Instance { get; private set; }

        [Header("Config")]
        [SerializeField] private LayerMask draggableLayer;
        [SerializeField] private LayerMask snapZoneLayer;

        private Camera _cam;
        private IDraggable _currentDraggable;
        private Vector3 _dragOffset;
        private bool _isDragging;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _cam = Camera.main;
        }

        private void Update()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = _cam.ScreenPointToRay(mousePos);

            if (Mouse.current.leftButton.wasPressedThisFrame)
                TryBeginDrag(ray, mousePos);

            if (_isDragging)
                UpdateDrag(mousePos);

            if (Mouse.current.leftButton.wasReleasedThisFrame && _isDragging)
                EndDrag(ray);
        }

        private void TryBeginDrag(Ray ray, Vector2 mousePos)
        {
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, draggableLayer))
                return;

            if (!hit.collider.TryGetComponent(out IDraggable draggable))
                return;

            _currentDraggable = draggable;
            _isDragging = true;

            var targetTransform = hit.collider.transform;
            _dragOffset = targetTransform.position - ScreenToWorld(mousePos, targetTransform.position);

            _currentDraggable.OnPickUp();
        }

        private void UpdateDrag(Vector2 mousePos)
        {
            var targetTransform = (_currentDraggable as MonoBehaviour)?.transform;
            if (targetTransform == null)
                return;

            Vector3 worldPos = ScreenToWorld(mousePos, targetTransform.position) + _dragOffset;
            _currentDraggable.OnDrag(worldPos);
        }

        private void EndDrag(Ray ray)
        {
            bool snapped = TrySnap(ray);
            _currentDraggable.OnDrop(snapped);

            _isDragging = false;
            _currentDraggable = null;
        }

        private bool TrySnap(Ray ray)
        {
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, snapZoneLayer))
                return false;

            if (!hit.collider.TryGetComponent(out ISnappable snapZone))
                return false;

            if (snapZone.IsOccupied)
                return false;

            if (snapZone.AcceptedID != _currentDraggable.DraggableID)
                return false;

            snapZone.OnSnap(_currentDraggable);
            return true;
        }

        private Vector3 ScreenToWorld(Vector2 screenPos, Vector3 worldReference)
        {
            Vector3 pos = new(
                screenPos.x,
                screenPos.y,
                _cam.WorldToScreenPoint(worldReference).z
            );
            return _cam.ScreenToWorldPoint(pos);
        }
    }
}