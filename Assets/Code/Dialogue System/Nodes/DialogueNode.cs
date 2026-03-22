using UnityEngine;
using UnityEngine.Events;

namespace FancyCrab.DialogueSystem
{
    public abstract class DialogueNode : ScriptableObject
    {
        [HideInInspector] public string nodeGUID;
        [HideInInspector] public Vector2 position;

        [Header("Node Settings")]
        public DialogueActor actor;
        public AudioClip nodeAudio;

        [Header("Node Index")]
        public int nodeIndex = -1;

        [Header("Node Events")]
        public UnityEvent onNodeEnter;
        public UnityEvent onNodeExit;

        public string onNodeEnterEvent;
        public string onNodeExitEvent;

        public virtual bool HasActor => true;
    }
}