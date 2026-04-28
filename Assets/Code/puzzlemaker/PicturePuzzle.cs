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
    private Canvas children;

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
            if (currentPiece != null)
            {
                if (currentPiece.minihandler == Piece.Minigame.diferences)
                {
                    Debug.Log(currentPiece);
                    children = currentPiece.GetComponentInChildren<Canvas>();
                    children.sortingOrder = 4;
                }

            }
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
            else if (currentPiece.minihandler == Piece.Minigame.diferences)
            {
                float distance = currentPiece.transform.position.x - currentPiece.correctPosition.x;
                children.sortingOrder = 3;
                if (Mathf.Abs(distance) < 5f)
                {
                    Debug.Log(distance);
                    currentPiece.SetCorrectPositions();
                    currentPiece = null;
                    CheckForWin();
                }
                else
                {
                    currentPiece = null;
                }

            }
            else
            {

                currentPiece = null;

            }

        }


        if (currentPiece != null && goingback == false)
        {
            currentPiece.transform.position = new Vector3(mousePosition.x, mousePosition.y, 1);

            if (currentPiece.IsInCorrectPosition() && currentPiece.minihandler != Piece.Minigame.diferences)
            {
                currentPiece.SetCorrectPositions();
                currentPiece = null;
                CheckForWin();
            }
        }


        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            RotatePiece();
        }
    }

    private void HandleSelection(Vector2 mousePosition)
    {
        float smallestDistance = 2f;
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