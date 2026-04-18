using UnityEngine;

namespace AstrolabeSystem
{
    [RequireComponent(typeof(Collider))]
    public class SnapZone : MonoBehaviour, ISnappable
    {
        [Header("Config")]
        [SerializeField] private string acceptedID;

        public string AcceptedID => acceptedID;
        public bool IsOccupied { get; private set; }
        public Vector3 SnapPosition => transform.position;
        public Transform SnapTransform => transform;

        public void OnSnap(IDraggable draggable)
        {
            IsOccupied = true;
            GetComponent<Collider>().enabled = false;

            if (draggable is DragAstrolabio dragAstrolabio)
            {
                dragAstrolabio.SnapTo(transform);
            }
        }
    }
}