using CoreSystems.Managers;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AstrolabioManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> pieces;
    [SerializeField] private UnityEvent onAllPiecesCollected;

    private void Awake()
    {
        foreach (var piece in pieces)
        {
            piece.SetActive(false);
        }
    }

    [Button]
    public void StartGame()
    {
        foreach (var piece in pieces)
        {
            piece.SetActive(true);
        }
    }

    public void PickupPiece(GameObject pickedPiece)
    {
        pickedPiece.SetActive(false);
        NotificationManager.instance.SetCorrectMessage("Apanhas-te uma peça!");
        if (pieces.Contains(pickedPiece))
        {
            pieces.Remove(pickedPiece);
            CheckAllPieces();
        }
    }
    private void CheckAllPieces()
    {
        if (pieces.Count.Equals(0))
        {
            onAllPiecesCollected?.Invoke();
        }
    }
}
