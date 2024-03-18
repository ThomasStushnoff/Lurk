using Interfaces;

namespace StateMachines
{
    public class SneakingState : BaseState<IBaseEntity>
    {
        public SneakingState(IBaseEntity baseEntity) : base("Sneaking", baseEntity) { }
        
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