using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableWord : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private bool isInteractable = true;

    [Header("ID")]
    public string wordID;

    [HideInInspector] public DropSlot currentSlot;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas rootCanvas;
    private Transform originalParent;
    private Vector2 originalPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        rootCanvas = GetComponentInParent<Canvas>();
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;
    }

    public void SetInteractable(bool decision)
    {
        isInteractable = decision;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isInteractable) return;

        if (currentSlot != null)
        {
            currentSlot.ClearSlot();
            currentSlot = null;
        }

        transform.SetParent(rootCanvas.transform);
        transform.SetAsLastSibling();

        canvasGroup.alpha = 0.75f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isInteractable) return;

        rectTransform.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isInteractable) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (currentSlot == null)
            ReturnToOrigin();
    }

    public void ReturnToOrigin()
    {
        if (!isInteractable) return;

        currentSlot = null;
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }

    public void SnapToSlot(DropSlot slot)
    {
        currentSlot = slot;
        transform.SetParent(slot.transform);
        rectTransform.anchoredPosition = Vector2.zero;
    }
}