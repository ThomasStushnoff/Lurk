using Interfaces;
using StateMachines;
using UnityEngine;

namespace Entities
{
    public abstract class BaseEntity : MonoBehaviour, IBaseEntity
    {
        public uint runtimeID;
        [SerializeField] protected EntityType entityType;
        
        // Cache commonly used components.
        protected Animator Animator;
        protected Rigidbody Rigidbody;
        protected AudioSource AudioSource;
        
        protected virtual void Awake()
        {
            runtimeID = (uint)GetInstanceID();
            Animator = TryGetComponent<Animator>(out var anim) ? anim : null;
            Rigidbody = TryGetComponent<Rigidbody>(out var rb) ? rb : null;
            AudioSource = TryGetComponent<AudioSource>(out var src) ? src : null;
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