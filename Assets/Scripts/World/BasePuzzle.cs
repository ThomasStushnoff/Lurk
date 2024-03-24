using Entities;
using Interfaces;
namespace World
{
    public class BasePuzzle : BaseObject, IInteractable
    { 
        public PuzzleState State { get; protected set; }
        public PuzzleLockState LockState { get; protected set; }
        public PuzzleType Type { get; protected set; }
        
        public virtual void BeginInteract(BaseEntity entity) { }
        public virtual void EndInteract() { }
    }
    
    public enum PuzzleState
    {
        Incomplete,
        Completed,
    }

    public enum PuzzleLockState
    {
        Locked,
        Unlocked,
    }

    public enum PuzzleType
    {
        Reflector,
    }
}