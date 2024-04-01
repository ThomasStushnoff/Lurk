using Managers;
using UnityEngine;

// WIP
// Camera: Right click to Equip, has an energy meter, scroll to switch between options, when scrolling update UI element.
//
// When Energy Meter, reached 20% (expose this value), the battery progress bar components flickers.
// F to switch between normal and nightvision, ui bar updates, opacity change for selected option, other one falls to 50%
//
// When in nightvision mode, player has a spot light coming him, like a flashlight and it also needs to flicker randomly
//
// Nightvision Profile Created. Resource>Prefabs>Profiles> Nightvision
// When turned on, lerp the Profile> Color Adjustment> Post Exposure Volume from 3 to 0 over 3 seconds of turning on the nightvision mode.
namespace Controllers
{
    public class NightVisionCameraController : MonoBehaviour
    {
        [SerializeField] private Transform nightVisionCameraTransform;
        [SerializeField] private Light spotlight;
        [SerializeField] private float energyMeter = 100.0f;
        [SerializeField] private float energyDrainRate = 5.0f;
        [SerializeField] private float lowEnergyThreshold = 20.0f;
        [ReadOnly] public bool isNightVisionActive;
        
        private VolumeProfileController _volumeProfileController;
        
        private void Awake()
        {
            _volumeProfileController = GetComponent<VolumeProfileController>();
        }

        private void Start()
        { 
            nightVisionCameraTransform.gameObject.SetActive(false);
            spotlight.gameObject.SetActive(false);
        }
        
        private void Update()
        {
            if (GameStateManager.Instance.IsGamePaused) return;
            
            // Equip the night vision camera when right mouse button is pressed.
            if (InputManager.Camera.WasPressedThisFrame())
                ToggleNightVision();

            if (isNightVisionActive)
                UpdateEnergyMeter();
        }
        
        private void ToggleNightVision()
        {
            isNightVisionActive = !isNightVisionActive;
            nightVisionCameraTransform.gameObject.SetActive(isNightVisionActive);
            spotlight.gameObject.SetActive(isNightVisionActive);
            _volumeProfileController.isNightVisionActive = isNightVisionActive;
        }
        
        private void UpdateEnergyMeter()
        {
            energyMeter -= energyDrainRate * Time.deltaTime;
            energyMeter = Mathf.Clamp(energyMeter, 0.0f, 100.0f);
            
            // Turn off night vision when energy meter is empty.
            if (energyMeter <= 0.0f)
                ToggleNightVision();
            
            UpdateEnergyMeterUI(energyMeter);
            
            if (energyMeter <= lowEnergyThreshold)
                FlickerUIElement();
        }
        
        private void UpdateEnergyMeterUI(float value)
        {
        }
        
        private void FlickerUIElement()
        {
        }
    }
}