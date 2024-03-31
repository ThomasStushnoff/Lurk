using Controllers;
using Interfaces;

namespace StateMachines
{
    public class CrouchingState : BaseState<IBaseEntity>
    {
        private PlayerController _player;
        
        public CrouchingState(IBaseEntity owner) : base("Crouching", owner)
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