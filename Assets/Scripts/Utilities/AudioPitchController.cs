using UnityEngine;

namespace Utilities
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPitchController : MonoBehaviour
    {
        public PitchType pitchType = PitchType.Random;
        
        [Tooltip("The minimum pitch value")]
        [Range(-3.0f, 3.0f)]
        public float minPitch = 0.5f;
        
        [Tooltip("The maximum pitch value")]
        [Range(-3.0f, 3.0f)]
        public float maxPitch = 3.0f;
        
        [Tooltip("The pitch value to increase or decrease by")]
        [Range(0.1f, 2.0f)]
        public float pitchStep = 0.1f;
        
        private AudioSource _audioSource;
        private float _currentPitch;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _currentPitch = _audioSource.pitch;
        }

        private void OnEnable() => AdjustAudioPitch();

        private void AdjustAudioPitch()
        {
            switch (pitchType)
            {
                case PitchType.Random:
                    _audioSource.pitch = Random.Range(minPitch, maxPitch);
                    break;
                case PitchType.Increase:
                    _currentPitch += pitchStep;
                    if (_currentPitch > maxPitch)
                        _currentPitch = maxPitch;
                    _audioSource.pitch = _currentPitch;
                    break;
                case PitchType.Decrease:
                    _currentPitch -= pitchStep;
                    if (_currentPitch < minPitch)
                        _currentPitch = minPitch;
                    _audioSource.pitch = _currentPitch;
                    break;
                default:
                    _currentPitch = _audioSource.pitch;
                    break;
            }
        }
    }
    
    public enum PitchType
    {
        Random,
        Increase,
        Decrease
    }
}