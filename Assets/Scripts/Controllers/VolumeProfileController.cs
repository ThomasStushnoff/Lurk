using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

// WIP
namespace Controllers
{
    public class VolumeProfileController : MonoBehaviour
    {
        [SerializeField, NotNull] private Volume globalVolume;
        [SerializeField] private float transitionTime = 2.0f;
        [ReadOnly] public bool isNightVisionActive;
        
        private PlayerController _player;
        private VolumeProfile _tempProfile;
        private VolumeProfile _nightVisionProfile;
        private VolumeProfile _targetProfile;
        private bool _isTransitioning;
        private float _timeElapsed;
        private bool _didSwitchProfile; // It's 5am I can't think of a better way lol.
        
        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            
            if (globalVolume == null)
                Debug.LogError("Global Volume is not assigned!");
        }

        private void Start()
        {
            // Create a temporary profile to store the interpolated values.
            _tempProfile = ScriptableObject.CreateInstance<VolumeProfile>();
            _tempProfile.name = "Temp Profile";
            CopyProfile(_player.settings.sanityProfile100, _tempProfile);
            _targetProfile = _tempProfile;
            globalVolume.profile = _tempProfile;
            
            // Create a night vision profile. This is a separate profile that will be used when the player equips the night vision.
            _nightVisionProfile = ScriptableObject.CreateInstance<VolumeProfile>();
            _nightVisionProfile.name = "Night Vision Profile";
            CopyProfile(_player.settings.nightVisionProfile, _nightVisionProfile);
        }

        private void Update()
        {
            if (isNightVisionActive)
            {
                globalVolume.profile = _nightVisionProfile;
                _didSwitchProfile = true;
            }
            else
            {
                if (_didSwitchProfile)
                {
                    // EZ.
                    globalVolume.profile = _tempProfile;
                    _didSwitchProfile = false;
                }
                
                if (_isTransitioning)
                {
                    _timeElapsed += Time.deltaTime;
                    var t = Mathf.Clamp01(_timeElapsed / transitionTime);
                    InterpolateProfile(_tempProfile, _targetProfile, t);
                    
                    if (t >= 1f)
                        _isTransitioning = false;
                }
                else
                {
                    HandleVolumeEffects();
                }
            }
        }

        private void OnDestroy()
        {
            CleanUpProfile(_tempProfile);
            CleanUpProfile(_nightVisionProfile);
        }

        private void HandleVolumeEffects()
        {
            var sanityPercentage = _player.CurrentSanity / _player.settings.maxSanity * 100.0f;
            var newTargetProfile = sanityPercentage switch
            {
                <= 100 and > 75 => _player.settings.sanityProfile100,
                <= 75 and > 50 => _player.settings.sanityProfile75,
                <= 50 and > 25 => _player.settings.sanityProfile50,
                <= 25 and > 10 => _player.settings.sanityProfile25,
                <= 10 and > 0 => _player.settings.sanityProfile10,
                _ => null
            };
            
            if (newTargetProfile != _targetProfile)
            {
                _targetProfile = newTargetProfile;
                _isTransitioning = true;
                _timeElapsed = 0;
            }
        }

        /// <summary>
        /// Interpolates the values of the given profiles.
        /// </summary>
        /// <param name="from">The profile to interpolate from.</param>
        /// <param name="to">The profile to interpolate to.</param>
        /// <param name="t">The interpolation value.</param>
        private static void InterpolateProfile(VolumeProfile from, VolumeProfile to, float t)
        {
            if (from.TryGet<Vignette>(out var fromVignette) && to.TryGet<Vignette>(out var toVignette))
            {
                fromVignette.mode.value = fromVignette.mode.value;
                fromVignette.color.Interp(fromVignette.color.value, toVignette.color.value, t);
                fromVignette.center.Interp(fromVignette.center.value, toVignette.center.value, t);
                fromVignette.intensity.Interp(fromVignette.intensity.value, toVignette.intensity.value, t);
                fromVignette.smoothness.Interp(fromVignette.smoothness.value, toVignette.smoothness.value, t);
                fromVignette.roundness.Interp(fromVignette.roundness.value, toVignette.roundness.value, t);
                fromVignette.rounded.value = fromVignette.rounded.value;
            }
            
            if (from.TryGet<FilmGrain>(out var fromFilmGrain) && to.TryGet<FilmGrain>(out var toFilmGrain))
            {
                fromFilmGrain.type.value = fromFilmGrain.type.value;
                fromFilmGrain.intensity.Interp(fromFilmGrain.intensity.value, toFilmGrain.intensity.value, t);
                fromFilmGrain.response.Interp(fromFilmGrain.response.value, toFilmGrain.response.value, t);
            }
            
            if (from.TryGet<ColorAdjustments>(out var fromColor) && to.TryGet<ColorAdjustments>(out var toColor))
            {
                fromColor.postExposure.Interp(fromColor.postExposure.value, toColor.postExposure.value, t);
                fromColor.contrast.Interp(fromColor.contrast.value, toColor.contrast.value, t);
                fromColor.colorFilter.Interp(fromColor.colorFilter.value, toColor.colorFilter.value, t);
                fromColor.hueShift.Interp(fromColor.hueShift.value, toColor.hueShift.value, t);
                fromColor.saturation.Interp(fromColor.saturation.value, toColor.saturation.value, t);
            }
        }
        
        /// <summary>
        /// Copies the values of the given profile to another profile.
        /// </summary>
        /// <param name="from">The profile to copy from.</param>
        /// <param name="to">The profile to copy to.</param>
        private static void CopyProfile(VolumeProfile from, VolumeProfile to)
        {
            if (from.TryGet<Vignette>(out var fromVignette))
            {
                var toVignette = to.Add<Vignette>();
                toVignette.SetAllOverridesTo(fromVignette.active);
                toVignette.mode.value = fromVignette.mode.value;
                toVignette.color.value = fromVignette.color.value;
                toVignette.center.value = fromVignette.center.value;
                toVignette.intensity.value = fromVignette.intensity.value;
                toVignette.smoothness.value = fromVignette.smoothness.value;
                toVignette.roundness.value = fromVignette.roundness.value;
                toVignette.rounded.value = fromVignette.rounded.value;
            }
            
            if (from.TryGet<FilmGrain>(out var fromFilmGrain))
            {
                var toFilmGrain = to.Add<FilmGrain>();
                toFilmGrain.SetAllOverridesTo(fromFilmGrain.active);
                toFilmGrain.type.value = fromFilmGrain.type.value;
                toFilmGrain.intensity.value = fromFilmGrain.intensity.value;
                toFilmGrain.response.value = fromFilmGrain.response.value;
            }
            
            if (from.TryGet<ColorAdjustments>(out var fromColor))
            {
                var toColor = to.Add<ColorAdjustments>();
                toColor.SetAllOverridesTo(fromColor.active);
                toColor.postExposure.value = fromColor.postExposure.value;
                toColor.contrast.value = fromColor.contrast.value;
                toColor.colorFilter.value = fromColor.colorFilter.value;
                toColor.hueShift.value = fromColor.hueShift.value;
                toColor.saturation.value = fromColor.saturation.value;
            }
        }
        
        /// <summary>
        /// Cleans up the given profile.
        /// </summary>
        /// <param name="profile">The profile to clean up.</param>
        private static void CleanUpProfile(VolumeProfile profile)
        {
            if (profile != null)
            {
                Destroy(profile);
                profile = null;
            }
        }
    }
}