using FancyCrab.CoreSystems.InteractionSystem;
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
        canvasGroupUtility.Show();
    }
    private void OnEnable()
    {
        PlayerStateHandler.OnPlayerStateChanged += HandlePlayerStateChanged;
        PlayerInteraction.OnDetectInterface += HandleCrosshairCallback;
        PlayerInteraction.OnClearDetection += ClearCrosshairCallback;
    }

    private void HandleCrosshairCallback(RaycastState state)
    {
        crosshairAnimation.SetBool(AnimThrow, true);
    }
    private void ClearCrosshairCallback()
    {
        crosshairAnimation.SetBool(AnimThrow, false);
    }
    private void HandlePlayerStateChanged(PlayerStates state)
    {
        canvasGroupUtility.SetVisibility(state == PlayerStates.Playing);
    }
    private void OnDisable()
    {
        PlayerStateHandler.OnPlayerStateChanged -= HandlePlayerStateChanged;
        PlayerInteraction.OnDetectInterface -= HandleCrosshairCallback;
        PlayerInteraction.OnClearDetection -= ClearCrosshairCallback;
    }
}
