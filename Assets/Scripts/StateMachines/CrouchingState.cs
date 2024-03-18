using Interfaces;

namespace StateMachines
{
    public class CrouchingState : BaseState<IBaseEntity>
    {
        public CrouchingState(IBaseEntity baseEntity) : base("Crouching", baseEntity) { }
        
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