using Managers;
using Objects;
using UnityEngine;

namespace World.Environmental
{
    [RequireComponent(typeof(AudioSource))]
    public class Sink : MonoBehaviour
    {
        [SerializeField] private AudioData audioData;
        
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            AudioManager.Instance.RegisterAudioSource(_audioSource, audioData);
        }
    }
}