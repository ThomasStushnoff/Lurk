using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace World
{
    public class EnvironmentTrigger : MonoBehaviour
    {
        [SerializeField] private List<GameObject> managedObjects;
        [SerializeField] private bool deactivateOnExit;

        private void Start()
        {
            foreach (var obj in managedObjects.Where(obj => obj))
                obj.SetActive(false);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.IsPlayer()) return;
            
            foreach (var obj in managedObjects.Where(obj => obj))
                obj.SetActive(true);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.IsPlayer() || !deactivateOnExit) return;
            
            foreach (var obj in managedObjects.Where(obj => obj))
                obj.SetActive(false);
        }
    }
}