using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;


public class PicturePuzzle : MonoBehaviour
{
    [SerializeField] private List<Piece> pieces = new List<Piece>();
    [SerializeField] private GameObject completedPicture;
    [SerializeField] private UnityEvent checkForWin;

    private Piece currentPiece;
    private Camera mainCamera;
    private bool goingback = false;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        foreach (Piece piece in pieces)
        {
            piece.gameObject.SetActive(true);
        }
        completedPicture.SetActive(false);
    }

    private void Update()
    {

        Vector2 screenPosition = Mouse.current.position.ReadValue();


        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, this.transform.position.z - mainCamera.transform.position.z));
        Vector2 mousePosition = new Vector2(worldPos.x, worldPos.y);


        if (Mouse.current.leftButton.wasPressedThisFrame && currentPiece == null)
        {

            HandleSelection(mousePosition);
        }


        if ((Mouse.current.leftButton.wasReleasedThisFrame && currentPiece != null) || goingback == true)
        {

            if (currentPiece.minihandler == Piece.Minigame.map)
            {
                goingback = true;
                float distance = Vector2.Distance(currentPiece.transform.position, currentPiece.startpos);
                currentPiece.movestartpos();

                if (distance < 0.5f)
                {

                    currentPiece = null;
                    goingback = false;
                }

            }
            else
            {
                Debug.Log("working");
                currentPiece = null;

            }

        }

        // 5. Dragging Logic
        if (currentPiece != null && goingback == false)
        {
            currentPiece.transform.position = new Vector3(mousePosition.x, mousePosition.y, 1);

            if (currentPiece.IsInCorrectPosition())
            {
                currentPiece.SetCorrectPositions();
                currentPiece = null;
                CheckForWin();
            }
        }

        // 6. Rotation (Optional: Map this to a specific key like 'R')
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            RotatePiece();
        }
    }

    private void HandleSelection(Vector2 mousePosition)
    {
        float smallestDistance = 2f; // Adjusted from 15f for world units
        Piece closestPiece = null;

        foreach (var piece in pieces)
        {
            if (piece.IsInCorrectPosition()) continue;

            float distance = Vector2.Distance(piece.transform.position, mousePosition);
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                closestPiece = piece;
            }
        }

        if (closestPiece != null)
        {
            currentPiece = closestPiece;
        }
    }

    private void CheckForWin()
    {
        bool allPiecesCorrect = true;
        foreach (var piece in pieces)
        {
            if (!piece.IsInCorrectPosition() && piece.hascorrectpos == true)
            {
                allPiecesCorrect = false;
                break;
            }
        }

        if (allPiecesCorrect)
        {
            completedPicture.SetActive(true);
            foreach (var piece in pieces)
            {
                piece.gameObject.SetActive(false);
            }
            checkForWin?.Invoke();
        }
    }

    public void RotatePiece()
    {
        if (currentPiece != null)
        {
            currentPiece.transform.Rotate(0, 0, 90f); // Rotates in 90-degree increments
        }
    }
}