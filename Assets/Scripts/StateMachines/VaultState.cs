using Interfaces;

namespace StateMachines
{
    public class VaultState : BaseState<IBaseEntity>
    {
        public VaultState(IBaseEntity baseEntity) : base("Vault", baseEntity) { }
        
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