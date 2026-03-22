using System;
using UnityEngine;

namespace FancyCrab.CustomPackages.FirstPersonController
{
    public class MouseLockHandler : MonoBehaviour
    {
        private void OnEnable()
        {
            PlayerStateHandler.OnPlayerStateChanged += OnChangeCallback;
        }
        private void OnChangeCallback(PlayerStates states)
        {
            switch (states)
            {
                case PlayerStates.Playing:
                    SetCursorLock(true);
                    break;
                case PlayerStates.Paused:
                    SetCursorLock(false);
                    break;
                default:
                    SetCursorLock(false);
                    break;
            }
        }
        private void OnDestroy()
        {
            PlayerStateHandler.OnPlayerStateChanged -= OnChangeCallback;
            SetCursorLock(false);
        }
        private void SetCursorLock(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}