using UnityEngine;
using UnityEngine.UI;

public class LetterSlot : MonoBehaviour
{
    [HideInInspector] public int slot_index;
    [HideInInspector] public char expected_character;
    [HideInInspector] public bool is_locked;
    
    public Color normal_color = Color.white;
    public Color filled_color = Color.gray;
    public Color locked_correct_color = Color.green;
    
    [HideInInspector] public AnagramPuzzleController puzzle_controller;

    public bool is_filled { get; set; }
    public char current_character { get; set; }

    private DraggableLetter current_occupant;
    private Image slot_image;

    private void Awake()
    {
        slot_image = GetComponent<Image>();
        slot_image.color = normal_color;
    }

    public void AcceptLetter(DraggableLetter incoming_letter)
    {
        DraggableLetter previous_letter = current_occupant;
        
        current_occupant = incoming_letter;
        current_character = incoming_letter.letter_character;
        is_filled = true;

        if (previous_letter != null && previous_letter != incoming_letter)
        {
            previous_letter.SetCurrentSlot(null);
            
            RectTransform prev_rt = previous_letter.GetComponent<RectTransform>();
            prev_rt.anchoredPosition += new Vector2(Random.Range(-50f, 50f), Random.Range(-150f, -100f));
            
            previous_letter.GoHome();
        }

        incoming_letter.SetCurrentSlot(this);
        incoming_letter.SnapToSlot(this);
        slot_image.color = filled_color;
    }

    public void ReleaseSlot()
    {
        if (is_locked) return;

        current_occupant = null;
        current_character = '\0';
        is_filled = false;
        
        if (slot_image != null) 
        {
            slot_image.color = normal_color;
        }
    }

    public void LockAsCorrect(DraggableLetter correct_letter)
    {
        AcceptLetter(correct_letter);
        is_locked = true;
        slot_image.color = locked_correct_color;
        
        correct_letter.GetComponent<CanvasGroup>().blocksRaycasts = false; 
    }
}