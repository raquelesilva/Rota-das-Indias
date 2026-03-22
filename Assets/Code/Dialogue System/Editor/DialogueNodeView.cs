using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

namespace FancyCrab.DialogueSystem.Editor
{
    public class DialogueNodeView : Node
    {
        public DialogueNode node;
        private DialogueGraphView graphView;

        public DialogueNodeView(DialogueNode node, DialogueGraphView graphView)
        {
            this.node = node;
            this.graphView = graphView;

            title = node.name;
            style.backgroundColor = GetNodeColor(node.GetType());

            style.borderTopWidth = 2;
            style.borderBottomWidth = 2;
            style.borderLeftWidth = 2;
            style.borderRightWidth = 2;
            style.borderTopColor = new Color(1f, 0.5f, 0f);
            style.borderBottomColor = new Color(1f, 0.5f, 0f);
            style.borderLeftColor = new Color(1f, 0.5f, 0f);
            style.borderRightColor = new Color(1f, 0.5f, 0f);

            style.width = 300;

            mainContainer.style.flexWrap = Wrap.Wrap;
            mainContainer.style.maxWidth = 280;
            extensionContainer.style.maxWidth = 280;
            inputContainer.style.maxWidth = 280;
            outputContainer.style.maxWidth = 280;

            if (node.HasActor)
            {
                var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Port));
                inputPort.portName = "In";
                inputContainer.Add(inputPort);

                var actorField = new ObjectField("Actor")
                {
                    objectType = typeof(DialogueActor),
                    value = node.actor
                };
                actorField.style.maxWidth = 260;
                actorField.RegisterValueChangedCallback(evt => node.actor = evt.newValue as DialogueActor);
                mainContainer.Add(actorField);
            }

            CreateNodeSpecificFields();

            RefreshExpandedState();
            RefreshPorts();
        }

        // Corrigido: switch expression em vez de if/else if encadeado
        private Color GetNodeColor(System.Type nodeType) => nodeType switch
        {
            _ when nodeType == typeof(TextDialogueNode) => new Color(0.2f, 0.3f, 0.5f),
            _ when nodeType == typeof(ChoiceDialogueNode) => new Color(0.5f, 0.3f, 0.2f),
            _ when nodeType == typeof(IndexDialogueNode) => new Color(0.3f, 0.5f, 0.2f),
            _ => new Color(0.3f, 0.3f, 0.3f)
        };

        private void CreateNodeSpecificFields()
        {
            if (node is TextDialogueNode textNode)
            {
                CreateTextNodeFields(textNode);
            }
            else if (node is ChoiceDialogueNode choiceNode)
            {
                CreateChoiceNodeFields(choiceNode);
            }
            else if (node is IndexDialogueNode indexNode)
            {
                CreateIndexNodeFields(indexNode);
                return;
            }

            CreateEventsSection();
        }

        private void CreateEventsSection()
        {
            var foldout = new Foldout
            {
                text = "Events",
                value = false
            };
            foldout.style.maxWidth = 260;
            foldout.style.marginTop = 4;

            var enterField = new TextField("On Enter")
            {
                value = node.onNodeEnterEvent,
                tooltip = "ID do trigger a invocar quando este node é activado"
            };
            enterField.style.maxWidth = 250;
            enterField.RegisterValueChangedCallback(evt =>
            {
                node.onNodeEnterEvent = evt.newValue;
                EditorUtility.SetDirty(node);
            });

            var exitField = new TextField("On Exit")
            {
                value = node.onNodeExitEvent,
                tooltip = "ID do trigger a invocar quando este node é abandonado"
            };
            exitField.style.maxWidth = 250;
            exitField.RegisterValueChangedCallback(evt =>
            {
                node.onNodeExitEvent = evt.newValue;
                EditorUtility.SetDirty(node);
            });

            foldout.Add(enterField);
            foldout.Add(exitField);
            mainContainer.Add(foldout);
        }

        private void CreateTextNodeFields(TextDialogueNode textNode)
        {
            var textField = new TextField("Dialogue Text")
            {
                multiline = true,
                value = textNode.dialogueText
            };
            textField.style.height = 100;
            textField.style.maxWidth = 260;
            textField.style.whiteSpace = WhiteSpace.Normal;
            textField.RegisterValueChangedCallback(evt =>
            {
                textNode.dialogueText = evt.newValue;
                EditorUtility.SetDirty(textNode);
            });
            mainContainer.Add(textField);

            var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
            outputPort.portName = "Next";
            outputContainer.Add(outputPort);
        }

        private void CreateChoiceNodeFields(ChoiceDialogueNode choiceNode)
        {
            for (int i = 0; i < 4; i++)
            {
                int index = i;

                var choiceField = new TextField($"Choice {i + 1}")
                {
                    multiline = true,
                    value = choiceNode.Choices[index].choiceText
                };
                choiceField.style.height = 60;
                choiceField.style.maxWidth = 260;
                choiceField.style.whiteSpace = WhiteSpace.Normal;
                choiceField.RegisterValueChangedCallback(evt =>
                {
                    var choices = choiceNode.Choices;
                    var newChoice = choices[index];
                    newChoice.choiceText = evt.newValue;
                    choices[index] = newChoice;

                    var fieldInfo = typeof(ChoiceDialogueNode).GetField(
                        "choices",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(choiceNode, choices);
                        EditorUtility.SetDirty(choiceNode);
                    }
                });
                mainContainer.Add(choiceField);

                var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
                outputPort.portName = $"Option {i + 1}";
                outputContainer.Add(outputPort);
            }
        }

        private void CreateIndexNodeFields(IndexDialogueNode indexNode)
        {
            var indexField = new IntegerField("Index Value")
            {
                value = indexNode.indexValue
            };
            indexField.style.maxWidth = 260;
            indexField.RegisterValueChangedCallback(evt =>
            {
                indexNode.indexValue = evt.newValue;
                indexNode.UpdateName();
                title = indexNode.name;
                EditorUtility.SetDirty(indexNode);

                // Invalida o cache do container pai quando o indexValue muda
                if (graphView?.CurrentDialogue != null)
                    graphView.CurrentDialogue.InvalidateCache();
            });
            mainContainer.Add(indexField);

            var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
            outputPort.portName = "Next";
            outputContainer.Add(outputPort);
        }

        public void UpdateTitle()
        {
            title = node.name;
        }
    }
}