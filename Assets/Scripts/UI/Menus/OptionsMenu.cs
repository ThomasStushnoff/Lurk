using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Menus
{
    public class OptionsMenu : Menu
    {
        [TitleHeader("Options Menu Settings")]
        #region Audio Settings
        
        public Slider masterVolumeSlider;
        public Slider musicVolumeSlider;
        public Slider sfxVolumeSlider;
        public Slider voiceVolumeSlider;
        // Output device requires Wwise integration.
        // See: https://forum.unity.com/threads/how-can-i-change-the-sound-output-device.1429936/
        // public TMP_Dropdown outputDeviceDropdown;
        
        #endregion
        
        #region Video Settings
        
        public TMP_Dropdown resolutionDropdown;
        public TMP_Dropdown windowModeDropdown;
        // public TMP_Dropdown qualityDropdown;
        
        #endregion
        
        #region Keybind Settings
        
        public Slider mouseSensitivitySlider;
        public Toggle invertToggle;
        public Button forwardButton;
        public Button backwardButton;
        public Button leftButton;
        public Button rightButton;
        public Button sneakButton;
        public Button crouchButton;
        public Button interactButton;
        public Button bringUpCameraButton;
        public Button nightVisionButton;
        public Button pauseMenu;

        #endregion

        #region Crosshair Control
        
        public Slider widthSlider;
        public Slider outlineWidthSlider;
        public TMP_Dropdown colorDropdown;
        public Toggle centreDotToggle;
        public Slider centreDotRadiusSlider;
        public Slider oppacitySlider;

        #endregion
        
        private float _masterVolume;
        private float _musicVolume;
        private float _sfxVolume;
        private float _voiceVolume;
        private Resolution[] _resolutions;
        private HashSet<ResolutionOptionData> _resolutionOptions = new HashSet<ResolutionOptionData>();
        private int _resolutionIndex;
        private int _windowModeIndex;
        
        private void Awake()
        {
            LoadAudioSettings();
            LoadVideoSettings();
        }
        
        #region Audio Settings Methods
        
        private void LoadAudioSettings()
        {
            // Add listeners to the sliders.
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            voiceVolumeSlider.onValueChanged.AddListener(OnVoiceVolumeChanged);
            
            // Load the stored volume settings.
            _masterVolume = PlayerPrefsUtil.MasterVolume;
            _musicVolume = PlayerPrefsUtil.MusicVolume;
            _sfxVolume = PlayerPrefsUtil.SoundFxVolume;
            _voiceVolume = PlayerPrefsUtil.VoiceOverVolume;
            
            // Change the volume settings based on the stored values.
            OnMasterVolumeChanged(_masterVolume);
            OnMusicVolumeChanged(_musicVolume);
            OnSfxVolumeChanged(_sfxVolume);
            OnVoiceVolumeChanged(_voiceVolume);
            
            // Change the UI elements based on the stored values.
            masterVolumeSlider.SetValueWithoutNotify(_masterVolume);
            musicVolumeSlider.SetValueWithoutNotify(_musicVolume);
            sfxVolumeSlider.SetValueWithoutNotify(_sfxVolume);
            voiceVolumeSlider.SetValueWithoutNotify(_voiceVolume);
        }
        
        public void OnMasterVolumeChanged(float value)
        {
            _masterVolume = value;
            PlayerPrefsUtil.MasterVolume = value;
            AudioManager.Instance.SetMasterVolume(_masterVolume);
        }
        
        public void OnMusicVolumeChanged(float value)
        {
            _musicVolume = value;
            PlayerPrefsUtil.MusicVolume = value;
            AudioManager.Instance.SetMusicVolume(_musicVolume);
        }
        
        public void OnSfxVolumeChanged(float value)
        {
            _sfxVolume = value;
            PlayerPrefsUtil.SoundFxVolume = value;
            AudioManager.Instance.SetSoundFxVolume(_sfxVolume);
        }
        
        public void OnVoiceVolumeChanged(float value)
        {
            _voiceVolume = value;
            PlayerPrefsUtil.VoiceOverVolume = value;
            AudioManager.Instance.SetVoiceOverVolume(_voiceVolume);
        }
        
        #endregion
        
        #region Video Settings Methods
        
        private void LoadVideoSettings()
        {
            // Add listeners to the dropdowns.
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            windowModeDropdown.onValueChanged.AddListener(OnWindowModeChanged);
            
            // Get the available resolutions.
            _resolutions = Screen.resolutions;
            _resolutionOptions.Clear();
            foreach (var resolution in _resolutions)
            {
                // Skip duplicate resolutions.
                var option = $"{resolution.width} x {resolution.height}";
                if (_resolutionOptions.Any(x => x.Option.Contains(option)))
                    continue;
                
                // Add the resolution option.
                var resolutionOption = new ResolutionOptionData(option, resolution.width, resolution.height);
                _resolutionOptions.Add(resolutionOption);
                resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(option));
            }
            
            // Load the stored video settings.
            _resolutionIndex = PlayerPrefsUtil.ResolutionIndex;
            _windowModeIndex = PlayerPrefsUtil.WindowModeIndex;
            
            // Change the UI elements based on the stored values.
            resolutionDropdown.SetValueWithoutNotify(_resolutionIndex);
            windowModeDropdown.SetValueWithoutNotify(_windowModeIndex);
        }
        
        public void OnResolutionChanged(int index)
        {
            var resolution = _resolutionOptions.ElementAt(index);
            Screen.SetResolution(resolution.Width, resolution.Height, GetCurrentFullScreenMode(_windowModeIndex));
            _resolutionIndex = index;
            PlayerPrefsUtil.ResolutionIndex = index;
        }
        
        public void OnWindowModeChanged(int index)
        {
            // https://support.unity.com/hc/en-us/articles/115001276723-Fullscreen-options-Exclusive-Fullscreen-vs-Fullscreen-Window-Borderless
            switch (index)
            {
                case 0:
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
                case 1:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case 2:
                    Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                    break;
                case 3:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
            }
            
            _windowModeIndex = index;
            
            PlayerPrefsUtil.WindowModeIndex = index;
        }
        
        private static FullScreenMode GetCurrentFullScreenMode(int index)
        {
            return index switch
            {
                0 => FullScreenMode.ExclusiveFullScreen,
                1 => FullScreenMode.FullScreenWindow,
                2 => FullScreenMode.MaximizedWindow,
                3 => FullScreenMode.Windowed,
                _ => FullScreenMode.ExclusiveFullScreen
            };
        }
        
        #endregion
        
    }
    
    /// <summary>
    /// Represents a resolution option.
    /// </summary>
    public sealed class ResolutionOptionData
    {
        public string Option;
        public int Width;
        public int Height;
        
        public ResolutionOptionData(string option, int width, int height)
        {
            Option = option;
            Width = width;
            Height = height;
        }
    }
}