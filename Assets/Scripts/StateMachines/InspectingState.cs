using Entities.Player;
using Interfaces;

namespace StateMachines
{
    public class InspectingState : BaseState<IBaseEntity>
    {
        private PlayerController _player;
        
        public InspectingState(IBaseEntity owner) : base("Inspecting", owner)
        {
            _player = owner as PlayerController;
        }
        
        public override void EnterState()
        {
            throw new System.NotImplementedException();
        }
        
        public override void UpdateState()
        {
            throw new System.NotImplementedException();
        }
        
        public override void ExitState()
        {
            throw new System.NotImplementedException();
        }
    }
}