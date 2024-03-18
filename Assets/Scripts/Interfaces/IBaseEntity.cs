using Entities;
using StateMachines;

namespace Interfaces
{
    public interface IBaseEntity
    {
        uint RuntimeID { get; }
        // bool IsAlive { get; }
        EntityType EntityType { get; }
        
        void ChangeState(BaseState<IBaseEntity> newState);
    }
}
