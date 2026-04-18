using TMPro;
using UnityEngine;

namespace FancyCrab.CoreSystems.InteractionSystem
{
    public class InteractableLabel : MonoBehaviour
    {
        [Header("Messages")]
        [SerializeField, TextArea(1, 3)] private string msgInteract = "Press {0} to Interact";
        [SerializeField, TextArea(1, 3)] private string msgGrab = "Press {0} to Grab";
        [SerializeField, TextArea(1, 3)] private string msgThrow = "Press {0} to Throw";
        [SerializeField, TextArea(1, 3)] private string msgDrop = "Press {0} to Drop";
        [SerializeField, TextArea(1, 3)] private string msgInspect = "Press {0} to Inspect";
        [SerializeField, TextArea(1, 3)] private string msgInspectExit = "Press {0} to Stop Inspecting";

        [Header("References")]
        [SerializeField] private InputReader inputReader;
        [SerializeField] private TextMeshProUGUI labelText;

        private void OnEnable()
        {
            PlayerInteraction.OnDetectInterface += OnDetect;
            PlayerInteraction.OnClearDetection += OnClear;
            PlayerInteraction.OnInteractionUpdate += OnInteractionUpdate;
        }

        private void OnDisable()
        {
            PlayerInteraction.OnDetectInterface -= OnDetect;
            PlayerInteraction.OnClearDetection -= OnClear;
            PlayerInteraction.OnInteractionUpdate -= OnInteractionUpdate;
        }

        private void OnDetect(RaycastState state)
        {
            string interactKey = inputReader != null ? inputReader.InteractKey : "?";
            string grabKey = inputReader != null ? inputReader.GrabKey : "?";
            string inspectKey = inputReader != null ? inputReader.InspectKey : "?";

            string interactLine = string.Format(msgInteract, interactKey);
            string grabLine = string.Format(msgGrab, grabKey);
            string inspectLine = string.Format(msgInspect, inspectKey);

            labelText.text = state switch
            {
                RaycastState.Interactable => interactLine,
                RaycastState.Grabbable => grabLine,
                RaycastState.Both => $"{interactLine}\n{grabLine}",
                RaycastState.Inspectable => inspectLine,
                _ => string.Empty
            };
        }

        private void OnClear()
        {
            labelText.text = string.Empty;
        }

        private void OnInteractionUpdate(InteractionState state)
        {
            if (state == InteractionState.Holding)
            {
                string throwLine = string.Format(msgThrow, inputReader != null ? inputReader.ThrowKey : "?");
                string dropLine = string.Format(msgDrop, inputReader != null ? inputReader.GrabKey : "?");
                labelText.text = $"{throwLine}\n{dropLine}";
                return;
            }

            if (state == InteractionState.Inspecting)
            {
                string inspectKey = inputReader != null ? inputReader.InspectKey : "?";
                labelText.text = string.Format(msgInspectExit, inspectKey);
                return;
            }

            if (state == InteractionState.None)
            {
                labelText.text = string.Empty;
            }
        }
    }
}