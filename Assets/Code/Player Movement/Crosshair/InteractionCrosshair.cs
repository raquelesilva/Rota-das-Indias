using FancyCrab.CustomPackages.FirstPersonController;
using KendirStudios.CustomPackages.Utilities.Tools;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class InteractionCrosshair : MonoBehaviour
{
    [SerializeField] private Animator crosshairAnimation;
    private static readonly int AnimThrow = Animator.StringToHash("IsInteracting");

    private CanvasGroupUtility canvasGroupUtility;
    private void Awake()
    {
        canvasGroupUtility = transform.GetOrAddComponent<CanvasGroupUtility>();
    }
    private void OnEnable()
    {
        PlayerStateHandler.OnPlayerStateChanged += HandlePlayerStateChanged;
        //InteractionSystem.OnUpdatePickup += HandleCrosshairCallback;
    }

    private void HandlePlayerStateChanged(PlayerStates newState)
    {
        canvasGroupUtility.SetVisibility(newState == PlayerStates.Playing);
    }

    private void OnDisable()
    {
        PlayerStateHandler.OnPlayerStateChanged -= HandlePlayerStateChanged;
        //InteractionSystem.OnUpdatePickup -= HandleCrosshairCallback;
    }

    //private void HandleCrosshairCallback(InteractionSystem.ObjectState state)
    //{
    //    crosshairAnimation.SetBool(AnimThrow, state != InteractionSystem.ObjectState.None);
    //}
}
