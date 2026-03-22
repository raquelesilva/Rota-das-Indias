using UnityEngine;

namespace FancyCrab.DialogueSystem
{
    [CreateAssetMenu(fileName = "New Text Dialogue", menuName = StudioInfo.ASSET_MENU_PATH + "Text Node")]
    public class TextDialogueNode : DialogueNode
    {
        [Header("Text Dialogue")]
        [TextArea(3, 10)]
        public string dialogueText;

        [Header("Next Node")]
        public DialogueNode nextNode;
    }
}