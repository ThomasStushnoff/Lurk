using Interfaces;

namespace StateMachines
{
    public class InspectingState : BaseState<IBaseEntity>
    {
        public InspectingState(IBaseEntity baseEntity) : base("Inspecting", baseEntity) { }
        
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