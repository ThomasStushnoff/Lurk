using Entities.Player;
using Interfaces;

namespace StateMachines
{
    public class IdleState : BaseState<IBaseEntity>
    {
        private PlayerController _player;

        public IdleState(IBaseEntity owner) : base("Idle", owner)
        {
            _player = owner as PlayerController;
        }
        
        public override void EnterState()
        {
            _player.animator.SetBool("Grounded", true);
        }
        
        public override void UpdateState()
        {
            
        }
        
        public override void ExitState()
        {
            
        }
    }
}