using UnityEngine;

namespace AstrolabeSystem
{
    [RequireComponent(typeof(Collider))]
    public class DragAstrolabio : MonoBehaviour, IDraggable
    {
        [Header("Config")]
        [SerializeField] private string draggableID;

        public string DraggableID => draggableID;

        private Vector3 _originalPosition;
        private Transform _originalParent;
        private Collider _collider;
        private bool _isSnapped;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        public void OnPickUp()
        {
            if (_isSnapped)
                return;

            _originalPosition = transform.position;
            _originalParent = transform.parent;
        }

        public void OnDrag(Vector3 worldPosition)
        {
            if (_isSnapped)
                return;

            transform.position = worldPosition;
        }

        public void OnDrop(bool snapped)
        {
            if (!snapped)
            {
                transform.position = _originalPosition;
                transform.SetParent(_originalParent);
            }
        }

        public void SnapTo(Transform snapTransform)
        {
            _isSnapped = true;
            transform.SetParent(snapTransform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            _collider.enabled = false;
        }
    }
}