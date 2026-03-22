using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace FancyCrab.DialogueSystem.Editor
{
    public class DialogueGraphView : GraphView
    {
        public DialogueGraphEditor editor;
        private Dictionary<string, DialogueNodeView> nodeViews = new Dictionary<string, DialogueNodeView>();

        private Color gridColor = new Color(0.3f, 0.3f, 0.3f);

        // Expoe o dialogo actual para que DialogueNodeView possa invalidar o cache
        public DialogueContainer CurrentDialogue => editor?.CurrentDialogue;

        public DialogueGraphView(DialogueGraphEditor editor)
        {
            this.editor = editor;

            var grid = new GridBackground();
            grid.StretchToParentSize();
            Insert(0, grid);
            grid.style.unityBackgroundImageTintColor = gridColor;

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // ContentDragger nativo não suporta right click — usamos manipulator próprio
            this.AddManipulator(new RightClickPanManipulator());

            var selectionDragger = new SelectionDragger();
            selectionDragger.activators.Clear();
            selectionDragger.activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });
            this.AddManipulator(selectionDragger);

            this.AddManipulator(new RectangleSelector());

            graphViewChanged = OnGraphViewChanged;

            style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);

            RegisterCallback<KeyDownEvent>(OnKeyDown);
            RegisterCallback<MouseMoveEvent>(evt => lastMousePosition = evt.localMousePosition);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            SaveAllChanges();
        }

        private void SaveAllChanges()
        {
            if (editor == null || editor.CurrentDialogue == null)
                return;

            foreach (var nodeView in nodeViews.Values)
            {
                nodeView.node.position = nodeView.GetPosition().position;
                EditorUtility.SetDirty(nodeView.node);
            }

            EditorUtility.SetDirty(editor.CurrentDialogue);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private Vector2 lastMousePosition;

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.Space)
                return;

            var graphPosition = contentViewContainer.WorldToLocal(lastMousePosition);
            ShowNodeCreationMenu(graphPosition);
            evt.StopPropagation();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // Desactiva o menu de contexto nativo do right click
        }

        private void ShowNodeCreationMenu(Vector2 graphPosition)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Text Node"), false,
                () => CreateNodeAtPosition("Text Node", typeof(TextDialogueNode), graphPosition));

            menu.AddItem(new GUIContent("Choice Node"), false,
                () => CreateNodeAtPosition("Choice Node", typeof(ChoiceDialogueNode), graphPosition));

            menu.AddItem(new GUIContent("Index Node"), false,
                () => CreateIndexNodeAtPosition(graphPosition));

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Save Dialogue"), false, SaveAllChanges);

            if (editor != null && editor.CurrentDialogue != null)
                menu.AddItem(new GUIContent("Validate Indices"), false, ValidateIndices);

            menu.ShowAsContext();
        }

        private void CreateNodeAtPosition(string nodeName, System.Type nodeType, Vector2 position)
        {
            if (editor == null || editor.CurrentDialogue == null)
            {
                Debug.LogError("[FancyCrabStudios] No dialogue loaded!");
                return;
            }

            var node = ScriptableObject.CreateInstance(nodeType) as DialogueNode;
            node.name = nodeName;
            node.nodeGUID = System.Guid.NewGuid().ToString();
            node.position = position;

            AssetDatabase.AddObjectToAsset(node, editor.CurrentDialogue);
            editor.CurrentDialogue.nodes.Add(node);

            var nodeView = new DialogueNodeView(node, this);
            nodeView.SetPosition(new Rect(position.x, position.y, 250, 200));
            AddElement(nodeView);
            nodeViews[node.nodeGUID] = nodeView;

            EditorUtility.SetDirty(node);
            EditorUtility.SetDirty(editor.CurrentDialogue);
            AssetDatabase.SaveAssets();

            Debug.Log($"[FancyCrabStudios] Created {nodeName}");
        }

        private void CreateIndexNodeAtPosition(Vector2 position)
        {
            if (editor == null || editor.CurrentDialogue == null)
            {
                Debug.LogError("[FancyCrabStudios] No dialogue loaded!");
                return;
            }

            int nextIndex = editor.CurrentDialogue.GetNextAvailableIndex();

            var node = ScriptableObject.CreateInstance<IndexDialogueNode>();
            node.indexValue = nextIndex;
            node.name = $"Index [{nextIndex}]";
            node.nodeGUID = System.Guid.NewGuid().ToString();
            node.position = position;

            AssetDatabase.AddObjectToAsset(node, editor.CurrentDialogue);
            editor.CurrentDialogue.nodes.Add(node);

            var nodeView = new DialogueNodeView(node, this);
            nodeView.SetPosition(new Rect(position.x, position.y, 250, 200));
            AddElement(nodeView);
            nodeViews[node.nodeGUID] = nodeView;

            EditorUtility.SetDirty(node);
            EditorUtility.SetDirty(editor.CurrentDialogue);
            AssetDatabase.SaveAssets();

            Debug.Log($"[FancyCrabStudios] Created Index Node [{nextIndex}]");
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()
                .Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node)
                .ToList();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            if (editor == null || editor.CurrentDialogue == null)
                return change;

            bool needsSave = false;

            if (change.elementsToRemove != null)
            {
                foreach (var element in change.elementsToRemove)
                {
                    if (element is DialogueNodeView nodeView)
                    {
                        if (editor.CurrentDialogue.nodes.Contains(nodeView.node))
                        {
                            editor.CurrentDialogue.nodes.Remove(nodeView.node);
                            RemoveNodeReferences(nodeView.node);
                            UnityEngine.Object.DestroyImmediate(nodeView.node, true);
                            nodeViews.Remove(nodeView.node.nodeGUID);
                            needsSave = true;
                        }
                    }
                    else if (element is Edge edge)
                    {
                        DisconnectNodes(edge);
                        needsSave = true;
                    }
                }
            }

            if (change.edgesToCreate != null)
            {
                foreach (var edge in change.edgesToCreate)
                {
                    var outputNode = edge.output.node as DialogueNodeView;
                    var inputNode = edge.input.node as DialogueNodeView;

                    if (outputNode != null && inputNode != null)
                    {
                        ConnectNodes(outputNode, inputNode, edge);
                        needsSave = true;
                    }
                }
            }

            if (change.movedElements != null)
            {
                foreach (var element in change.movedElements)
                {
                    if (element is DialogueNodeView nodeView)
                    {
                        nodeView.node.position = nodeView.GetPosition().position;
                        needsSave = true;
                    }
                }
            }

            if (needsSave)
            {
                EditorUtility.SetDirty(editor.CurrentDialogue);
                AssetDatabase.SaveAssets();
            }

            return change;
        }

        private void RemoveNodeReferences(DialogueNode nodeToRemove)
        {
            foreach (var node in editor.CurrentDialogue.nodes)
            {
                if (node == nodeToRemove)
                    continue;

                if (node is TextDialogueNode textNode && textNode.nextNode == nodeToRemove)
                {
                    textNode.nextNode = null;
                    EditorUtility.SetDirty(textNode);
                }
                else if (node is ChoiceDialogueNode choiceNode)
                {
                    for (int i = 0; i < choiceNode.Choices.Length; i++)
                    {
                        var choices = choiceNode.Choices;
                        if (choices[i].nextNode != nodeToRemove)
                            continue;

                        var newChoice = choices[i];
                        newChoice.nextNode = null;
                        choices[i] = newChoice;

                        var fieldInfo = typeof(ChoiceDialogueNode).GetField(
                            "choices",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                        if (fieldInfo != null)
                        {
                            fieldInfo.SetValue(choiceNode, choices);
                            EditorUtility.SetDirty(choiceNode);
                        }
                        break;
                    }
                }
                else if (node is IndexDialogueNode indexNode && indexNode.nextNode == nodeToRemove)
                {
                    indexNode.nextNode = null;
                    EditorUtility.SetDirty(indexNode);
                }
            }

            if (editor.CurrentDialogue.startNode == nodeToRemove)
                editor.CurrentDialogue.startNode = null;
        }

        private void ConnectNodes(DialogueNodeView outputNode, DialogueNodeView inputNode, Edge edge)
        {
            if (outputNode.node is TextDialogueNode textNode)
            {
                textNode.nextNode = inputNode.node;
                EditorUtility.SetDirty(textNode);
            }
            else if (outputNode.node is ChoiceDialogueNode choiceNode)
            {
                int portIndex = outputNode.outputContainer.IndexOf(edge.output);
                if (portIndex >= 0 && portIndex < choiceNode.Choices.Length)
                {
                    var choices = choiceNode.Choices;
                    var newChoice = choices[portIndex];
                    newChoice.nextNode = inputNode.node;
                    choices[portIndex] = newChoice;

                    var fieldInfo = typeof(ChoiceDialogueNode).GetField(
                        "choices",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(choiceNode, choices);
                        EditorUtility.SetDirty(choiceNode);
                    }
                }
            }
            else if (outputNode.node is IndexDialogueNode indexNode)
            {
                indexNode.nextNode = inputNode.node;
                EditorUtility.SetDirty(indexNode);
            }
        }

        private void DisconnectNodes(Edge edge)
        {
            var outputNode = edge.output.node as DialogueNodeView;

            if (outputNode?.node is TextDialogueNode textNode)
            {
                if (textNode.nextNode == (edge.input.node as DialogueNodeView)?.node)
                {
                    textNode.nextNode = null;
                    EditorUtility.SetDirty(textNode);
                }
            }
            else if (outputNode?.node is ChoiceDialogueNode choiceNode)
            {
                int portIndex = outputNode.outputContainer.IndexOf(edge.output);
                if (portIndex >= 0 && portIndex < choiceNode.Choices.Length)
                {
                    var choices = choiceNode.Choices;
                    var newChoice = choices[portIndex];
                    newChoice.nextNode = null;
                    choices[portIndex] = newChoice;

                    var fieldInfo = typeof(ChoiceDialogueNode).GetField(
                        "choices",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(choiceNode, choices);
                        EditorUtility.SetDirty(choiceNode);
                    }
                }
            }
            else if (outputNode?.node is IndexDialogueNode indexNode)
            {
                if (indexNode.nextNode == (edge.input.node as DialogueNodeView)?.node)
                {
                    indexNode.nextNode = null;
                    EditorUtility.SetDirty(indexNode);
                }
            }
        }

        public void Load(DialogueContainer dialogue)
        {
            DeleteElements(graphElements.ToList());
            nodeViews.Clear();

            foreach (var node in dialogue.nodes)
            {
                var nodeView = new DialogueNodeView(node, this);
                nodeView.SetPosition(new Rect(node.position.x, node.position.y, 250, 200));
                AddElement(nodeView);
                nodeViews[node.nodeGUID] = nodeView;
            }

            foreach (var node in dialogue.nodes)
            {
                if (node is TextDialogueNode textNode && textNode.nextNode != null)
                {
                    CreateEdge(node, textNode.nextNode);
                }
                else if (node is ChoiceDialogueNode choiceNode)
                {
                    for (int i = 0; i < choiceNode.Choices.Length; i++)
                    {
                        if (choiceNode.Choices[i].nextNode != null)
                            CreateEdge(node, choiceNode.Choices[i].nextNode, i);
                    }
                }
                else if (node is IndexDialogueNode indexNode && indexNode.nextNode != null)
                {
                    CreateEdge(node, indexNode.nextNode);
                }
            }
        }

        private void CreateEdge(DialogueNode fromNode, DialogueNode toNode, int outputIndex = 0)
        {
            if (!nodeViews.TryGetValue(fromNode.nodeGUID, out var fromView))
                return;

            if (!nodeViews.TryGetValue(toNode.nodeGUID, out var toView))
                return;

            if (outputIndex >= fromView.outputContainer.childCount)
                return;

            var outputPort = fromView.outputContainer[outputIndex] as Port;
            var inputPort = toView.inputContainer[0] as Port;

            if (outputPort == null || inputPort == null)
                return;

            var edge = outputPort.ConnectTo(inputPort);
            AddElement(edge);
        }

        private void ValidateIndices()
        {
            if (editor == null || editor.CurrentDialogue == null)
                return;

            if (editor.CurrentDialogue.ValidateIndices(out string errorMessage))
            {
                EditorUtility.DisplayDialog("Validation Success", "All indices are unique!", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Validation Failed", errorMessage, "OK");
            }
        }
    }

    /// <summary>
    /// Pan do canvas com right click drag.
    /// O ContentDragger nativo da Unity não suporta MouseButton.RightMouse,
    /// por isso implementamos o comportamento manualmente.
    /// </summary>
    public class RightClickPanManipulator : MouseManipulator
    {
        private Vector2 startMousePosition;
        private Vector3 startContentOffset;
        private bool isDragging;

        public RightClickPanManipulator()
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.RightMouse });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (!CanStartManipulation(evt))
                return;

            var graphView = target as GraphView;
            if (graphView == null)
                return;

            isDragging = true;
            startMousePosition = evt.localMousePosition;
            startContentOffset = graphView.contentViewContainer.transform.position;

            target.CaptureMouse();
            evt.StopPropagation();
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (!isDragging)
                return;

            var graphView = target as GraphView;
            if (graphView == null)
                return;

            var delta = (Vector2)evt.localMousePosition - startMousePosition;
            var newPos = startContentOffset + (Vector3)delta;

            graphView.contentViewContainer.transform.position = newPos;
            evt.StopPropagation();
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            if (!isDragging || !CanStopManipulation(evt))
                return;

            isDragging = false;
            target.ReleaseMouse();
            evt.StopPropagation();
        }
    }

}