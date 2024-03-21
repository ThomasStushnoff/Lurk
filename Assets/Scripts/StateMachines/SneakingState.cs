using Entities.Player;
using Interfaces;

namespace StateMachines
{
    public class SneakingState : BaseState<IBaseEntity>
    {
        private PlayerController _player;
        
        public SneakingState(IBaseEntity owner) : base("Sneaking", owner)
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