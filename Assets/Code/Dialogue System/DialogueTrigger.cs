using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace FancyCrab.DialogueSystem
{
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Dialogue Settings")]
        public DialogueContainer dialogue;

        [Header("Start Options")]
        public int startIndex = -1;

        [Header("Events")]
        public UnityEvent onDialogueStart;
        public UnityEvent onDialogueEnd;

        [Header("Custom Trigger Events")]
        public List<StringEventPair> customTriggers = new List<StringEventPair>();

        [System.Serializable]
        public struct StringEventPair
        {
            public string triggerID;
            public UnityEvent onTrigger;
        }

        public void TriggerDialogue()
        {
            if (DialogueManager.Instance == null)
            {
                Debug.LogError("[FancyCrabStudios] No DialogueManager found in scene!");
                return;
            }

            onDialogueStart?.Invoke();

            DialogueNode startNode = null;

            if (startIndex >= 0 && dialogue != null)
            {
                startNode = dialogue.GetNodeByIndex(startIndex);
                if (startNode == null)
                {
                    Debug.LogWarning($"[FancyCrabStudios] Node with index {startIndex} not found. Using default start node.");
                }
            }

            DialogueManager.Instance.StartDialogue(dialogue, OnDialogueComplete, this, startNode);
        }

        public void TriggerDialogueFromIndex(int index)
        {
            if (DialogueManager.Instance == null || dialogue == null) return;

            onDialogueStart?.Invoke();

            var startNode = dialogue.GetNodeByIndex(index);
            if (startNode == null)
            {
                Debug.LogError($"[FancyCrabStudios] Node with index {index} not found!");
                return;
            }

            DialogueManager.Instance.StartDialogue(dialogue, OnDialogueComplete, this, startNode);
        }

        public void InvokeCustomTrigger(string triggerID)
        {
            foreach (var pair in customTriggers)
            {
                if (pair.triggerID == triggerID)
                {
                    pair.onTrigger?.Invoke();
                    return;
                }
            }

            Debug.LogWarning($"[FancyCrabStudios] No custom trigger found with ID: {triggerID}");
        }

        public void SetDialogue(DialogueContainer container)
        {
            dialogue = container;
        }

        public void SetDialogueAndTrigger(DialogueContainer container)
        {
            SetDialogue(container);
            TriggerDialogue();
        }

        private void OnDialogueComplete()
        {
            onDialogueEnd?.Invoke();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawIcon(transform.position, "DialogueTrigger Icon", true);
        }
    }
}