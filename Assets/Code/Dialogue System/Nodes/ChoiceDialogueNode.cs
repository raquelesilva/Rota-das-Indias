using System;
using UnityEngine;

namespace FancyCrab.DialogueSystem
{
    [CreateAssetMenu(fileName = "New Choice Dialogue", menuName = StudioInfo.ASSET_MENU_PATH + "Choice Node")]
    public class ChoiceDialogueNode : DialogueNode
    {
        [Header("Choice Dialogue")]
        [SerializeField] private ChoiceOption[] choices = new ChoiceOption[4];

        public ChoiceOption[] Choices => choices;

        [Serializable]
        public struct ChoiceOption
        {
            [TextArea(1, 3)]
            public string choiceText;
            public DialogueNode nextNode;

            [Tooltip("Optional")]
            public string requiredFlag;
            public bool hideIfFlagMissing;
        }

        public bool HasValidChoices()
        {
            foreach (var choice in choices)
            {
                if (!string.IsNullOrEmpty(choice.choiceText))
                    return true;
            }
            return false;
        }
    }
}