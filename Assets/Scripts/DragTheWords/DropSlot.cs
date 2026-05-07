using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropSlot : MonoBehaviour, IDropHandler
{
    [Header("IDs")]
    [SerializeField] private string correctWordID;
    
    [HideInInspector] public DraggableWord currentWord;


    public void OnDrop(PointerEventData eventData)
    {
        DraggableWord dropped = eventData.pointerDrag?.GetComponent<DraggableWord>();
        if (dropped == null) return;

        if (currentWord != null && currentWord != dropped)
        {
            currentWord.ReturnToOrigin();
        }
        currentWord = dropped;
        dropped.SnapToSlot(this);
    }

    public void ClearSlot()
    {
        currentWord = null;
    }

    public void PropagateColor(DragTheWordManager manager)
    {
        StartCoroutine(ColorRoutine(manager));
    }
    IEnumerator ColorRoutine(DragTheWordManager manager)
    {
        bool isCorrect = IsCorrect();
        var img = currentWord.GetComponent<Image>();

        img.color = isCorrect ? manager.correctColor : manager.wrongColor;
        yield return new WaitForSeconds(3);
        img.color = manager.baseDraggableColor;
    }

    public bool IsCorrect() => currentWord != null && currentWord.wordID == correctWordID;
}