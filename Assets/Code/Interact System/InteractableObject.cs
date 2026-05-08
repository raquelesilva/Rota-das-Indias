using UnityEngine;
using UnityEngine.Events;
namespace FancyCrab.CoreSystems.InteractionSystem
{
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool canInteract = true;
        [SerializeField] private UnityEvent onInteract;
        [Header("Effects")]
        [SerializeField] private bool canUseOutline = true;

        private Outline outline;

        private void Awake()
        {
            if (canUseOutline)
            {
                outline = GetComponent<Outline>();
                if (outline == null)
                {
                    outline = gameObject.AddComponent<Outline>();
                    outline.enabled = canInteract;
                }
            }
        }

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
            if (canUseOutline)
            {
                if (outline != null)
                {
                    outline.enabled = value;
                }
            }
        }
    }
}