namespace Interfaces
{
    public interface IInspectable
    {
        // Used for reflector or other rotatable objects.
        bool IsRotatable { get; }
        float RotationSpeed { get; }
        
        // Method called when the player starts inspecting the object.
        void OnInspectBegin();
        
        // Method called when the player finishes inspecting the object.
        void OnInspectEnd();
        
        // Method called to return the information about the object.
        // public string GetDescription();
    }
}