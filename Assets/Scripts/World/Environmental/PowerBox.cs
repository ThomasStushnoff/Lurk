using Audio;
using Managers;
using UnityEngine;

namespace World.Environmental
{
    [RequireComponent(typeof(AudioSource))]
    public class PowerBox : MonoBehaviour
    {
        [SerializeField] private AudioDataEnumSoundFx sparkSound;
        
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            AudioManager.Instance.RegisterAudioSource(_audioSource, sparkSound);
        }
    }
}