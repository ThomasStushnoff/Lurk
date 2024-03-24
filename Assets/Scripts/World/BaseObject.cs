using Interfaces;
using UnityEngine;

namespace World
{
    public abstract class BaseObject : MonoBehaviour, IBaseObject
    {
        public uint runtimeID;
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
    }
}