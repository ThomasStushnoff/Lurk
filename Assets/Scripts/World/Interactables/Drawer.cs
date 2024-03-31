using Audio;
using Entities;
using Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace World.Interactables
{
    public class Drawer : BaseObject, IInteractable
    {
        [TitleHeader("Drawer Settings")] 
        [SerializeField] private Vector3 openPositionOffset; 
        [SerializeField] private float duration = 0.5f;
        [ReadOnly] public bool isOpen;
        
        [TitleHeader("Audio Settings")]
        [SerializeField, CanBeNull] private AudioDataEnumSoundFx openSoundFx;
        [SerializeField, CanBeNull] private AudioDataEnumSoundFx closeSoundFx;
        
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _timeElapsed;
        private bool _isInteracting;

        private void Start()
        {
            isOpen = false;
            var position = transform.localPosition;
            _startPosition = position;
        }
        
        private void Update()
        {
            if (_isInteracting)
            {
                if (_timeElapsed < duration)
                {
                    _timeElapsed += Time.deltaTime;
                    transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPosition, _timeElapsed / duration);
                }
            }
        }

        public void BeginInteract(BaseEntity entity)
        {
            _isInteracting = true;
            
            if (_timeElapsed > duration)
                audioSource.PlaySoundFx(isOpen ? openSoundFx : closeSoundFx);
            
            isOpen = !isOpen;
            
            _targetPosition = isOpen ? _startPosition + openPositionOffset : _startPosition;
            
            _timeElapsed = 0.0f;
        }
        
        public void EndInteract()
        {
            _isInteracting = false;
            transform.localPosition = _targetPosition;
        }
    }
}