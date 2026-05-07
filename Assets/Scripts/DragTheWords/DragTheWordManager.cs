using CoreSystems.Managers;
using FancyCrab.CustomPackages.FirstPersonController;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DragTheWordManager : MonoBehaviour
{
    [Header("Slots & Draggables")]
    [SerializeField] private DropSlot[] allSlots;
    [SerializeField] private DraggableWord[] allDraggables;

    [Header("Messages")]
    [SerializeField] private string onWinMessage = "Parabéns! Acertas-te em todas as palavras!";

    [Header("Events")]
    [SerializeField] private UnityEvent onExerciseStart;
    [SerializeField] private UnityEvent onExerciseEnd;

    [Header("Colors")]
    public Color32 baseDraggableColor;
    public Color32 correctColor;
    public Color32 wrongColor;

    [Header("References")]
    [SerializeField] private GameObject exerciseParent;
    [SerializeField] private Button validationButton;

    private void OnEnable()
    {
        validationButton?.onClick.AddListener(CheckAnswers);
    }
    private void OnDisable()
    {
        validationButton?.onClick.RemoveListener(CheckAnswers);
    }

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    public IEnumerator StartGameRoutine()
    {
        yield return new WaitForSeconds(.2f);
        PlayerStateHandler.Instance.SetPlayerState(PlayerStates.Paused);
        BlockAllDraggables(false);
        exerciseParent.SetActive(true);
        onExerciseStart?.Invoke();
    }

    private IEnumerator EndGame()
    {
        NotificationManager.instance.SetCorrectMessage(onWinMessage);
        BlockAllDraggables(true);
        yield return new WaitForSeconds(3);
        PlayerStateHandler.Instance.SetPlayerState(PlayerStates.Playing);
        exerciseParent.SetActive(false);
        onExerciseEnd?.Invoke();
    }

    public void BlockAllDraggables(bool block)
    {
        foreach (var draggable in allDraggables)
        {
            draggable.SetInteractable(!block);
        }
    }   

    public void CheckAnswers()
    {
        int correct = 0;

        foreach (var slot in allSlots)
        {
            slot.PropagateColor(this);
            if (slot.IsCorrect()) correct++;
        }

        if (correct == allSlots.Length)
        {
            StartCoroutine(EndGame());
        }
        else
        {
            NotificationManager.instance.SetInfoMessage($"{correct} de {allSlots.Length} palavras corretas");
        }
    }

    public void ResetExercise()
    {
        foreach (var slot in allSlots)
        {
            if (slot.currentWord != null)
            {
                slot.currentWord.ReturnToOrigin();
                slot.ClearSlot();
            }
        }
    }
}