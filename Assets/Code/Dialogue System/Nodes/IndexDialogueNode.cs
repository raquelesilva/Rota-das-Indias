using UnityEngine;

namespace FancyCrab.DialogueSystem
{
    [CreateAssetMenu(fileName = "New Index Node", menuName = StudioInfo.ASSET_MENU_PATH + "Index Node")]
    public class IndexDialogueNode : DialogueNode
    {
        [Header("Index Settings")]
        public int indexValue = 0;

        [Header("Next Node")]
        public DialogueNode nextNode;

        public override bool HasActor => false;

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(name) || name == "New Index Node")
            {
                name = $"Index [{indexValue}]";
            }
        }

        public void UpdateName()
        {
            name = $"Index [{indexValue}]";
        }
    }
}