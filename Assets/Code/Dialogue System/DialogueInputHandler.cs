using UnityEngine;

namespace FancyCrab.DialogueSystem
{
    /// <summary>
    /// Trata exclusivamente do input do jogador durante o diálogo.
    /// Separado da UI e da lógica para facilitar troca de input system.
    /// </summary>
    public class DialogueInputHandler : MonoBehaviour
    {
        [Header("Input Settings")]
        public KeyCode continueKey = KeyCode.Space;
        public bool skipTypingOnClick = true;

        private void Update()
        {
            if (DialogueManager.Instance == null || !DialogueManager.Instance.IsDialogueActive) return;

            if (Input.GetKeyDown(continueKey))
            {
                DialogueManager.Instance.HandleContinueInput();
            }
        }
    }
}
