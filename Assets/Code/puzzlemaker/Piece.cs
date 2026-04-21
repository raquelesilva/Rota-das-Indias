using UnityEngine;

public class Piece : MonoBehaviour
{
    GameObject pieceObject; // The visual representation of the piece

    private Vector3 correctPosition;
    private Quaternion correctRotation;

    private bool isLockedInPlace = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogWarning("Sprite Render not working!");
        spriteRenderer.sortingOrder = 1;
        pieceObject = this.gameObject;
        correctPosition = pieceObject.transform.position;
        correctRotation = pieceObject.transform.rotation;

        RandomizePosition();
    }

    public void RandomizePosition()
    {
        
        pieceObject.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
    }

    public bool IsInCorrectPosition()
    {
        
        if (isLockedInPlace) return true;

        float distance = Vector2.Distance(transform.position, correctPosition);

        return distance < 0.5f;
    }

    public void SetCorrectPositions()
    {
        Debug.Log("potato");
        if (spriteRenderer != null) spriteRenderer.sortingOrder = 0;
        transform.position = correctPosition;
        transform.rotation = correctRotation;
        isLockedInPlace = true; // Lock it so it stops checking distance
    }
}
