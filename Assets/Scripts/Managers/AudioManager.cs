using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Objects;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Utils;

namespace Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        public AudioMixer audioMixer;
        private struct AudioSourceInfo
        {
            public readonly AudioSource Source;
            public readonly Transform ObjTransform;
            
            public AudioSourceInfo(AudioSource source, Transform objTransform)
            {
                Source = source;
                ObjTransform = objTransform;
            }
        }
        
        private List<AudioSourceInfo> _audioSources = new List<AudioSourceInfo>();
        
        /// <summary>
        /// Special singleton initializer method.
        /// </summary>
        public new static void Initialize()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Managers/AudioManager");
            if (prefab == null) throw new Exception("Missing AudioManager prefab!");
            
            var instance = Instantiate(prefab);
            if (instance == null) throw new Exception("Failed to instantiate AudioManager prefab!");
            
            instance.name = "Managers.AudioManager (Singleton)";
        }
        
        protected override void OnSceneUnloaded(Scene scene) => _audioSources.Clear();

        protected override void OnAwake()
        {
            LoadVolumeSettings();
        }

        /// <summary>
        /// Registers an audio source with the given AudioData
        /// </summary>
        /// <param name="source">The audio source to register.</param>
        /// <param name="data">The AudioData to configure with.</param>
        public void RegisterAudioSource(AudioSource source, AudioData data)
        {
            source.Configure(data);
            if (!_audioSources.Exists(info => info.Source == source))
            {
                _audioSources.Add(new AudioSourceInfo(source, source.transform));
                source.ConfigureAndPlay(data);
            }
        }
        
        /// <summary>
        /// Registers an audio source with the given AudioDataEnumSoundFx
        /// </summary>
        /// <param name="source">The audio source to register.</param>
        /// <param name="data">The AudioData to configure with.</param>
        public void RegisterAudioSource(AudioSource source, AudioDataEnumSoundFx data) 
            => RegisterAudioSource(source, data.GetAudioData());
        
        /// <summary>
        /// Unregisters an audio source.
        /// </summary>
        /// <param name="source"></param>
        public void UnregisterAudioSource(AudioSource source)
        {
            if (_audioSources.Exists(info => info.Source == source))
            {
                _audioSources.RemoveAll(info => info.Source == source);
                if (source.isPlaying) source.Stop();
            }
        }
        
        /// <summary>
        /// Updates the volume of all registered audio sources based on proximity to the player.
        /// </summary>
        /// <param name="playerPosition">The position of the player.</param>
        /// <param name="proximityDistance">The distance to play the audio at.</param>
        public void UpdateAudioSource(Vector3 playerPosition, float proximityDistance = 10.0f)
        {
            for (var i = 0; i < _audioSources.Count; i++)
            {
                var source = _audioSources[i];
                if (source.ObjTransform == null)
                {
                    _audioSources.RemoveAt(i);
                    continue;
                }
                
                var distance = Vector3.Distance(source.ObjTransform.position, playerPosition);
                var volume = Mathf.Clamp01(1 - distance / proximityDistance);
                source.Source.volume = volume;
            }
        }
        
        /// <summary>
        /// Plays a one-shot audio clip with the given AudioData.
        /// </summary>
        /// <param name="data">The AudioData to play.</param>
        /// <param name="position">The position to play the audio at.</param>
        /// <param name="proximity">Play the audio based on proximity to the player.</param>
        /// <param name="proximityDistance">The distance to play the audio at.</param>
        /// <exception cref="Exception">If the AudioData is null or missing a clip.</exception>
        public void PlayOneShotAudio(AudioData data, Vector3 position, bool proximity = true, float proximityDistance = 10.0f)
        {
            if (!data || !data.clip) throw new Exception("AudioData is null or missing a clip!");
            
            // Create a temporary audio source to play the audio.
            var tempObject = new GameObject("TempAudio")
            {
                transform =
                {
                    position = position, 
                    parent = transform
                }
            };

            // Add an audio source to the temporary object.
            var tempAudioSource = tempObject.AddComponent<AudioSource>();
            
            // Configure the audio source with the given AudioData.
            tempAudioSource.Configure(data);
            
            // If the audio should play based on proximity to the player, set the volume based on distance.
            if (proximity)
            {
                var distance = Vector3.Distance(position, GameManager.Instance.localPlayer.transform.position);
                var volume = Mathf.Clamp01(1 - distance / proximityDistance);
                tempAudioSource.volume = volume;
            }
            
            // Play the audio source.
            tempAudioSource.Play();
            
            // Destroy the temporary object after the audio clip has finished playing.
            Destroy(tempObject, data.clip.length);
        }
        
        /// <summary>
        /// Reverses the audio of the given audio source.
        /// </summary>
        /// <param name="source">The audio source to reverse.</param>
        public static void ReverseAudio(AudioSource source)
        {
            var clip = source.clip;
            
            // Get the samples from the clip.
            var samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);
            
            // Reverse the samples.
            Array.Reverse(samples);
            
            // Create a new AudioClip with the reversed samples.
            var reversedClip = AudioClip.Create($"{clip.name}_Reversed", clip.samples, clip.channels, clip.frequency, false);
            reversedClip.SetData(samples, 0);
            
            // Set the reversed clip to the audio source.
            source.clip = reversedClip;
            
            // Play the reversed clip.
            source.Play();
        }
        
        /// <summary>
        /// Fades the audio source in or out over the given duration.
        /// </summary>
        /// <param name="source">The audio source to fade.</param>
        /// <param name="mode">The fade mode.</param>
        /// <param name="duration">The duration of the fade.</param>
        public void FadeAudio(AudioSource source, FadeMode mode, float duration)
        {
            switch (mode)
            {
                case FadeMode.In:
                    StartCoroutine(Fade(source, duration, 1.0f));
                    break;
                case FadeMode.Out:
                    StartCoroutine(Fade(source, duration, 0.0f));
                    break;
            }
        }
        
        /// <summary>
        /// Fades the audio source to the target volume over the given duration.
        /// </summary>
        /// <param name="source">The audio source to fade.</param>
        /// <param name="duration">The duration of the fade.</param>
        /// <param name="targetVolume">The target volume to fade to.</param>
        private IEnumerator Fade(AudioSource source, float duration, float targetVolume)
        {
            var startVolume = source.volume;
            var currentTime = 0.0f;
            
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
                yield return null;
            }
            
            source.volume = targetVolume;
            if (targetVolume == 0) source.Stop();
        }

        private void LoadVolumeSettings()
        {
            SetMasterVolume(PlayerPrefsUtil.MasterVolume);
            SetMusicVolume(PlayerPrefsUtil.MusicVolume);
            SetSoundFxVolume(PlayerPrefsUtil.SoundFxVolume);
            SetVoiceOverVolume(PlayerPrefsUtil.VoiceOverVolume);
        }
        
        public void SetMasterVolume(float value) => 
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        
        public void SetMusicVolume(float value) => 
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        
        public void SetSoundFxVolume(float value) => 
            audioMixer.SetFloat("SoundFxVolume", Mathf.Log10(value) * 20);
        
        public void SetVoiceOverVolume(float value) => 
            audioMixer.SetFloat("VoiceOverVolume", Mathf.Log10(value) * 20);
    }

    public enum FadeMode
    {
        In,
        Out
    }
}