using UnityEngine;

namespace Objects
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Objects/Player")]
    public class PlayerSettings : ScriptableObject
    {
        public float mouseSensitivity = 20f;
        public float movementSpeed = 5f;
        public float slowWalkMultiplier  = 1.15f;
        public float jumpHeight = 6f;
        public float gravity = -9.81f;
        
        public float sanity = 100f;
        public float maxSanity = 100f;
        public float panicThreshold = 30f;
        
        public float stamina = 100f;
        public float maxStamina = 100f;
        public float staminaThreshold = 30f;
        public float staminaRegenRate = 5f;
        public float staminaDrainRate = 10f;
        
        public float interactDistance = 2f;

        public LayerMask ground;
    }
}