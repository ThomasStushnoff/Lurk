using Audio;
using Entities;
using Interfaces;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace World.Interactables
{
    public class Door : BaseObject, IInteractable
    {
        [TitleHeader("Door Settings")]
        [SerializeField] private Vector3 openRotation = new Vector3(0, 90, 0);
        [SerializeField] private Vector3 closedRotation = new Vector3(0, 0, 0);
        // [SerializeField, Tooltip("Speed at which the door snaps open or closed.")] private float snapSpeed = 4.0f;
        [SerializeField, Tooltip("The duration the door takes to open or close.")] private float holdDuration = 10.0f;
        [ReadOnly] public bool isOpen;
        
        [TitleHeader("Audio Settings")]
        [SerializeField, CanBeNull] private AudioDataEnumSoundFx openSoundFx;
        [SerializeField, CanBeNull] private AudioDataEnumSoundFx closeSoundFx;
        [SerializeField, CanBeNull] private AudioDataEnumSoundFx slamSoundFx;

        public UnityEvent onSlam;
        
        private bool _isInteracting;
        private float _interactTime;
        private Vector3 _relativeOpenRotation;
        private Vector3 _relativeClosedRotation;
        private Quaternion _targetRotation;
        
        protected override void Awake()
        {
            base.Awake();
            isOpen = false;
            _isInteracting = false;
            _interactTime = 0.0f;
        }
        
        private void Start()
        {
            _relativeOpenRotation = new Vector3(openRotation.x != 0 ? openRotation.x : transform.rotation.eulerAngles.x,
                openRotation.y != 0 ? openRotation.y : transform.rotation.eulerAngles.y,
                openRotation.z != 0 ? openRotation.z : transform.rotation.eulerAngles.z);
            _relativeClosedRotation = new Vector3(closedRotation.x != 0 ? closedRotation.x : transform.rotation.eulerAngles.x,
                closedRotation.y != 0 ? closedRotation.y : transform.rotation.eulerAngles.y,
                closedRotation.z != 0 ? closedRotation.z : transform.rotation.eulerAngles.z);
            
            onSlam.AddListener(Slam);
        }
        
        private void Update()
        {
            if (_isInteracting)
            {
                // Lerp the door to the target rotation.
                var holdProgress = Mathf.Clamp01(_interactTime / holdDuration);
                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, holdProgress);
                
                if (_interactTime < holdDuration)
                    _interactTime += Time.deltaTime;
                else
                    onSlam?.Invoke();
            }
        }
        
        public void BeginInteract(BaseEntity entity)
        {
            _isInteracting = true;
            isOpen = !isOpen;
            _targetRotation = isOpen ? Quaternion.Euler(_relativeOpenRotation) : Quaternion.Euler(_relativeClosedRotation);
            
            if (isOpen)
            {
                if (openSoundFx is not AudioDataEnumSoundFx.None)
                    audioSource.PlaySoundFx(openSoundFx);
            }
            else
            {
                if (closeSoundFx is not AudioDataEnumSoundFx.None)
                    audioSource.PlaySoundFx(closeSoundFx);
            }
        }
        
        public void EndInteract()
        {
            onSlam?.Invoke();
        }
        
        private void Slam()
        {
            _isInteracting = false;
            _interactTime = 0.0f;
            // transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, snapSpeed * Time.deltaTime);
            transform.rotation = _targetRotation;
            if (slamSoundFx is not AudioDataEnumSoundFx.None) audioSource.PlaySoundFx(slamSoundFx);
        }
    }
}