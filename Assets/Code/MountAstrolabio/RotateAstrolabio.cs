using UnityEngine;
using UnityEngine.InputSystem;

namespace AstrolabeSystem
{
    public class RotateAstrolabio : MonoBehaviour
    {
        #region Variables
        [SerializeField] private Camera _cam;
        [SerializeField] private float speed = 100f;
        [SerializeField] private bool inverted;
        [SerializeField] private float snapAngle = 45f;

        private bool _isDragging;
        private float _currentAngle;
        #endregion

        private void Start()
        {
            _currentAngle = transform.eulerAngles.y;
        }

        private void Update()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            if (Mouse.current.leftButton.wasPressedThisFrame)
                TryBeginRotate(mousePos);

            if (Mouse.current.leftButton.wasReleasedThisFrame && _isDragging)
                EndRotate();

            if (_isDragging)
                ApplyRotation();
        }

        private void TryBeginRotate(Vector2 mousePos)
        {
            Ray ray = _cam.ScreenPointToRay(mousePos);

            if (!Physics.Raycast(ray, out RaycastHit hit))
                return;

            if (hit.collider.gameObject != gameObject)
                return;

            _isDragging = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void EndRotate()
        {
            _currentAngle = Mathf.Round(_currentAngle / snapAngle) * snapAngle;

            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                _currentAngle,
                transform.eulerAngles.z
            );

            _isDragging = false;
            Cursor.lockState = CursorLockMode.None;
        }

        private void ApplyRotation()
        {
            float mouseX = Mouse.current.delta.x.ReadValue();
            float rotation = mouseX * speed * Time.deltaTime;
            _currentAngle += inverted ? -rotation : rotation;
            _currentAngle = (_currentAngle % 360f + 360f) % 360f;

            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                _currentAngle,
                transform.eulerAngles.z
            );
        }
    }
}