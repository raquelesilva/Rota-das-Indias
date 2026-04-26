using CoreSystems.Managers;
using FancyCrab.CustomPackages.FirstPersonController;
using FancyCrab.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class InputGame : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] GameObject gameUI;
        [SerializeField] List<Answers> allAnswers = new();

        [Header("NPC")]
        [SerializeField] DialogueTrigger npc;
        [SerializeField] DialogueContainer nextDialogue;

        public void StartGame()
        {
            StartCoroutine(DelayStartGame());
        }

        IEnumerator DelayStartGame()
        {
            yield return new WaitForSeconds(.5f);

            gameUI.SetActive(true);
            PlayerStateHandler.Instance.SetPlayerState(PlayerStates.Paused);
        }

        public void SetCurrentAnswer(TMP_InputField currentInput)
        {
            foreach (var answer in allAnswers)
            {
                if (currentInput == answer.inputField)
                {
                    answer.currentAnswer = currentInput.text;
                }
            }
        }

        public void CheckAnswer()
        {
            int correct = 0;

            foreach (var answer in allAnswers)
            {
                if (answer.currentAnswer.ToLower().Trim() == answer.correctAnswer.ToLower().Trim())
                {
                    answer.inputField.image.color = Color.green;
                    correct++;
                }
                else
                {
                    answer.inputField.image.color = Color.red;
                }
            }

            if (correct == allAnswers.Count)
            {
                NotificationManager.instance.SetMessage("Boa conseguiste!", Color.green, "win");
                CloseGame();
            }
            else if (correct == 0)
            {
                NotificationManager.instance.SetMessage("Revê todas as tuas respostas!", Color.red, "lose");
            }
            else
            {
                NotificationManager.instance.SetMessage("Revê algumas das tuas respostas!", Color.yellow, "lose");
            }
        }

        public void CloseGame()
        {
            npc.SetDialogueAndTrigger(nextDialogue);
            gameUI.SetActive(false);
            PlayerStateHandler.Instance.SetPlayerState(PlayerStates.Playing);
        }
    }
}

[Serializable]
public class Answers
{
    public TMP_InputField inputField;
    public string correctAnswer;
    public string currentAnswer;

    public Answers(TMP_InputField inputField, string correctAnswer, string currentAnswer)
    {
        this.inputField = inputField;
        this.correctAnswer = correctAnswer;
        this.currentAnswer = currentAnswer;
    }
}