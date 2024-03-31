using Audio;
using Entities;
using Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace World.Interactables
{
    public class Chest : BaseObject, IInteractable
    {
        [TitleHeader("Chest Settings")]
        [SerializeField] private Transform lid;
        // [SerializeField] private GameObject lockObject;
        [SerializeField] private Vector3 openRotation = new Vector3(-90, 0, 0);
        [SerializeField] private float duration = 0.5f;
        [ReadOnly] public bool isOpen;
        
        [TitleHeader("Audio Settings")]
        [SerializeField, CanBeNull] private AudioDataEnumSoundFx openSoundFx;
        [SerializeField, CanBeNull] private AudioDataEnumSoundFx closeSoundFx;
        
        private Quaternion _startRotation;
        private Quaternion _targetRotation;
        private float _timeElapsed;
        private bool _isInteracting;
        
        private void Start()
        {
            isOpen = false;
            var rotation = lid.localRotation;
            _startRotation = Quaternion.Euler(rotation.eulerAngles);
        }
        
        private void Update()
        {
            if (_isInteracting)
            {
                if (_timeElapsed < duration)
                {
                    _timeElapsed += Time.deltaTime;
                    lid.localRotation = Quaternion.Lerp(lid.localRotation, _targetRotation, _timeElapsed / duration);
                }
            }
        }
        
        public void BeginInteract(BaseEntity entity)
        {
            _isInteracting = true;
            
            isOpen = !isOpen;
            
            audioSource.PlaySoundFx(isOpen ? openSoundFx : closeSoundFx);
            
            _targetRotation = isOpen ? Quaternion.Euler(openRotation) : _startRotation;
            
            _timeElapsed = 0.0f;
        }
        
        public void EndInteract()
        {
            _isInteracting = false;
            lid.localRotation = _targetRotation;
        }
    }
}