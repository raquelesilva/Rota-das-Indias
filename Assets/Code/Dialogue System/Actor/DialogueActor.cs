using UnityEngine;

namespace FancyCrab.DialogueSystem
{
    [CreateAssetMenu(fileName = "New Dialogue Actor", menuName = StudioInfo.ASSET_MENU_PATH + "Actor")]
    public class DialogueActor : ScriptableObject
    {
        [Header("Actor Information")]
        public string actorName;
        public Sprite actorPortrait;
    }
}