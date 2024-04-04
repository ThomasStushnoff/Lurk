using Interfaces;
using JetBrains.Annotations;
using Managers;
using Objects;
using UnityEngine;

namespace World
{
    public abstract class BaseObject : MonoBehaviour, IBaseObject
    {
        [ReadOnly] public uint runtimeID;
        [HideInInspector] public Animator animator;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public AudioSource audioSource;
        [CanBeNull] public TooltipData tooltipData;
        
        protected virtual void Awake()
        {
            runtimeID = (uint)GetInstanceID();
            animator = TryGetComponent<Animator>(out var anim) ? anim : null;
            rb = TryGetComponent<Rigidbody>(out var r) ? r : null;
            audioSource = TryGetComponent<AudioSource>(out var src) ? src : null;
        }
        
        public virtual void OnRaycastEnter()
        {
            if (!tooltipData) return;
            
            TooltipManager.Instance.SetTooltipData(tooltipData);
            TooltipManager.Instance.ShowTooltip();
        }
        
        public virtual void OnRaycastExit()
        {
            if (!tooltipData) return;
            
            TooltipManager.Instance.HideTooltip();
        }
        
        public uint RuntimeID => runtimeID;
    }
}