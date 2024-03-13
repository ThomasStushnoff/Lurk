using Interfaces;
using UnityEngine;

namespace World
{
    public class LightSwitch : MonoBehaviour, IInteractable
    {
        [SerializeField] private Light light;
        
        public void Interact()
        {
            light.enabled = !light.enabled;
        }
    }
}