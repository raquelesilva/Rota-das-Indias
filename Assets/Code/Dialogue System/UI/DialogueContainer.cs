using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FancyCrab.DialogueSystem
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = StudioInfo.ASSET_MENU_PATH + "Dialogue Container")]
    public class DialogueContainer : ScriptableObject
    {
        [Header("Dialogue Settings")]
        public string dialogueID;
        public string dialogueTitle;

        [TextArea(2, 5)]
        public string description;

        [Header("Nodes")]
        public List<DialogueNode> nodes = new List<DialogueNode>();
        public DialogueNode startNode;

        [Header("Audio")]
        public AudioClip backgroundMusic;
        public float backgroundMusicVolume = 1f;

        [Header("Events")]
        public string onDialogueStartEvent;
        public string onDialogueEndEvent;

        // Cache do dicionário e versão para invalidação correcta
        private Dictionary<int, IndexDialogueNode> indexNodeCache;
        private int cachedNodeCount = -1;

        // Obtém um node pelo seu índice (apenas IndexNodes)
        public DialogueNode GetNodeByIndex(int index)
        {
            RebuildIndexDictionaryIfNeeded();

            if (indexNodeCache.TryGetValue(index, out IndexDialogueNode indexNode))
                return indexNode.nextNode != null ? indexNode.nextNode : indexNode;

            Debug.LogWarning($"[FancyCrabStudios] No index node found with value: {index}");
            return null;
        }

        // Obtém todos os IndexNodes
        public List<IndexDialogueNode> GetIndexNodes()
        {
            return nodes.OfType<IndexDialogueNode>().ToList();
        }

        // Verifica se um índice específico existe
        public bool HasIndex(int index)
        {
            RebuildIndexDictionaryIfNeeded();
            return indexNodeCache.ContainsKey(index);
        }

        // Encontra o próximo índice disponível
        public int GetNextAvailableIndex()
        {
            var usedIndices = GetIndexNodes().Select(n => n.indexValue).ToHashSet();
            int nextIndex = 0;

            while (usedIndices.Contains(nextIndex))
            {
                nextIndex++;
            }

            return nextIndex;
        }

        // Força invalidação manual do cache (chamar após editar nodes no editor)
        public void InvalidateCache()
        {
            cachedNodeCount = -1;
            indexNodeCache = null;
        }

        // Reconstrói o dicionário apenas quando necessário
        private void RebuildIndexDictionaryIfNeeded()
        {
            var currentIndexNodes = GetIndexNodes();

            // Invalida se o count mudou — para mudanças de indexValue usa InvalidateCache() manualmente
            if (indexNodeCache != null && currentIndexNodes.Count == cachedNodeCount) return;

            indexNodeCache = new Dictionary<int, IndexDialogueNode>();
            cachedNodeCount = currentIndexNodes.Count;

            foreach (var node in currentIndexNodes)
            {
                if (!indexNodeCache.ContainsKey(node.indexValue))
                {
                    indexNodeCache[node.indexValue] = node;
                }
                else
                {
                    Debug.LogWarning($"[FancyCrabStudios] Duplicate index value found: {node.indexValue} in node {node.name}");
                }
            }
        }

        // Valida se não há índices duplicados
        public bool ValidateIndices(out string errorMessage)
        {
            var indexNodes = GetIndexNodes();
            var duplicateIndices = indexNodes.GroupBy(n => n.indexValue).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

            if (duplicateIndices.Count > 0)
            {
                errorMessage = $"Duplicate indices found: {string.Join(", ", duplicateIndices)}";
                return false;
            }

            errorMessage = "";
            return true;
        }
    }
}