using System;
using Managers;
using Objects;
using UnityEngine;

namespace World.Environmental
{
    [RequireComponent(typeof(AudioSource))]
    public class LightBulb : MonoBehaviour
    {
        [SerializeField] private AudioData defaultAudio;
        [SerializeField] private AudioData roomChangeAudio;
        public Action OnRoomChange; // Triggered after you read and put the page down.
        
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            AudioManager.Instance.RegisterAudioSource(_audioSource, defaultAudio);
            
            OnRoomChange += RoomChange;
        }
        
        private void RoomChange()
        {
            AudioManager.Instance.UnregisterAudioSource(_audioSource);
            _audioSource.PlayOneShot(roomChangeAudio);
        }
    }
}