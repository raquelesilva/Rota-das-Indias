using UnityEngine;

public class Piece : MonoBehaviour
{
    public enum Minigame
    {
        puzzle,
        map,
        anagram
    };
    public Minigame minihandler;
    GameObject pieceObject;

    private Vector3 correctPosition;
    private Quaternion correctRotation;

    private bool isLockedInPlace = false;
    private SpriteRenderer spriteRenderer;
    public bool rightplace = false;
    public bool hascorrectpos;
    private Vector3 parentpos;
    [HideInInspector] public Vector3 startpos;



    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogWarning("Sprite Render not working!");
        spriteRenderer.sortingOrder = 2;
        pieceObject = this.gameObject;
        correctPosition = pieceObject.transform.position;
        correctRotation = pieceObject.transform.rotation;
        parentpos = transform.parent.position;
        if (rightplace == false) RandomizePosition();

    }

    public void RandomizePosition()
    {
        if (minihandler == Minigame.puzzle)
            pieceObject.transform.position = correctPosition + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
        if (minihandler == Minigame.map)
        {
            pieceObject.transform.position = parentpos + new Vector3(Random.Range(-3f, 3f), Random.Range(-8f, 8f), 0);
            startpos = pieceObject.transform.position;
        }


    }

    public bool IsInCorrectPosition()
    {

        if (isLockedInPlace) return true;

        float distance = Vector2.Distance(transform.position, correctPosition);

        return distance < 0.5f;
    }

    public void SetCorrectPositions()
    {

        if (spriteRenderer != null) spriteRenderer.sortingOrder = 1;
        transform.position = correctPosition;
        transform.rotation = correctRotation;
        isLockedInPlace = true; // Lock it so it stops checking distance
    }
    public void movestartpos()
    {
        float distance = Vector2.Distance(transform.position, startpos);
        if (distance > 0.3f)
        {
            if (pieceObject.transform.position.x > startpos.x + 0.3f)
            {

                pieceObject.transform.position -= new Vector3(0.1f, 0, 0);
            }
            else if (pieceObject.transform.position.x < startpos.x - 0.3f)
            {
                pieceObject.transform.position += new Vector3(0.1f, 0, 0);
            }
            if (pieceObject.transform.position.y > startpos.y + 0.3f)
            {

                pieceObject.transform.position -= new Vector3(0, 0.2f / distance, 0);
            }
            else if (pieceObject.transform.position.y < startpos.y - 0.3f)
            {
                pieceObject.transform.position += new Vector3(0, 0.2f / distance, 0);
            }
        }


    }

}
