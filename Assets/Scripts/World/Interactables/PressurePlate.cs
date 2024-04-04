using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace World.Interactables
{
    public class PressurePlate : BaseObject
    {
        [TitleHeader("Pressure Plate Settings")]
        [SerializeField] private float weightThreshold = 1.0f;
        [SerializeField, CanBeNull, Tooltip("The point where the pressure plate detects weight.")] private Transform detectionPoint;
        [SerializeField] private float detectionRadius = 1.0f;
        [SerializeField] private LayerMask detectionLayer;
        public UnityEvent onActivate;
        public UnityEvent onDeactivate;
        
        private float _currentWeight;

        private void Start()
        {
            // TODO: Remove this if the pressure plate mesh doesn't need a detection point to be set.
            if (!detectionPoint)
                detectionPoint = transform;
        }
        
        private void Update()
        {
            var results = new Collider[12];
            var size = Physics.OverlapSphereNonAlloc(detectionPoint!.position, detectionRadius, results, detectionLayer);
            
            var totalWeight = 0.0f;
            for (var i = 0; i < size; i++)
            {
                var resRb = results[i].attachedRigidbody;
                if (!resRb) continue;
                // Add the mass of the rigidbody to the total weight.
                totalWeight += rb.mass;
            }
            
            if (totalWeight >= weightThreshold && _currentWeight < weightThreshold)
                onActivate.Invoke();
            else if (totalWeight < weightThreshold && _currentWeight >= weightThreshold)
                onDeactivate.Invoke();
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(detectionPoint!.position, detectionRadius);
        }
    }
}