using System;
using UnityEngine;

namespace FancyCrab.CoreSystems.InteractionSystem
{
    public class InspectHandler : MonoBehaviour
    {
        [Header("Auto Rotation")]
        [SerializeField] private float autoRotateSpeed = 30f;
        [SerializeField] private Vector3 autoRotateAxis = Vector3.up;

        [Header("Manual Rotation")]
        [SerializeField] private float manualRotateSensitivity = 150f;
        [SerializeField] private float manualInputThreshold = 0.01f;

        [Header("Inspect Point")]
        [Tooltip("Transform que define onde o objeto aparece durante o inspect. Se vazio, usa a câmara com InspectDistance e InspectPositionOffset do InspectableObject.")]
        [SerializeField] private Transform inspectPoint;

        [Header("Smoothing")]
        [SerializeField] private float positionLerpSpeed = 12f;
        [SerializeField] private float rotationLerpSpeed = 12f;

        private Camera _camera;
        private Transform _inspectedTransform;
        private Rigidbody _inspectedRigidbody;
        private InspectableObject _inspectableObject;

        private Quaternion _baseRotation;
        private Quaternion _accumulatedRotation = Quaternion.identity;

        private bool _isInspecting;
        private bool _hasManualInput;

        private Vector3 _savedPosition;
        private Quaternion _savedRotation;
        private Transform _savedParent;
        private bool _savedUseGravity;
        private float _savedLinearDamping;
        private float _savedAngularDamping;
        private bool _savedIsKinematic;

        private Quaternion _pendingRotation;
        private bool _hasPendingRotation;

        public static event Action<bool> OnInspectStateChanged;

        public bool IsInspecting => _isInspecting;

        private void Awake()
        {
            _camera = Camera.main;
        }

        public void SetCamera(Camera cam)
        {
            _camera = cam;
        }

        public bool TryBeginInspect(Transform target, Rigidbody rb, InspectableObject inspectableObject)
        {
            if (_isInspecting) return false;
            if (target == null) return false;
            if (inspectPoint == null && _camera == null) return false;

            OnInspectStateChanged?.Invoke(true);

            _inspectedTransform = target;
            _inspectedRigidbody = rb;
            _inspectableObject = inspectableObject;

            _savedPosition = target.position;
            _savedRotation = target.rotation;
            _savedParent = target.parent;

            if (_inspectedRigidbody != null)
            {
                _savedUseGravity = _inspectedRigidbody.useGravity;
                _savedLinearDamping = _inspectedRigidbody.linearDamping;
                _savedAngularDamping = _inspectedRigidbody.angularDamping;
                _savedIsKinematic = _inspectedRigidbody.isKinematic;

                _inspectedRigidbody.useGravity = false;
                _inspectedRigidbody.isKinematic = true;
                _inspectedRigidbody.linearVelocity = Vector3.zero;
                _inspectedRigidbody.angularVelocity = Vector3.zero;
            }

            Vector3 rotOffset = inspectableObject != null ? inspectableObject.InspectRotationOffset : Vector3.zero;

            _baseRotation = Quaternion.Euler(rotOffset);
            _accumulatedRotation = Quaternion.identity;
            _hasManualInput = false;
            _hasPendingRotation = false;

            target.SetParent(null, true);
            _isInspecting = true;

            return true;
        }

        public void EndInspect()
        {
            OnInspectStateChanged?.Invoke(false);

            if (!_isInspecting) return;
            if (_inspectedTransform == null)
            {
                Reset();
                return;
            }

            if (_inspectedRigidbody != null)
            {
                _inspectedRigidbody.isKinematic = _savedIsKinematic;
                _inspectedRigidbody.useGravity = _savedUseGravity;
                _inspectedRigidbody.linearDamping = _savedLinearDamping;
                _inspectedRigidbody.angularDamping = _savedAngularDamping;
                _inspectedRigidbody.linearVelocity = Vector3.zero;
                _inspectedRigidbody.angularVelocity = Vector3.zero;
            }

            _inspectedTransform.SetParent(_savedParent, true);
            _inspectedTransform.position = _savedPosition;
            _inspectedTransform.rotation = _savedRotation;

            Reset();
        }

        private void Reset()
        {
            _inspectedTransform = null;
            _inspectedRigidbody = null;
            _inspectableObject = null;
            _accumulatedRotation = Quaternion.identity;
            _hasManualInput = false;
            _hasPendingRotation = false;
            _isInspecting = false;
        }

        public void UpdateInspect(Vector2 lookDelta)
        {
            if (!_isInspecting || _inspectedTransform == null) return;

            _hasManualInput = lookDelta.sqrMagnitude > manualInputThreshold * manualInputThreshold;

            if (_hasManualInput)
            {
                float rotX = -lookDelta.y * manualRotateSensitivity * Time.deltaTime;
                float rotY = -lookDelta.x * manualRotateSensitivity * Time.deltaTime;

                Quaternion deltaRot = Quaternion.Euler(rotX, rotY, 0f);
                _accumulatedRotation = deltaRot * _accumulatedRotation;
            }
            else
            {
                Quaternion autoRot = Quaternion.AngleAxis(autoRotateSpeed * Time.deltaTime, autoRotateAxis);
                _accumulatedRotation = autoRot * _accumulatedRotation;
            }

            _pendingRotation = Quaternion.Slerp(
                _inspectedTransform.rotation,
                _accumulatedRotation * _baseRotation,
                Time.deltaTime * rotationLerpSpeed
            );
            _hasPendingRotation = true;

            if (_inspectedRigidbody == null)
            {
                Vector3 targetPosition = ResolveTargetPosition(_inspectableObject);
                _inspectedTransform.position = Vector3.Lerp(
                    _inspectedTransform.position,
                    targetPosition,
                    Time.deltaTime * positionLerpSpeed
                );
                _inspectedTransform.rotation = _pendingRotation;
                _hasPendingRotation = false;
            }
        }

        public void FixedUpdateInspect()
        {
            if (!_isInspecting || _inspectedRigidbody == null || _inspectedTransform == null) return;

            Vector3 targetPosition = ResolveTargetPosition(_inspectableObject);
            _inspectedRigidbody.MovePosition(Vector3.Lerp(
                _inspectedTransform.position,
                targetPosition,
                Time.fixedDeltaTime * positionLerpSpeed
            ));

            if (_hasPendingRotation)
            {
                _inspectedRigidbody.MoveRotation(_pendingRotation);
                _hasPendingRotation = false;
            }
        }

        private Vector3 ResolveTargetPosition(InspectableObject inspectableObject)
        {
            if (inspectPoint != null) return inspectPoint.position;

            float distance = inspectableObject != null ? inspectableObject.InspectDistance : 0.5f;
            Vector3 offset = inspectableObject != null ? inspectableObject.InspectPositionOffset : Vector3.zero;

            return _camera.transform.position
                + _camera.transform.forward * distance
                + _camera.transform.TransformDirection(offset);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (inspectPoint == null) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(inspectPoint.position, 0.1f);
            Gizmos.DrawLine(transform.position, inspectPoint.position);
        }
#endif
    }
}