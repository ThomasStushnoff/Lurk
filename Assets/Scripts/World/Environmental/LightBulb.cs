using Audio;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace World.Environmental
{
    [RequireComponent(typeof(AudioSource))]
    public class LightBulb : MonoBehaviour
    {
        [SerializeField] private AudioDataEnumSoundFx defaultAudio;
        [SerializeField] private AudioDataEnumSoundFx roomChangeAudio;
        public UnityEvent onRoomChange;
        
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            AudioManager.Instance.RegisterAudioSource(_audioSource, defaultAudio);
            
            onRoomChange.AddListener(RoomChange);
        }
        
        private void OnDestroy()
        {
            AudioManager.Instance.UnregisterAudioSource(_audioSource);
            
            onRoomChange.RemoveListener(RoomChange);
        }
        
        private void RoomChange()
        {
            AudioManager.Instance.UnregisterAudioSource(_audioSource);
            _audioSource.PlayOneShot(roomChangeAudio);
        }
    }
}