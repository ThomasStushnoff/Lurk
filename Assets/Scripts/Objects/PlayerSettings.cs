using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Rendering;

namespace Objects
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Presets/Player")]
    public class PlayerSettings : ScriptableObject
    {
        [TitleHeader("Movement")]
        public float movementSpeed = 5.0f;
        public float vaultDistance = 1.0f;
        public float crouchSpeedMultiplier  = 0.5f;
        public float sneakSpeedMultiplier = 0.5f;
        public float standHeight = 2.0f;
        public float crouchHeight = 1.0f;
        
        [TitleHeader("Camera")]
        public float mouseSensitivity = 20.0f;
        public float minXClamp = -90.0f;
        public float maxXClamp = 90.0f;
        public float bobFrequency = 5.0f;
        public float bobSpeed = 10.0f;
        public float bobAmountX = 0.005f;
        public float bobAmountY = 0.05f;
        
        [TitleHeader("Night Vision")]
        public float energyMeter = 100.0f;
        public float energyDrainRate = 1.5f;
        public float lowEnergyThreshold = 20.0f;
        
        [TitleHeader("Sanity")]
        public float maxSanity = 100.0f;
        public float panicThreshold = 30.0f;
        public float sanityRegenRate = 5.0f;
        public float sanityDrainRate = 10.0f;

        [TitleHeader("Volume Profiles")]
        public VolumeProfile sanityProfile100;
        public VolumeProfile sanityProfile75;
        public VolumeProfile sanityProfile50;
        public VolumeProfile sanityProfile25;
        public VolumeProfile sanityProfile10;
        public VolumeProfile nightVisionProfile;
        
        [TitleHeader("Interaction")]
        public float interactDistance = 2.0f;
        public float interactDropDistance = 2.0f;
        public float inspectDistance = 5.0f;
        public float inspectRotationSpeed = 0.5f;
        
        [TitleHeader("Enemy Detection")]
        public float detectionRadius = 10.0f;
        
        [TitleHeader("Layers")]
        public LayerMask ground;
        public LayerMask inspectable;
        public LayerMask interactable;
        public LayerMask puzzle;
        public LayerMask obstacle;
        public LayerMask enemy;
        public LayerMask environment;
        public LayerMask bloodyFloor;
        
        [TitleHeader("Audio")]
        public List<AudioDataEnumSoundFx> footstepSounds;
        public AudioDataEnumSoundFx bloodyFloorSound;
        [Range(0.0f, 1.0f)]
        public float sneakVolume = 0.5f;
        [Range(0.0f, 1.0f)]
        public float sneakPitch = 0.5f;
    }
}