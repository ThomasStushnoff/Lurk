using UnityEngine;

namespace World.Environmental
{
    public class DoorSlam : BaseObject
    {
        [Tooltip("Reference to the Door object.")]
        public Door door;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.IsPlayer() || door == null) return;
            door.OnSlam?.Invoke();
            Destroy(gameObject);
        }
    }
}