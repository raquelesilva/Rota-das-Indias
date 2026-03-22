using UnityEngine;
using UnityEngine.Events;
namespace FancyCrab.CoreSystems.InteractionSystem
{
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool canInteract = true;
        [SerializeField] private UnityEvent onInteract;

        public void OnInteract()
        {
            if (!canInteract) return;
            onInteract?.Invoke();
        }

        public bool CanInteract()
        {
            return canInteract;
        }

        public void SetCanInteract(bool value)
        {
            canInteract = value;
        }
    }
}