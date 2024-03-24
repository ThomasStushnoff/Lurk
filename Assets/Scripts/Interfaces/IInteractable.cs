using Entities;
namespace Interfaces
{
    public interface IInteractable
    {
        void BeginInteract(BaseEntity entity);
        void EndInteract();
    }
}
