using System;
using UnityEngine;

namespace Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
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
    }
}