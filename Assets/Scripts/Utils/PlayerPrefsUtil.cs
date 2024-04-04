using UnityEngine;

namespace Utils
{
    public static class PlayerPrefsUtil
    {
        public static float MasterVolume
        {
            get => PlayerPrefs.GetFloat("MasterVolume", 1.0f);
            set
            {
                PlayerPrefs.SetFloat("MasterVolume", value);
                PlayerPrefs.Save();
            }
        }
        
        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat("MusicVolume", 1.0f);
            set
            {
                PlayerPrefs.SetFloat("MusicVolume", value);
                PlayerPrefs.Save();
            }
        }
        
        public static float SoundFxVolume
        {
            get => PlayerPrefs.GetFloat("SoundFxVolume", 1.0f);
            set
            {
                PlayerPrefs.SetFloat("SoundFxVolume", value);
                PlayerPrefs.Save();
            }
        }
        
        public static float VoiceOverVolume
        {
            get => PlayerPrefs.GetFloat("VoiceOverVolume", 1.0f);
            set
            {
                PlayerPrefs.SetFloat("VoiceOverVolume", value);
                PlayerPrefs.Save();
            }
        }
        
        public static int ResolutionIndex
        {
            get => PlayerPrefs.GetInt("ResolutionIndex", 0);
            set
            {
                PlayerPrefs.SetInt("ResolutionIndex", value);
                PlayerPrefs.Save();
            }
        }
        
        public static int WindowModeIndex
        {
            get => PlayerPrefs.GetInt("WindowModeIndex", 0);
            set
            {
                PlayerPrefs.SetInt("WindowModeIndex", value);
                PlayerPrefs.Save();
            }
        }
    }
}