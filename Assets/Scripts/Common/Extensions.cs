using System.Collections.Generic;
using Audio;
using Managers;
using Objects;
using UnityEngine;

public static class Extensions
{
    /// <summary>
    /// Resets the transform's position and rotation.
    /// </summary>
    /// <param name="transform">The transform to reset.</param>
    /// <param name="resetPosition">Whether to reset the position.</param>
    /// <param name="resetRotation">Whether to reset the rotation.</param>
    public static void Reset(this Transform transform,
        bool resetPosition = true,
        bool resetRotation = false)
    {
        if (resetPosition) transform.position = Vector3.zero;

        if (resetRotation) transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// Returns true if the collider is a player.
    /// </summary>
    /// <param name="collider">The collider to check.</param>
    public static bool IsPlayer(this Collider collider)
        => collider.CompareTag("Player") || collider.gameObject.CompareLayer("Player");

    /// <summary>
    /// Returns true if the collision is a player.
    /// </summary>
    /// <param name="collision">The collision to check.</param>
    public static bool IsPlayer(this Collision collision)
        => collision.gameObject.CompareTag("Player") || collision.gameObject.CompareLayer("Player");

    /// <summary>
    /// Returns true if the game object's layer matches the layer name.
    /// </summary>
    /// <param name="gameObject">The game object to check.</param>
    /// <param name="layerName">The layer name to compare.</param>
    /// <returns>true or false.</returns>
    public static bool CompareLayer(this GameObject gameObject, string layerName)
        => gameObject.layer == LayerMask.NameToLayer(layerName);
    
    /// <summary>
    /// Configures the audio source with the audio data.
    /// </summary>
    /// <param name="audioSource">The audio source to configure.</param>
    /// <param name="audioData">The audio data to use.</param>
    public static void Configure(this AudioSource audioSource, AudioData audioData)
    {
        if (!audioData) return;
        
        audioSource.clip = audioData.clip;
        audioSource.outputAudioMixerGroup = audioData.mixerGroup;
        audioSource.playOnAwake = audioData.playOnAwake;
        audioSource.loop = audioData.loop;
        audioSource.volume = audioData.volume;
        audioSource.spatialize = audioData.spatialize;
        audioSource.pitch = audioData.pitch;
    }
    
    /// <summary>
    /// Configures the audio source with the audio data and plays it.
    /// </summary>
    /// <param name="audioSource">The audio source to configure.</param>
    /// <param name="audioData">The audio data to use.</param>
    public static void ConfigureAndPlay(this AudioSource audioSource, AudioData audioData)
    {
        audioSource.Configure(audioData);
        audioSource.Play();
    }

    /// <summary>
    /// Overloads the PlayOneShot method to use the audio data.
    /// </summary>
    /// <param name="audioSource">The audio source to configure.</param>
    /// <param name="audioData">The audio data to use.</param>
    /// <param name="proximity">Should the audio be played at the audio source's position?</param>
    public static void PlayOneShot(this AudioSource audioSource, AudioData audioData, bool proximity = false)
    {
        if (!audioData) return;
        
        if (proximity)
            AudioManager.Instance.PlayOneShotAudio(audioData, audioSource.transform.position);
        else
            audioSource.PlayOneShot(audioData.clip);
    }
    
    // /// <summary>
    // /// Plays the audio data as a one shot.
    // /// </summary>
    // /// <param name="audioSource">The audio source to configure.</param>
    // /// <param name="audioEnum">The music audio data enum.</param>
    // /// <param name="proximity">Should the audio be played at the audio source's position?</param>
    // public static void PlayOneShot(this AudioSource audioSource, AudioDataEnumMusic audioEnum, bool proximity = false)
    // {
    //     if (AudioDataMapSoundFx.Map.TryGetValue(audioEnum, out var audioData))
    //         audioSource.PlayOneShot(audioData, proximity);
    // }
    
    /// <summary>
    /// Plays the audio data as a one shot.
    /// </summary>
    /// <param name="audioSource">The audio source to configure.</param>
    /// <param name="audioEnum">The sound fx audio data enum.</param>
    /// <param name="proximity">Should the audio be played at the audio source's position?</param>
    public static void PlayOneShot(this AudioSource audioSource, AudioDataEnumSoundFx audioEnum, bool proximity = false)
    {
        if (AudioDataMapSoundFx.Map.TryGetValue(audioEnum, out var audioData))
            audioSource.PlayOneShot(audioData, proximity);
    }
    
    /// <summary>
    /// Plays the audio data as a one shot.
    /// </summary>
    /// <param name="audioSource">The audio source to configure.</param>
    /// <param name="audioEnum">The voice over audio data enum.</param>
    /// <param name="proximity">Should the audio be played at the audio source's position?</param>
    public static void PlayOneShot(this AudioSource audioSource, AudioDataEnumVoiceOver audioEnum, bool proximity = false)
    {
        if (AudioDataMapVoiceOver.Map.TryGetValue(audioEnum, out var audioData))
            audioSource.PlayOneShot(audioData, proximity);
    }
    
    // /// <summary>
    // /// Gets the audio data from the audio enum.
    // /// </summary>
    // /// <param name="audioEnum">The audio enum to get the audio data from.</param>
    // /// <returns>The audio data.</returns>
    // public static AudioData GetAudioData(this AudioDataEnumMusic audioEnum) 
    //     => AudioDataMapSoundFx.Map.AudioDataEnumMusic(audioEnum);
    
    /// <summary>
    /// Gets the audio data from the audio enum.
    /// </summary>
    /// <param name="audioEnum">The audio enum to get the audio data from.</param>
    /// <returns>The audio data.</returns>
    public static AudioData GetAudioData(this AudioDataEnumSoundFx audioEnum) 
        => AudioDataMapSoundFx.Map.GetValueOrDefault(audioEnum);
    
    /// <summary>
    /// Gets the audio data from the audio enum.
    /// </summary>
    /// <param name="audioEnum">The audio enum to get the audio data from.</param>
    /// <returns>The audio data.</returns>
    public static AudioData GetAudioData(this AudioDataEnumVoiceOver audioEnum)
        => AudioDataMapVoiceOver.Map.GetValueOrDefault(audioEnum);
    
    // /// <summary>
    // /// Plays the music audio data.
    // /// </summary>
    // /// <param name="audioSource">The audio source to play the music.</param>
    // /// <param name="audioEnum">The music audio data enum.</param>
    // public static void PlayMusic(this AudioSource audioSource, AudioDataEnumMusic audioEnum)
    // {
    //     if (audioEnum is AudioDataEnumMusic.None) return;
    //
    //     if (AudioDataMapMusic.Map.TryGetValue(audioEnum, out var audioData))
    //         audioSource.ConfigureAndPlay(audioData);
    // }
    
    /// <summary>
    /// Plays the voice over audio data.
    /// </summary>
    /// <param name="audioSource">The audio source to play the voice over.</param>
    /// <param name="audioEnum">The voice over audio data enum.</param>
    public static void PlayVoiceOver(this AudioSource audioSource, AudioDataEnumVoiceOver audioEnum)
    {
        if (audioEnum is AudioDataEnumVoiceOver.None) return;
        
        if (AudioDataMapVoiceOver.Map.TryGetValue(audioEnum, out var audioData))
            audioSource.ConfigureAndPlay(audioData);
    }
    
    /// <summary>
    /// Plays the sound fx audio data.
    /// </summary>
    /// <param name="audioSource">The audio source to play the sound fx.</param>
    /// <param name="audioEnum">The voice over audio data enum.</param>
    public static void PlaySoundFx(this AudioSource audioSource, AudioDataEnumSoundFx audioEnum)
    {
        if (audioEnum is AudioDataEnumSoundFx.None) return;
        
        if (AudioDataMapSoundFx.Map.TryGetValue(audioEnum, out var audioData))
            audioSource.ConfigureAndPlay(audioData);
    }
}