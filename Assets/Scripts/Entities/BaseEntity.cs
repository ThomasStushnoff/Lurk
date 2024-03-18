using Interfaces;
using StateMachines;
using UnityEngine;

namespace Entities
{
    public abstract class BaseEntity : MonoBehaviour, IBaseEntity
    {
        protected uint runtimeID;
        [SerializeField] protected EntityType entityType;
        protected Animator animator;
        
        protected virtual void Awake()
        {
            runtimeID = (uint)GetInstanceID();
            animator = GetComponent<Animator>();
        }
        
        public uint RuntimeID => runtimeID;
        public EntityType EntityType => entityType;
        
        public abstract void ChangeState(BaseState<IBaseEntity> newState);
    }
    
    public enum EntityType
    {
        Player,
        Enemy,
    }
}