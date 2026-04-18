using FancyCrab.CoreSystems.InteractionSystem;
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
            DialogueManager.OnDialogueState += OnDialogueState;
            InspectHandler.OnInspectStateChanged += OnInspectStateChangedCallback;
        }

        private void OnInspectStateChangedCallback(bool state)
        {
            OnPausedGameCallback(state);
        }

        private void OnDisable()
        {
            DialogueManager.OnDialogueState -= OnDialogueState;
            InspectHandler.OnInspectStateChanged -= OnInspectStateChangedCallback;
        }
        private void Awake()
        {
            if (Instance != null)
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

        private void OnDialogueState(bool state)
        {
            OnPausedGameCallback(state);
        }

        private void OnPausedGameCallback(bool isPaused)
        {
            SetPlayerState(isPaused ? PlayerStates.Paused : PlayerStates.Playing);
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