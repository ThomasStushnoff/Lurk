using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Controllers
{
    public class VolumeProfileController : MonoBehaviour
    {
        [SerializeField, NotNull] private Volume globalVolume;
        [SerializeField] private float transitionTime = 2.0f;
        
        private PlayerController _player;
        private VolumeProfile _tempProfile;
        private VolumeProfile _targetProfile;
        private bool _isTransitioning;
        private float _timeElapsed;
        
        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            
            if (globalVolume == null)
                Debug.LogError("Global Volume is not assigned!");
        }

        private void Start()
        {
            _tempProfile = ScriptableObject.CreateInstance<VolumeProfile>();
            CopyProfile(_player.settings.sanityProfile100, _tempProfile);
            _targetProfile = _tempProfile;
            globalVolume.profile = _tempProfile;
        }

        private void Update()
        {
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

        private void OnDestroy()
        {
            CleanUpProfile(_tempProfile);
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

        private void InterpolateProfile(VolumeProfile from, VolumeProfile to, float t)
        {
            if (from.TryGet<Vignette>(out var fromVignette) && to.TryGet<Vignette>(out var toVignette))
            {
                _tempProfile.TryGet<Vignette>(out var tempVignette);
                tempVignette.mode.value = fromVignette.mode.value;
                tempVignette.color.Interp(fromVignette.color.value, toVignette.color.value, t);
                tempVignette.center.Interp(fromVignette.center.value, toVignette.center.value, t);
                tempVignette.intensity.Interp(fromVignette.intensity.value, toVignette.intensity.value, t);
                tempVignette.smoothness.Interp(fromVignette.smoothness.value, toVignette.smoothness.value, t);
                tempVignette.roundness.Interp(fromVignette.roundness.value, toVignette.roundness.value, t);
                tempVignette.rounded.value = fromVignette.rounded.value;
            }
            
            if (from.TryGet<FilmGrain>(out var fromFilmGrain) && to.TryGet<FilmGrain>(out var toFilmGrain))
            {
                _tempProfile.TryGet<FilmGrain>(out var tempFilmGrain);
                tempFilmGrain.type.value = fromFilmGrain.type.value;
                tempFilmGrain.intensity.Interp(fromFilmGrain.intensity.value, toFilmGrain.intensity.value, t);
                tempFilmGrain.response.Interp(fromFilmGrain.response.value, toFilmGrain.response.value, t);
            }
            
            if (from.TryGet<ColorAdjustments>(out var fromColor) && to.TryGet<ColorAdjustments>(out var toColor))
            {
                _tempProfile.TryGet<ColorAdjustments>(out var tempColor);
                tempColor.postExposure.Interp(fromColor.postExposure.value, toColor.postExposure.value, t);
                tempColor.contrast.Interp(fromColor.contrast.value, toColor.contrast.value, t);
                tempColor.colorFilter.Interp(fromColor.colorFilter.value, toColor.colorFilter.value, t);
                tempColor.hueShift.Interp(fromColor.hueShift.value, toColor.hueShift.value, t);
                tempColor.saturation.Interp(fromColor.saturation.value, toColor.saturation.value, t);
            }
        }
        
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