using Entities.Player;
using Interfaces;

namespace StateMachines
{
    public class VaultState : BaseState<IBaseEntity>
    {
        private PlayerController _player;
        
        public VaultState(IBaseEntity owner) : base("Vault", owner)
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