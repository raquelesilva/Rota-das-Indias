using UnityEditor;
using UnityEngine;

namespace FancyCrab.DialogueSystem.Editor
{
    public static class DialogueMenu
    {
        [MenuItem("GameObject/Fancy Crab Studios/Dialogue/UI Manager", false, 10)]
        public static void CreateDialogueManager()
        {
            GameObject go = new GameObject("FancyCrabDialogueManager");
            go.AddComponent<DialogueManager>();

            // Create basic UI structure
            CreateBasicUI(go);

            Selection.activeGameObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create Dialogue Manager");
        }

        [MenuItem("GameObject/Fancy Crab Studios/Dialogue/Dialogue Trigger", false, 11)]
        public static void CreateDialogueTrigger()
        {
            GameObject go = new GameObject("FancyCrabDialogueTrigger");
            go.AddComponent<DialogueTrigger>();

            // Não adiciona mais collider automático
            // Apenas cria um trigger vazio que precisa ser ativado por script

            Selection.activeGameObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create Dialogue Trigger");
        }

        private static void CreateBasicUI(GameObject manager)
        {
            // This would create a basic canvas with the required UI elements
            // Implementation depends on your UI setup
        }

        [MenuItem("Assets/Create/Fancy Crab Studios/Dialogue/Actor", false, 1)]
        public static void CreateDialogueActor()
        {
            var asset = ScriptableObject.CreateInstance<DialogueActor>();
            ProjectWindowUtil.CreateAsset(asset, "New Dialogue Actor.asset");
        }

        [MenuItem("Assets/Create/Fancy Crab Studios/Dialogue/Dialogue Container", false, 2)]
        public static void CreateDialogueContainer()
        {
            var asset = ScriptableObject.CreateInstance<DialogueContainer>();
            ProjectWindowUtil.CreateAsset(asset, "New Dialogue Container.asset");
        }
    }
}