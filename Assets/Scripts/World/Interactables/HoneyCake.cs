﻿using Audio;
using Entities;
using Entities.Player;
using Interfaces;
using JetBrains.Annotations;
using UnityEngine;
using World.Environmental;

namespace World.Interactables
{
    [RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
    public class HoneyCake : MonoBehaviour, IInteractable, IGrabbable
    {
        // public int score = 10;
        [SerializeField, CanBeNull] private Deposit deposit;
        [SerializeField] private LayerMask depositLayer;
        [SerializeField] private bool depositAnywhere;
        [SerializeField] private AudioDataEnumSoundFx pickupSound;
        
        private Transform _t;
        private Rigidbody _rb;
        private AudioSource _audioSource;
        private Transform _defaultParent;
        private Quaternion _defaultRotation;
        private PlayerController _player;
        private bool _isPlaced;
        
        private void Awake()
        {
            _t = transform;
            _rb = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();
        }
        
        public void BeginInteract(BaseEntity entity)
        {
            if (_isPlaced) return;
            
            PickUp(entity);
        }
        
        public void EndInteract()
        {
            if (_isPlaced) return;
            
            if (depositAnywhere)
            {
                if (Physics.Raycast(_player.cameraTransform.position, _player.cameraTransform.forward, out var hit,
                        _player.settings.interactDropDistance, depositLayer))
                {
                    var dep = hit.collider.GetComponent<Deposit>();
                    if (dep != null)
                    {
                        Place(hit.point + hit.transform.up * 0.5f, hit.transform.rotation);
                        dep.DepositItem();
                        _isPlaced = true;
                    }
                    else
                    {
                        Drop();
                    }
                }
                else
                {
                    Drop();
                }
            }
            else
            {
                if (Physics.Raycast(_player.cameraTransform.position, _player.cameraTransform.forward, out var hit,
                        _player.settings.interactDropDistance, depositLayer))
                {
                    var dep = hit.collider.GetComponent<Deposit>();
                    if (dep == deposit)
                    {
                        Place(hit.point + hit.transform.up * 0.5f, hit.transform.rotation);
                        dep!.DepositItem();
                        _isPlaced = true;
                    }
                    else
                    {
                        Drop();
                    }
                }
                else
                {
                    Drop();
                }
            }
        }
        
        public void PickUp(BaseEntity entity)
        {
            _player = entity as PlayerController;
            if (_player == null) return;
            
            if (!_audioSource.isPlaying)
                _audioSource.PlayOneShot(pickupSound);
            
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
            
            _t.SetParent(_defaultParent);
            _t.position = position;
            _t.rotation = rotation;
            _rb.isKinematic = false;
            _player = null;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareLayer("Deposit")) return;
            
            var bounds = other.collider.bounds;
            _t.position = bounds.center + bounds.extents.y * Vector3.up;
            _rb.isKinematic = true;
        }
    }
}