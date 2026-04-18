using System;
using UnityEngine;

namespace FancyCrab.CoreSystems.InteractionSystem
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Detect")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float detectDistance = 3f;
        [SerializeField] private LayerMask detectLayer = ~0;

        [Header("Input")]
        [SerializeField] private InputReader inputReader;

        [Header("Grab")]
        [SerializeField] private Transform grabPoint;
        [SerializeField] private float throwForce = 10f;

        [Header("Held Physics")]
        [SerializeField] private float holdForce = 80f;
        [SerializeField] private float holdDamping = 10f;
        [SerializeField] private float holdTorque = 80f;
        [SerializeField] private float holdAngularDamping = 10f;

        [Header("Inspect")]
        [SerializeField] private InspectHandler inspectHandler;

        public static event Action<RaycastState> OnDetectInterface;
        public static event Action<InteractionState> OnInteractionUpdate;
        public static event Action OnClearDetection;

        private IGrabbable currentGrabbable;
        private GrabbableObject currentPickupObject;
        private IInteractable currentInteractable;
        private InteractableObject currentInteractableObject;
        private IInspectable currentInspectable;
        private InspectableObject currentInspectableObject;
        private Rigidbody currentRigidbody;
        private Transform currentTransform;

        private IGrabbable heldGrabbable;
        private GrabbableObject heldPickupObject;
        private Rigidbody heldRigidbody;
        private Transform heldTransform;

        private bool heldOriginalUseGravity;
        private float heldOriginalDrag;
        private float heldOriginalAngularDrag;

        private RaycastState lastRaycastState = RaycastState.None;
        private InteractionState lastInteractionState = InteractionState.None;

        private void Awake()
        {
            if (playerCamera == null) playerCamera = Camera.main;
            if (inspectHandler != null) inspectHandler.SetCamera(playerCamera);
        }

        private void OnEnable()
        {
            if (inputReader == null) return;

            inputReader.Interact += OnInteractPressed;
            inputReader.Grab += OnGrabPressed;
            inputReader.Throw += OnThrowPressed;
            inputReader.Inspect += OnInspectPressed;
        }

        private void OnDisable()
        {
            if (inputReader == null) return;

            inputReader.Interact -= OnInteractPressed;
            inputReader.Grab -= OnGrabPressed;
            inputReader.Throw -= OnThrowPressed;
            inputReader.Inspect -= OnInspectPressed;
        }

        private void Update()
        {
            if (inspectHandler != null && inspectHandler.IsInspecting)
            {
                Vector2 lookDelta = inputReader != null ? inputReader.LookDelta : Vector2.zero;
                inspectHandler.UpdateInspect(lookDelta);
                return;
            }

            DetectTarget();
        }

        private void FixedUpdate()
        {
            UpdateHeldPhysics();
            inspectHandler?.FixedUpdateInspect();
        }

        private void OnInteractPressed()
        {
            if (heldGrabbable != null) return;
            if (inspectHandler != null && inspectHandler.IsInspecting) return;
            if (currentInteractable == null) return;

            bool canInteract = currentInteractableObject == null || currentInteractableObject.CanInteract();
            if (canInteract) currentInteractable.OnInteract();
        }

        private void OnGrabPressed()
        {
            if (inspectHandler != null && inspectHandler.IsInspecting) return;

            if (heldGrabbable != null)
            {
                DropHeld();
                return;
            }

            if (currentGrabbable == null) return;

            bool canGrab = currentPickupObject == null || currentPickupObject.CanGrab();
            if (canGrab) GrabCurrent();
        }

        private void OnThrowPressed()
        {
            if (heldGrabbable == null) return;
            if (inspectHandler != null && inspectHandler.IsInspecting) return;

            ThrowHeld();
        }

        private void OnInspectPressed()
        {
            if (inspectHandler == null) return;

            if (inspectHandler.IsInspecting)
            {
                ExitInspect();
                return;
            }

            if (heldGrabbable != null) return;
            if (currentInspectable == null) return;

            bool canInspect = currentInspectableObject == null || currentInspectableObject.CanInspect();
            if (!canInspect) return;

            bool started = inspectHandler.TryBeginInspect(currentTransform, currentRigidbody, currentInspectableObject);
            if (!started) return;

            currentInspectable.OnInspect();
            NotifyInteractionState(InteractionState.Inspecting);
            NotifyRaycastState(RaycastState.None);
        }

        private void ExitInspect()
        {
            if (inspectHandler == null) return;

            inspectHandler.EndInspect();
            currentInspectable?.OnInspectEnd();
            NotifyInteractionState(InteractionState.None);
        }

        private void DetectTarget()
        {
            if (heldGrabbable != null)
            {
                NotifyInteractionState(InteractionState.Holding);
                return;
            }

            NotifyInteractionState(InteractionState.None);

            currentGrabbable = null;
            currentPickupObject = null;
            currentInteractable = null;
            currentInteractableObject = null;
            currentInspectable = null;
            currentInspectableObject = null;
            currentRigidbody = null;
            currentTransform = null;

            if (playerCamera == null)
            {
                NotifyRaycastState(RaycastState.None);
                return;
            }

            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, detectDistance, detectLayer))
            {
                currentTransform = hit.collider.transform;
                currentRigidbody = hit.collider.attachedRigidbody;

                bool hasGrabbable = hit.collider.TryGetComponent(out IGrabbable grabbable);
                bool hasInteractable = hit.collider.TryGetComponent(out IInteractable interactable);
                bool hasInspectable = hit.collider.TryGetComponent(out IInspectable inspectable);

                if (hasGrabbable)
                {
                    currentGrabbable = grabbable;
                    currentPickupObject = hit.collider.GetComponent<GrabbableObject>();
                }

                if (hasInteractable)
                {
                    currentInteractable = interactable;
                    currentInteractableObject = hit.collider.GetComponent<InteractableObject>();
                }

                if (hasInspectable)
                {
                    currentInspectable = inspectable;
                    currentInspectableObject = hit.collider.GetComponent<InspectableObject>();
                }

                RaycastState newState = (hasGrabbable, hasInteractable, hasInspectable) switch
                {
                    (true, true, _) => RaycastState.Both,
                    (true, false, _) => RaycastState.Grabbable,
                    (false, true, _) => RaycastState.Interactable,
                    (false, false, true) => RaycastState.Inspectable,
                    _ => RaycastState.None
                };

                NotifyRaycastState(newState);
                return;
            }

            NotifyRaycastState(RaycastState.None);
        }

        private void NotifyRaycastState(RaycastState newState)
        {
            if (newState == lastRaycastState) return;

            lastRaycastState = newState;

            if (newState == RaycastState.None)
            {
                OnClearDetection?.Invoke();
                return;
            }

            OnDetectInterface?.Invoke(newState);
        }

        private void NotifyInteractionState(InteractionState newState)
        {
            if (newState == lastInteractionState) return;

            lastInteractionState = newState;
            OnInteractionUpdate?.Invoke(newState);
        }

        private void GrabCurrent()
        {
            if (grabPoint == null) return;
            if (currentTransform == null) return;

            heldGrabbable = currentGrabbable;
            heldPickupObject = currentPickupObject;
            heldRigidbody = currentRigidbody;
            heldTransform = currentTransform;

            if (heldRigidbody == null)
            {
                ClearHeld();
                return;
            }

            heldOriginalUseGravity = heldRigidbody.useGravity;
            heldOriginalDrag = heldRigidbody.linearDamping;
            heldOriginalAngularDrag = heldRigidbody.angularDamping;

            heldRigidbody.useGravity = false;
            heldRigidbody.linearDamping = Mathf.Max(heldOriginalDrag, 6f);
            heldRigidbody.angularDamping = Mathf.Max(heldOriginalAngularDrag, 6f);

            heldTransform.SetParent(null, true);

            heldGrabbable.OnGrab();
            NotifyInteractionState(InteractionState.Holding);
        }

        private void UpdateHeldPhysics()
        {
            if (heldRigidbody == null || grabPoint == null) return;

            Vector3 targetPos = grabPoint.position;
            Vector3 posError = targetPos - heldRigidbody.position;
            Vector3 accel = (posError * holdForce) - (heldRigidbody.linearVelocity * holdDamping);
            heldRigidbody.AddForce(accel, ForceMode.Acceleration);

            Quaternion targetRot = grabPoint.rotation;
            Quaternion rotError = targetRot * Quaternion.Inverse(heldRigidbody.rotation);

            rotError.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;

            if (!float.IsNaN(axis.x) && axis.sqrMagnitude > 0.0001f)
            {
                Vector3 torque = axis.normalized * (angle * Mathf.Deg2Rad * holdTorque) - (heldRigidbody.angularVelocity * holdAngularDamping);
                heldRigidbody.AddTorque(torque, ForceMode.Acceleration);
            }
        }

        private void DropHeld()
        {
            if (heldRigidbody == null)
            {
                ClearHeld();
                NotifyRaycastState(RaycastState.None);
                return;
            }

            RestoreHeldRigidbody();
            heldGrabbable?.OnDrop();

            ClearHeld();
            NotifyRaycastState(RaycastState.None);
        }

        private void ThrowHeld()
        {
            if (heldRigidbody == null)
            {
                ClearHeld();
                NotifyRaycastState(RaycastState.None);
                return;
            }

            RestoreHeldRigidbody();

            if (playerCamera != null)
            {
                heldRigidbody.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
            }

            heldGrabbable?.OnThrow();
            ClearHeld();
            NotifyRaycastState(RaycastState.None);
        }

        private void RestoreHeldRigidbody()
        {
            heldRigidbody.useGravity = heldOriginalUseGravity;
            heldRigidbody.linearDamping = heldOriginalDrag;
            heldRigidbody.angularDamping = heldOriginalAngularDrag;
        }

        private void ClearHeld()
        {
            heldGrabbable = null;
            heldPickupObject = null;
            heldRigidbody = null;
            heldTransform = null;
        }
    }
}