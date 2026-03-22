namespace FancyCrab.CoreSystems.InteractionSystem
{
    public interface IGrabbable 
    {
        public void OnGrab();
        public void OnDrop();
        public void OnThrow();
    }
}
