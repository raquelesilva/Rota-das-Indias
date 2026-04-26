namespace FancyCrab.CoreSystems.InteractionSystem
{
    public enum RaycastState
    {
        None,
        Grabbable,
        Interactable,
        Inspectable,
        Both
    }

    public enum InteractionState
    {
        None,
        Holding,
        Interacting,
        Inspecting
    }
}