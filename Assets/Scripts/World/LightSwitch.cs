using Entities;
using Interfaces;
using UnityEngine;

namespace World
{
    public class LightSwitch : MonoBehaviour, IInteractable
    {
        [SerializeField] private Light light;
        
        public void Interact(BaseEntity entity)
        {
            light.enabled = !light.enabled;
        }
    }
}