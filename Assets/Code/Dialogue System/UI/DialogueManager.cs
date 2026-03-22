using System;
using System.Collections;
using UnityEngine;

namespace FancyCrab.DialogueSystem
{
    /// <summary>
    /// Gere o estado e fluxo do diálogo. Não tem referências de UI nem de input.
    /// Comunica com a UI através de eventos.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        [Header("Settings")]
        public bool autoAdvance = false;
        public float autoAdvanceDelay = 3f;

        // Estado interno
        private DialogueContainer currentDialogue;
        private DialogueNode currentNode;
        private DialogueTrigger currentTrigger;
        private bool isAdvancing = false;
        private Coroutine autoAdvanceCoroutine;

        // Eventos públicos — a UI subscreve estes
        public event Action<DialogueContainer> OnDialogueStarted;
        public event Action<DialogueContainer> OnDialogueEnded;
        public event Action<DialogueNode> OnNodeChanged;
        public event Action<DialogueNode, string, string> OnTextNodeDisplayed; // (node, actorName, text)
        public event Action<ChoiceDialogueNode> OnChoiceNodeDisplayed;
        public event Action OnTypingSkipped;

        public static DialogueManager Instance { get; private set; }

        public bool IsDialogueActive => currentDialogue != null;
        public DialogueNode CurrentNode => currentNode;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void StartDialogue(DialogueContainer dialogue, Action onComplete = null, DialogueTrigger trigger = null, DialogueNode specificStartNode = null)
        {
            if (dialogue == null)
            {
                Debug.LogError("[FancyCrabStudios] Attempted to start null dialogue!");
                return;
            }

            currentDialogue = dialogue;
            currentTrigger = trigger;

            if (onComplete != null)
            {
                OnDialogueEnded += WrapOnComplete(onComplete);
            }

            var startNode = specificStartNode != null ? specificStartNode : dialogue.startNode;

            if (startNode == null)
            {
                Debug.LogError("[FancyCrabStudios] No start node found for dialogue!");
                return;
            }

            OnDialogueStarted?.Invoke(dialogue);
            DisplayNode(startNode);
        }

        /// <summary>
        /// Chamado pelo DialogueInputHandler quando o jogador carrega em continuar.
        /// </summary>
        public void HandleContinueInput()
        {
            if (currentNode is not TextDialogueNode textNode) return;

            if (isAdvancing)
            {
                isAdvancing = false;
                OnTypingSkipped?.Invoke();
                return;
            }

            if (textNode.nextNode != null)
            {
                DisplayNode(textNode.nextNode);
            }
            else
            {
                EndDialogue();
            }
        }

        /// <summary>
        /// Chamado pela UI quando o jogador escolhe uma opção.
        /// </summary>
        public void SelectChoice(DialogueNode nextNode)
        {
            if (autoAdvanceCoroutine != null)
            {
                StopCoroutine(autoAdvanceCoroutine);
                autoAdvanceCoroutine = null;
            }

            if (nextNode != null)
            {
                DisplayNode(nextNode);
            }
            else
            {
                EndDialogue();
            }
        }

        private void DisplayNode(DialogueNode node)
        {
            if (node == null)
            {
                EndDialogue();
                return;
            }

            if (currentNode != null)
            {
                currentNode.onNodeExit?.Invoke();
                InvokeLegacyTrigger(currentNode.onNodeExitEvent);
            }

            currentNode = node;

            currentNode.onNodeEnter?.Invoke();
            InvokeLegacyTrigger(currentNode.onNodeEnterEvent);

            OnNodeChanged?.Invoke(node);

            if (node is TextDialogueNode textNode)
            {
                ProcessTextNode(textNode);
            }
            else if (node is ChoiceDialogueNode choiceNode)
            {
                ProcessChoiceNode(choiceNode);
            }
            else if (node is IndexDialogueNode indexNode)
            {
                ProcessIndexNode(indexNode);
            }
        }

        private void ProcessTextNode(TextDialogueNode textNode)
        {
            isAdvancing = true;

            string actorName = textNode.actor != null ? textNode.actor.actorName : string.Empty;
            OnTextNodeDisplayed?.Invoke(textNode, actorName, textNode.dialogueText);

            if (!autoAdvance || textNode.nextNode == null) return;

            if (autoAdvanceCoroutine != null)
            {
                StopCoroutine(autoAdvanceCoroutine);
            }

            autoAdvanceCoroutine = StartCoroutine(AutoAdvanceRoutine(textNode.nextNode));
        }

        private void ProcessChoiceNode(ChoiceDialogueNode choiceNode)
        {
            isAdvancing = false;
            OnChoiceNodeDisplayed?.Invoke(choiceNode);
        }

        private void ProcessIndexNode(IndexDialogueNode indexNode)
        {
            if (indexNode.nextNode != null)
            {
                DisplayNode(indexNode.nextNode);
                return;
            }

            Debug.LogWarning($"[FancyCrabStudios] Index node {indexNode.indexValue} has no next node. Ending dialogue.");
            EndDialogue();
        }

        private void EndDialogue()
        {
            if (currentNode != null)
            {
                currentNode.onNodeExit?.Invoke();
                InvokeLegacyTrigger(currentNode.onNodeExitEvent);
            }

            var endedDialogue = currentDialogue;

            currentDialogue = null;
            currentNode = null;
            currentTrigger = null;
            isAdvancing = false;

            OnDialogueEnded?.Invoke(endedDialogue);
        }

        private void InvokeLegacyTrigger(string triggerEvent)
        {
            if (!string.IsNullOrEmpty(triggerEvent) && currentTrigger != null)
            {
                currentTrigger.InvokeCustomTrigger(triggerEvent);
            }
        }

        private IEnumerator AutoAdvanceRoutine(DialogueNode nextNode)
        {
            yield return new WaitForSeconds(autoAdvanceDelay);
            autoAdvanceCoroutine = null;
            DisplayNode(nextNode);
        }

        // Wrapper para remover o callback do evento após ser invocado uma vez
        private Action<DialogueContainer> WrapOnComplete(Action onComplete)
        {
            Action<DialogueContainer> wrapper = null;
            wrapper = _ =>
            {
                onComplete.Invoke();
                OnDialogueEnded -= wrapper;
            };
            return wrapper;
        }
    }
}