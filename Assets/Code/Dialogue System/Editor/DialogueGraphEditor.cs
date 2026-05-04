using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

namespace FancyCrab.DialogueSystem.Editor
{
    public class DialogueGraphEditor : EditorWindow
    {
        private DialogueGraphView graphView;
        private DialogueContainer currentDialogue;

        [MenuItem(StudioInfo.WINDOW_PATH + "Dialogue Graph", false, 1)]
        public static void OpenWindow()
        {
            var window = GetWindow<DialogueGraphEditor>();
            window.titleContent = new GUIContent("Dialogue Graph", EditorGUIUtility.IconContent("d_UnityEditor.ConsoleWindow").image);
            window.minSize = new Vector2(800, 600);
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID) as DialogueContainer;
            if (asset != null)
            {
                var window = GetWindow<DialogueGraphEditor>();
                window.LoadDialogue(asset);
                return true;
            }
            return false;
        }

        private void OnEnable()
        {
            ConstructGraphView();

            // Garante save antes de domain reload (recompilação de scripts)
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        }

        private void OnBeforeAssemblyReload()
        {
            graphView?.SaveAllChanges();
        }

        private void ConstructGraphView()
        {
            graphView = new DialogueGraphView(this)
            {
                name = "Dialogue Graph"
            };

            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }

        public void LoadDialogue(DialogueContainer dialogue)
        {
            currentDialogue = dialogue;
            graphView.Load(dialogue);

            titleContent = new GUIContent($"Dialogue Graph - {dialogue.name}");
        }

        public DialogueContainer CurrentDialogue
        {
            get { return currentDialogue; }
            set { currentDialogue = value; }
        }

        private void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;

            // Garante save ao fechar a janela
            graphView?.SaveAllChanges();

            if (graphView != null)
            {
                rootVisualElement.Remove(graphView);
            }
        }
    }
}