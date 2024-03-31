using UnityEngine;

namespace World.Interactables
{
    public class PressurePlateReceiver : MonoBehaviour
    {
        public void Activate()
        {
            Debug.Log("Pressure plate activated!");
        }
        
        public void Deactivate()
        {
            Debug.Log("Pressure plate deactivated!");
        }
    }
}