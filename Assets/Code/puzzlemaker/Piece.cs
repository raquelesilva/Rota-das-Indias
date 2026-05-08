using UnityEngine;
using UnityEngine.Events;
public class Piece : MonoBehaviour
{
    [SerializeField] private UnityEvent Feedback;
    private bool feedback;
    public enum Minigame
    {
        puzzle,
        map,
        diferences
    };

    public Minigame minihandler;
    GameObject pieceObject;

    [HideInInspector] public Vector3 correctPosition;
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
        spriteRenderer.sortingOrder = 3;
        pieceObject = this.gameObject;
        correctPosition = pieceObject.transform.position;
        correctRotation = pieceObject.transform.rotation;
        parentpos = transform.parent.position;
        if (rightplace == false) RandomizePosition();
    }

    public void RandomizePosition()
    {
        if (minihandler == Minigame.puzzle || minihandler == Minigame.diferences)
            pieceObject.transform.position = parentpos + new Vector3(Random.Range(-6f, 6f), Random.Range(-6f, 6f), 0);
        if (minihandler == Minigame.map)
        {
            pieceObject.transform.position = parentpos + new Vector3(Random.Range(-3f, 1f), Random.Range(-8f, 8f), 0);
            startpos = pieceObject.transform.position;
        }
    }

    public bool IsInCorrectPosition()
    {
        feedback = true;
        if (isLockedInPlace) return true;

        float distance = Vector2.Distance(transform.position, correctPosition);

        return distance < 0.5f;
    }

    public void SetCorrectPositions()
    {
        if (spriteRenderer != null) spriteRenderer.sortingOrder = 2;
        transform.position = correctPosition;
        transform.rotation = correctRotation;
        isLockedInPlace = true; // Lock it so it stops checking distance
    }

    public void MoveStartPos()
    {
        if (feedback == true)
        {
            Feedback?.Invoke();
            feedback = false;
        }
        float distance = Vector2.Distance(transform.position, startpos);
        if (distance > 0.3f)
        {
            if (pieceObject.transform.position.x > startpos.x + 0.3f)
            {
                pieceObject.transform.position -= new Vector3(Time.deltaTime * 15, 0, 0);
            }
            else if (pieceObject.transform.position.x < startpos.x - 0.3f)
            {
                pieceObject.transform.position += new Vector3(Time.deltaTime * 15, 0, 0);
            }
            if (pieceObject.transform.position.y > startpos.y + 0.3f)
            {
                pieceObject.transform.position -= new Vector3(0, Time.deltaTime * 15, 0);
            }
            else if (pieceObject.transform.position.y < startpos.y - 0.3f)
            {
                pieceObject.transform.position += new Vector3(0, Time.deltaTime * 15, 0);
            }
        }
    }
}