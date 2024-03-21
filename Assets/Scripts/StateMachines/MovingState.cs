using Entities.Player;
using Interfaces;

namespace StateMachines
{
    public class MovingState : BaseState<IBaseEntity>
    {
        private PlayerController _player;
        
        public MovingState(IBaseEntity owner) : base("Moving", owner)
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