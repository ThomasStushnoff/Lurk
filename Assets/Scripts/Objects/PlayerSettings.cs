using UnityEngine;

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
        [Range(0.0f, 1.0f)]
        public float sneakVolume = 0.5f;
        [Range(0.0f, 1.0f)]
        public float sneakPitch = 0.5f;
        [TitleHeader("Camera")]
        public float mouseSensitivity = 20.0f;
        public float bobFrequency = 5.0f;
        public float bobSpeed = 10.0f;
        public float bobAmountX = 0.005f;
        public float bobAmountY = 0.05f;
        [TitleHeader("Sanity")]
        public float maxSanity = 100.0f;
        public float panicThreshold = 30.0f;
        public float sanityDrainRate = 10.0f;
        [TitleHeader("Stamina")]
        public float maxStamina = 100.0f;
        public float staminaThreshold = 30.0f;
        public float staminaRegenRate = 5.0f;
        public float staminaDrainRate = 10.0f;
        public float sneakStaminaDrainRate = 4.0f;
        public float crouchStaminaRegenRate = 10.0f;
        public float vaultStaminaCost = 5.0f;
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
    }
}