using FancyCrab.DialogueSystem;
using NaughtyAttributes;
using System;
using UnityEngine;

namespace FancyCrab.CustomPackages.FirstPersonController
{
    public class PlayerStateHandler : MonoBehaviour
    {
        public static PlayerStateHandler Instance { get; private set; }
        [SerializeField, OnValueChanged(nameof(RaisePlayerStateChanged))] private PlayerStates currentPlayerState;

        public static event Action<PlayerStates> OnPlayerStateChanged;

        private void OnEnable()
        {
            //PauseHandler.OnPausedGame += OnPausedGameCallback;
            if (DialogueManager.Instance == null)
                return;

            DialogueManager.Instance.OnDialogueStarted += HandleDialogueStarted;
            DialogueManager.Instance.OnDialogueEnded += HandleDialogueEnded;
        }

        private void HandleDialogueEnded(DialogueContainer container)
        {
            SetPlayerState(PlayerStates.Playing);
        }

        private void HandleDialogueStarted(DialogueContainer container)
        {
            SetPlayerState(PlayerStates.Paused);
        }

        private void OnDisable()
        {
            //PauseHandler.OnPausedGame -= OnPausedGameCallback;

            DialogueManager.Instance.OnDialogueStarted -= HandleDialogueStarted;
            DialogueManager.Instance.OnDialogueEnded -= HandleDialogueEnded;
        }
        private void OnPausedGameCallback(bool isPaused)
        {
            SetPlayerState(isPaused ? PlayerStates.Paused : PlayerStates.Playing);
        }
        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            RaisePlayerStateChanged();
        }

        public void SetPlayerState(PlayerStates newState)
        {
            currentPlayerState = newState;
            RaisePlayerStateChanged();
        }

        private void RaisePlayerStateChanged()
        {
            OnPlayerStateChanged?.Invoke(currentPlayerState);
        }
    }
}