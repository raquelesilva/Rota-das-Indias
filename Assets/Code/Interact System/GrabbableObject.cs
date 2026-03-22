using UnityEngine;
using UnityEngine.Events;

namespace FancyCrab.CoreSystems.InteractionSystem
{
    public class GrabbableObject : MonoBehaviour, IGrabbable
    {
        [SerializeField] private bool canGrab = true;

        [SerializeField] private UnityEvent onGrab;
        [SerializeField] private UnityEvent onDrop;
        [SerializeField] private UnityEvent onThrow;

        public void OnGrab()
        {
            if (!canGrab) return;
            onGrab?.Invoke();
        }

        public void OnDrop()
        {
            if (!canGrab) return;
            onDrop?.Invoke();
        }

        public void OnThrow()
        {
            if (!canGrab) return;
            onThrow?.Invoke();
        }

        public bool CanGrab()
        {
            return canGrab;
        }

        public void SetCanGrab(bool value)
        {
            canGrab = value;
        }
    }
}
