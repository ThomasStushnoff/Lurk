using Interfaces;

namespace StateMachines
{
    public class IdleState : BaseState<IBaseEntity>
    {
        public IdleState(IBaseEntity baseEntity) : base("Idle", baseEntity) { }
        
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