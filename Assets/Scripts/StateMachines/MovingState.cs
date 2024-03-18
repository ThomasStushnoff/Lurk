using Interfaces;

namespace StateMachines
{
    public class MovingState : BaseState<IBaseEntity>
    {
        public MovingState(IBaseEntity baseEntity) : base("Moving", baseEntity) { }
        
        public override void EnterState()
        {
            throw new System.NotImplementedException();
        }
        
        public override void ExitState()
        {
            throw new System.NotImplementedException();
        }
        
        public override void UpdateState()
        {
            throw new System.NotImplementedException();
        }
    }
}