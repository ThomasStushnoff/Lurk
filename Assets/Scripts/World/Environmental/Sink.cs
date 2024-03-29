using Audio;
using Managers;
using UnityEngine;

namespace World.Environmental
{
    [RequireComponent(typeof(AudioSource))]
    public class Sink : MonoBehaviour
    {
        [SerializeField] private AudioDataEnumSoundFx soundFx;
        
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            AudioManager.Instance.RegisterAudioSource(_audioSource, soundFx);
        }
    }
}