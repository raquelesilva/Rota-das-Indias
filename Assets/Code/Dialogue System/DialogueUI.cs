using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FancyCrab.DialogueSystem
{
    /// <summary>
    /// Responsável por toda a apresentação visual do diálogo.
    /// Subscreve os eventos do DialogueManager e não tem lógica de estado.
    /// </summary>
    public class DialogueUI : MonoBehaviour
    {
        [Header("Panel")]
        public GameObject dialoguePanel;

        [Header("Actor")]
        public TMP_Text actorNameText;
        public Image actorPortraitImage;

        [Header("Dialogue Text")]
        public TMP_Text dialogueText;

        [Header("Choices")]
        public Transform choicesContainer;
        public GameObject choiceButtonPrefab;

        [Header("Continue Indicator")]
        public GameObject continueIndicator;

        [Header("Typing Effect")]
        public float typingSpeed = 0.05f;
        public AudioClip typingSound;
        public AudioSource typingAudioSource;

        private Coroutine typingCoroutine;
        private string pendingFullText;

        private void OnEnable()
        {
            if (DialogueManager.Instance == null)
                return;

            DialogueManager.Instance.OnDialogueStarted += HandleDialogueStarted;
            DialogueManager.Instance.OnDialogueEnded += HandleDialogueEnded;
            DialogueManager.Instance.OnTextNodeDisplayed += HandleTextNodeDisplayed;
            DialogueManager.Instance.OnChoiceNodeDisplayed += HandleChoiceNodeDisplayed;
            DialogueManager.Instance.OnTypingSkipped += HandleTypingSkipped;
        }

        private void OnDisable()
        {
            if (DialogueManager.Instance == null)
                return;

            DialogueManager.Instance.OnDialogueStarted -= HandleDialogueStarted;
            DialogueManager.Instance.OnDialogueEnded -= HandleDialogueEnded;
            DialogueManager.Instance.OnTextNodeDisplayed -= HandleTextNodeDisplayed;
            DialogueManager.Instance.OnChoiceNodeDisplayed -= HandleChoiceNodeDisplayed;
            DialogueManager.Instance.OnTypingSkipped -= HandleTypingSkipped;
        }

        private void HandleDialogueStarted(DialogueContainer dialogue)
        {
            dialoguePanel.SetActive(true);
            ClearChoices();
            SetContinueIndicator(false);
        }

        private void HandleDialogueEnded(DialogueContainer dialogue)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            ClearChoices();
            SetContinueIndicator(false);
            dialoguePanel.SetActive(false);
        }

        private void HandleTextNodeDisplayed(DialogueNode node, string actorName, string text)
        {
            ClearChoices();
            choicesContainer.gameObject.SetActive(false);

            UpdateActorDisplay(node);

            actorNameText.text = actorName;
            pendingFullText = text;

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);

            SetContinueIndicator(false);
            typingCoroutine = StartCoroutine(TypeTextRoutine(text));
        }

        private void HandleChoiceNodeDisplayed(ChoiceDialogueNode choiceNode)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            dialogueText.text = "";
            UpdateActorDisplay(choiceNode);
            SetContinueIndicator(false);
            ClearChoices();
            choicesContainer.gameObject.SetActive(true);

            foreach (var choice in choiceNode.Choices)
            {
                if (string.IsNullOrEmpty(choice.choiceText))
                    continue;

                CreateChoiceButton(choice);
            }
        }

        private void HandleTypingSkipped()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            dialogueText.text = pendingFullText;
            SetContinueIndicator(true);
        }

        private void UpdateActorDisplay(DialogueNode node)
        {
            if (!node.HasActor || node.actor == null)
            {
                actorNameText.text = string.Empty;

                if (actorPortraitImage != null)
                    actorPortraitImage.gameObject.SetActive(false);

                return;
            }

            actorNameText.text = node.actor.actorName;

            if (actorPortraitImage != null)
            {
                actorPortraitImage.gameObject.SetActive(true);
                actorPortraitImage.sprite = node.actor.actorPortrait;
            }
        }

        private void CreateChoiceButton(ChoiceDialogueNode.ChoiceOption choice)
        {
            var buttonObj = Instantiate(choiceButtonPrefab, choicesContainer);
            var button = buttonObj.GetComponent<Button>();
            var buttonText = buttonObj.GetComponentInChildren<TMP_Text>();

            buttonText.text = choice.choiceText;

            button.onClick.AddListener(() =>
            {
                DialogueManager.Instance.SelectChoice(choice.nextNode);
            });

            ApplyChoiceButtonStyle(button);
        }

        private void ApplyChoiceButtonStyle(Button button)
        {
            var colors = button.colors;
            colors.highlightedColor = new Color(1f, 0.8f, 0.2f);
            button.colors = colors;
        }

        private void ClearChoices()
        {
            foreach (Transform child in choicesContainer)
            {
                Destroy(child.gameObject);
            }
        }

        private IEnumerator TypeTextRoutine(string text)
        {
            dialogueText.text = "";

            foreach (char letter in text)
            {
                dialogueText.text += letter;

                if (typingSound != null && typingAudioSource != null)
                {
                    typingAudioSource.PlayOneShot(typingSound);
                }

                yield return new WaitForSeconds(typingSpeed);
            }

            typingCoroutine = null;
            SetContinueIndicator(true);
        }

        private void SetContinueIndicator(bool visible)
        {
            if (continueIndicator != null)
            {
                continueIndicator.SetActive(visible);
            }
        }
    }
}