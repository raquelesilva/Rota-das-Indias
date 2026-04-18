using UnityEngine;

namespace AstrolabeSystem
{
    public interface IDraggable
    {
        string DraggableID { get; }
        void OnPickUp();
        void OnDrop(bool snapped);
        void OnDrag(Vector3 worldPosition);
    }
}