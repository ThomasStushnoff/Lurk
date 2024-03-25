using Entities;
using Entities.Player;
using Interfaces;
using Objects;
using UnityEngine;

namespace World.Interactables
{
    [RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
    public class HoneyCake : MonoBehaviour, IInteractable, IGrabbable
    {
        public int score = 10;
        [SerializeField] private AudioData _pickupSound;
        
        private Transform _t;
        private Rigidbody _rb;
        private AudioSource _audioSource;
        private Transform _defaultParent;
        private Quaternion _defaultRotation;
        private PlayerController _player;
        
        private void Awake()
        {
            _t = transform;
            _rb = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();
        }
        
        public void BeginInteract(BaseEntity entity)
        {
            PickUp(entity);
        }
        
        public void EndInteract()
        {
            Drop();
            // TODO:
            // Deposit logic then call Place method.
        }
        
        public void PickUp(BaseEntity entity)
        {
            _player = entity as PlayerController;
            if (_player == null) return;
            
            if (!_audioSource.isPlaying)
                _audioSource.PlayOneShot(_pickupSound);
            
            _defaultParent = _t.parent;
            _defaultRotation = _t.rotation;
            
            _t.SetParent(_player.itemHoldTransform.transform);
            _t.position = _player.itemHoldTransform.position;
            _rb.isKinematic = true;
        }
        
        public void Drop()
        {
            if (_player == null) return;
            
            _t.SetParent(_defaultParent);
            _t.position = _player.transform.forward + _player.transform.position;
            _t.rotation = _defaultRotation;
            _rb.isKinematic = false;
            _player = null;
        }
        
        public void Place(Vector3 position, Quaternion rotation)
        {
            if (_player == null) return;
            
            _t.SetParent(null);
            _t.position = position;
            _t.rotation = rotation;
            _rb.isKinematic = false;
            _player = null;
        }
    }
}