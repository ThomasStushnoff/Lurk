namespace Interfaces
{
    public interface IInspectable
    {
        // Method called when the player starts inspecting the object.
        public void OnInspectBegin();
        
        // Method called when the player finishes inspecting the object.
        public void OnInspectEnd();
        
        // Method called to return the information about the object.
        // public string GetDescription();
        
        // Used for reflector or other rotatable objects.
        bool IsRotatable { get; }
        float RotationSpeed { get; }
    }
}