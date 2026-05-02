using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class DraggableLetter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public char letter_character;
    [HideInInspector] public Vector2 home_position;
    [HideInInspector] public AnagramPuzzleController puzzle_controller;

    private RectTransform rect_transform;
    private CanvasGroup canvas_group;
    private Canvas root_canvas;
    private LetterSlot current_slot;

    private void Awake()
    {
        rect_transform = GetComponent<RectTransform>();
        canvas_group = GetComponent<CanvasGroup>();
        
        root_canvas = FindAnyObjectByType<Canvas>(); 
    }

    public void OnBeginDrag(PointerEventData event_data)
    {
        if (current_slot != null)
        {
            current_slot.ReleaseSlot();
            current_slot = null;
        }

        canvas_group.alpha = 0.75f;
        canvas_group.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData event_data)
    {
        rect_transform.anchoredPosition += event_data.delta / root_canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData event_data)
    {
        canvas_group.alpha = 1f;
        canvas_group.blocksRaycasts = true;

        LetterSlot target_slot = FindSlotUnderTile();

        if (target_slot != null && !target_slot.is_locked)
        {
            current_slot = target_slot;
            target_slot.AcceptLetter(this);
        }
        else
        {
            KeepInsideContainer();
        }
    }

    public void SnapToSlot(LetterSlot slot)
    {
        RectTransform slot_rect_transform = slot.GetComponent<RectTransform>();
        RectTransform parent_rect_transform = rect_transform.parent as RectTransform;
        
        Vector2 screen_position = RectTransformUtility.WorldToScreenPoint(null, slot_rect_transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent_rect_transform, screen_position, null, out Vector2 local_position);
        
        rect_transform.anchoredPosition = local_position;
    }

    public void GoHome() 
    {
        current_slot = null;
        KeepInsideContainer();
    }

    public void SetCurrentSlot(LetterSlot slot)
    {
        current_slot = slot;
    }

    private LetterSlot FindSlotUnderTile()
    {
        Vector2 screen_position = RectTransformUtility.WorldToScreenPoint(null, rect_transform.position);

        foreach (LetterSlot slot in puzzle_controller.GetAllSlots())
        {
            if (slot.is_locked) continue;

            if (RectTransformUtility.RectangleContainsScreenPoint(slot.GetComponent<RectTransform>(), screen_position, null))
            {
                return slot;
            }
        }
        return null;
    }

    private void KeepInsideContainer()
    {
        RectTransform container = transform.parent as RectTransform;
        Vector2 pos = rect_transform.anchoredPosition;

        float limit_x = (container.rect.width / 2f) - (rect_transform.rect.width / 2f);
        float limit_y = (container.rect.height / 2f) - (rect_transform.rect.height / 2f);

        pos.x = Mathf.Clamp(pos.x, -limit_x, limit_x);
        pos.y = Mathf.Clamp(pos.y, -limit_y, limit_y);

        rect_transform.anchoredPosition = pos;
    }
}