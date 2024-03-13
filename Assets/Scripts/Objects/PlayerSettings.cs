using UnityEngine;

namespace Objects
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Objects/Player")]
    public class PlayerSettings : ScriptableObject
    {
        public float sanity = 100f;

        public float mouseSensitivity = 20f;
        public float movementSpeed = 5f;
        public float jumpHeight = 6f;
        public float gravity = -9.81f;
        
        public float sprintMultiplier = 1.15f;

        public LayerMask ground;
    }
}