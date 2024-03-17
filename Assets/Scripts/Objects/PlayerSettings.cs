using UnityEngine;

namespace Objects
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Presets/Player")]
    public class PlayerSettings : ScriptableObject
    {
        [TitleHeader("Movement")]
        public float mouseSensitivity = 20.0f;
        public float movementSpeed = 5.0f;
        public float vaultDistance = 1.0f;
        public float crouchSpeedMultiplier  = 0.5f;
        public float sneakSpeedMultiplier = 0.5f;
        [TitleHeader("Sanity")]
        public float maxSanity = 100.0f;
        public float panicThreshold = 30.0f;
        public float sanityDrainRate = 10.0f;
        [TitleHeader("Stamina")]
        public float maxStamina = 100.0f;
        public float staminaThreshold = 30.0f;
        public float staminaRegenRate = 5.0f;
        public float staminaDrainRate = 10.0f;
        public float crouchStaminaRegenRate = 10.0f;
        public float vaultStaminaCost = 5.0f;
        [TitleHeader("Interaction")]
        public float interactDistance = 2.0f;
        public float inspectDistance = 5.0f;
        public float inspectRotationSpeed = 0.5f;
        [TitleHeader("Enemy Detection")]
        public float detectionRadius = 10.0f;
        [TitleHeader("Layers")]
        public LayerMask ground;
        public LayerMask interactable;
        public LayerMask obstacle;
        public LayerMask enemy;
    }
}