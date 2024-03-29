﻿using Interfaces;
using StateMachines;
using UnityEngine;

namespace Entities
{
    public abstract class BaseEntity : MonoBehaviour, IBaseEntity
    {
        [ReadOnly] public uint runtimeID;
        [SerializeField] protected EntityType entityType;
        
        // Cache commonly used components.
        [HideInInspector] public Animator animator;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public AudioSource audioSource;
        
        protected virtual void Awake()
        {
            runtimeID = (uint)GetInstanceID();
            animator = TryGetComponent<Animator>(out var anim) ? anim : null;
            rb = TryGetComponent<Rigidbody>(out var r) ? r : null;
            audioSource = TryGetComponent<AudioSource>(out var src) ? src : null;
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