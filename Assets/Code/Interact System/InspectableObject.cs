using UnityEngine;
using UnityEngine.Events;

namespace FancyCrab.CoreSystems.InteractionSystem
{
    public class InspectableObject : MonoBehaviour, IInspectable
    {
        [SerializeField] private bool canInspect = true;

        [Header("Inspect Settings")]
        [SerializeField] private Vector3 inspectPositionOffset = Vector3.zero;
        [SerializeField] private Vector3 inspectRotationOffset = Vector3.zero;
        [SerializeField] private float inspectDistance = 0.5f;

        [Header("Events")]
        [SerializeField] private UnityEvent onInspect;
        [SerializeField] private UnityEvent onInspectEnd;

        public bool CanInspect() => canInspect;
        public void SetCanInspect(bool value) => canInspect = value;

        public Vector3 InspectPositionOffset => inspectPositionOffset;
        public Vector3 InspectRotationOffset => inspectRotationOffset;
        public float InspectDistance => inspectDistance;

        public void OnInspect()
        {
            if (!canInspect) return;
            onInspect?.Invoke();
        }

        public void OnInspectEnd()
        {
            if (!canInspect) return;
            onInspectEnd?.Invoke();
        }
    }
}
