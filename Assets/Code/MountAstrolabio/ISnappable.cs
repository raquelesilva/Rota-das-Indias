using UnityEngine;

namespace AstrolabeSystem
{
    public interface ISnappable
    {
        string AcceptedID { get; }
        bool IsOccupied { get; }
        Vector3 SnapPosition { get; }
        Transform SnapTransform { get; }
        void OnSnap(IDraggable draggable);
    }
}