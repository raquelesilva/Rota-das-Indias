using UnityEngine;
using NaughtyAttributes;

namespace FancyCrab.CustomPackages.FirstPersonController
{
    public class CameraController : MonoBehaviour
    {
        #region Serialized Fields

        [BoxGroup("Input"), SerializeField]
        private InputReader inputReader;

        [BoxGroup("References"), SerializeField]
        private FirstPersonController fpc;

        [BoxGroup("Camera Movement")]
        public Camera playerCamera;

        [BoxGroup("Camera Movement"), SerializeField] public float fov = 60f;
        [BoxGroup("Camera Movement"), SerializeField] public bool invertCamera = false;
        [BoxGroup("Camera Movement"), SerializeField] public float mouseSensitivity = 2f;
        [BoxGroup("Camera Movement"), SerializeField] public float maxLookAngle = 85f;

        [BoxGroup("Camera Movement/Zoom")] public bool enableZoom = true;
        [BoxGroup("Camera Movement/Zoom"), ShowIf(nameof(enableZoom)), SerializeField] public bool holdToZoom = false;
        [BoxGroup("Camera Movement/Zoom"), ShowIf(nameof(enableZoom)), SerializeField] public KeyCode zoomKey = KeyCode.Mouse1;
        [BoxGroup("Camera Movement/Zoom"), ShowIf(nameof(enableZoom)), SerializeField] public float zoomFOV = 30f;
        [BoxGroup("Camera Movement/Zoom"), ShowIf(nameof(enableZoom)), SerializeField] public float zoomStepTime = 5f;

        [BoxGroup("Head Bob"), SerializeField] public bool enableHeadBob = true;
        [BoxGroup("Head Bob"), ShowIf(nameof(enableHeadBob)), SerializeField] public Transform joint;
        [BoxGroup("Head Bob"), ShowIf(nameof(enableHeadBob)), SerializeField] public float bobSpeed = 10f;
        [BoxGroup("Head Bob"), ShowIf(nameof(enableHeadBob)), SerializeField] public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

        #endregion

        #region Private Fields

        private Vector2 _aim;
        private float _yaw;
        private float _pitch;
        private bool _cameraZoomPressed;
        private bool _isZoomed;
        private float _bobTimer;
        private Vector3 _jointOriginalPos;

        private bool _isSprinting;
        private bool _isWalking;
        private bool _isCrouched;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            if (playerCamera != null)
            {
                playerCamera.fieldOfView = fov;
            }

            if (joint != null)
            {
                _jointOriginalPos = joint.localPosition;
            }

            if (fpc != null)
            {
                _yaw = fpc.transform.localEulerAngles.y;
                _pitch = playerCamera != null ? playerCamera.transform.localEulerAngles.x : 0f;
            }
        }

        private void OnEnable()
        {
            if (inputReader == null) return;
            inputReader.Aim += OnAim;
            inputReader.CameraZoom += OnCameraZoom;
        }

        private void OnDisable()
        {
            if (inputReader == null) return;
            inputReader.Aim -= OnAim;
            inputReader.CameraZoom -= OnCameraZoom;
        }

        private void Update()
        {
            if (!CanMove()) return;

            CacheFPCState();
            HandleRotation();

            if (enableZoom && playerCamera != null)
            {
                HandleZoom();
            }

            if (enableHeadBob)
            {
                HandleHeadBob();
            }
        }

        #endregion

        #region Input Handlers

        private void OnAim(Vector2 vector) { _aim = vector; }
        private void OnCameraZoom(bool state) { _cameraZoomPressed = state; }

        #endregion

        #region Camera Logic

        private void CacheFPCState()
        {
            if (fpc == null) return;
            _isSprinting = fpc.IsSprinting;
            _isWalking = fpc.IsWalking;
            _isCrouched = fpc.IsCrouched;
        }

        private void HandleRotation()
        {
            _yaw += _aim.x * mouseSensitivity;
            _pitch += mouseSensitivity * _aim.y * (invertCamera ? 1f : -1f);
            _pitch = Mathf.Clamp(_pitch, -maxLookAngle, maxLookAngle);

            fpc.transform.localEulerAngles = new Vector3(0f, _yaw, 0f);
            playerCamera.transform.localEulerAngles = new Vector3(_pitch, 0f, 0f);
        }

        private void HandleZoom()
        {
            if (inputReader != null)
            {
                if (!holdToZoom && _cameraZoomPressed && !_isSprinting)
                {
                    _isZoomed = !_isZoomed;
                }
                else if (holdToZoom && !_isSprinting)
                {
                    _isZoomed = _cameraZoomPressed;
                }
            }
            else
            {
                if (!holdToZoom && !_isSprinting)
                {
                    if (Input.GetKeyDown(zoomKey))
                    {
                        _isZoomed = !_isZoomed;
                    }
                }
                else if (holdToZoom && !_isSprinting)
                {
                    if (Input.GetKeyDown(zoomKey))
                    {
                        _isZoomed = true;
                    }
                    else if (Input.GetKeyUp(zoomKey))
                    {
                        _isZoomed = false;
                    }
                }
            }

            float targetFOV = (_isZoomed && !_isSprinting) ? zoomFOV : fov;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, zoomStepTime * Time.deltaTime);
        }

        private void HandleHeadBob()
        {
            if (!_isWalking)
            {
                _bobTimer = 0f;
                if (joint != null)
                {
                    joint.localPosition = Vector3.Lerp(
                        joint.localPosition,
                        _jointOriginalPos,
                        Time.deltaTime * bobSpeed
                    );
                }
                return;
            }

            float speed = bobSpeed;
            if (_isSprinting && fpc != null)
            {
                speed += fpc.SprintSpeed;
            }
            else if (_isCrouched && fpc != null)
            {
                speed *= fpc.SpeedReduction;
            }

            _bobTimer += Time.deltaTime * speed;

            if (joint != null)
            {
                joint.localPosition = new Vector3(
                    _jointOriginalPos.x,
                    _jointOriginalPos.y + Mathf.Sin(_bobTimer) * bobAmount.y,
                    _jointOriginalPos.z
                );
            }
        }

        #endregion

        #region Helpers

        private bool CanMove() => fpc != null && fpc.CanMove;

        #endregion
    }
}