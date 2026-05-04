using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class AnagramPuzzleController : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public string target_word = "PUZZLE";
    public int starting_correct_index = -1;

    [Header("UI References")]
    public GameObject puzzle_panel;
    public Transform slots_container;
    public Transform letters_container;
    public GameObject letter_prefab;
    public GameObject slot_prefab;
    public Button complete_button;

    [Header("Events")]
    public UnityEvent OnPuzzleComplete;

    private List<LetterSlot> all_slots = new List<LetterSlot>();
    private List<DraggableLetter> all_letters = new List<DraggableLetter>();

    private void Start()
    {
        if (CheckForMissingReferences())
        {
            puzzle_panel.SetActive(false); 
            complete_button.onClick.AddListener(CheckIfPuzzleIsCorrect);
            ActivatePuzzle();
        }
    }

    private bool CheckForMissingReferences()
    {
        if (puzzle_panel == null) return false;
        if (slots_container == null) return false;
        if (letters_container == null) return false;
        if (letter_prefab == null) return false;
        if (slot_prefab == null) return false;
        if (complete_button == null) return false;
        return true;
    }

    public void ActivatePuzzle()
    {
        puzzle_panel.SetActive(true);
        GeneratePuzzle();
    }

    private void GeneratePuzzle()
    {
        foreach (Transform child in slots_container) Destroy(child.gameObject);
        foreach (Transform child in letters_container) Destroy(child.gameObject);
        all_slots.Clear();
        all_letters.Clear();

        target_word = target_word.ToUpper();
        char[] word_characters = target_word.ToCharArray();

        RectTransform slots_rect = slots_container.GetComponent<RectTransform>();
        float slot_dimension = Mathf.Min((slots_rect.rect.width / word_characters.Length) - 5f, slots_rect.rect.height, 150f); 

        for (int i = 0; i < word_characters.Length; i++)
        {
            GameObject new_slot_object = Instantiate(slot_prefab, slots_container);
            new_slot_object.GetComponent<RectTransform>().sizeDelta = new Vector2(slot_dimension, slot_dimension);

            LetterSlot slot_script = new_slot_object.GetComponent<LetterSlot>();
            slot_script.slot_index = i;
            slot_script.expected_character = word_characters[i];
            slot_script.puzzle_controller = this;
            all_slots.Add(slot_script);
        }

        Canvas.ForceUpdateCanvases(); 

        RectTransform container_rect = letters_container.GetComponent<RectTransform>();
        float spawn_limit_x = (container_rect.rect.width / 2f) - (slot_dimension / 2f);
        float spawn_limit_y = (container_rect.rect.height / 2f) - (slot_dimension / 2f);

        for (int i = 0; i < word_characters.Length; i++)
        {
            GameObject new_letter_object = Instantiate(letter_prefab, letters_container);
            new_letter_object.GetComponent<RectTransform>().sizeDelta = new Vector2(slot_dimension, slot_dimension);

            DraggableLetter letter_script = new_letter_object.GetComponent<DraggableLetter>();
            letter_script.letter_character = word_characters[i];
            letter_script.puzzle_controller = this;

            TMP_Text text_component = new_letter_object.GetComponentInChildren<TMP_Text>();
            if (text_component != null) {
                text_component.text = word_characters[i].ToString();
                text_component.enableAutoSizing = true;
            }

            float rx = Random.Range(-spawn_limit_x, spawn_limit_x);
            float ry = Random.Range(-spawn_limit_y, spawn_limit_y);
            new_letter_object.GetComponent<RectTransform>().anchoredPosition = new Vector2(rx, ry);

            all_letters.Add(letter_script);
        }

        LockStartingLetter();
    }

    private void LockStartingLetter()
    {
        if (target_word.Length == 0) return;
        int idx = (starting_correct_index < 0 || starting_correct_index >= target_word.Length) ? Random.Range(0, target_word.Length) : starting_correct_index;

        LetterSlot slot_to_lock = all_slots[idx];
        foreach (DraggableLetter letter in all_letters)
        {
            if (letter.letter_character == slot_to_lock.expected_character && letter.GetComponent<CanvasGroup>().blocksRaycasts)
            {
                slot_to_lock.LockAsCorrect(letter);
                letter.transform.SetAsLastSibling(); 
                break;
            }
        }
    }

    public void CheckIfPuzzleIsCorrect()
    {
        bool is_puzzle_correct = true;
        foreach (LetterSlot slot in all_slots)
        {
            if (!slot.is_filled || slot.current_character != slot.expected_character)
            {
                is_puzzle_correct = false;
                break;
            }
        }

        if (is_puzzle_correct)
        {
            Debug.Log("PUZZLE COMPLETE!");
            OnPuzzleComplete?.Invoke();
            puzzle_panel.SetActive(false);
        }
    }

    public List<LetterSlot> GetAllSlots() { return all_slots; }
}