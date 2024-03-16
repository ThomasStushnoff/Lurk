using System;
using UnityEngine;

namespace World
{
    /// <summary>
    /// Environmental change when the player looks at the same direction again.
    /// </summary>
    public class EnvironmentTrigger : MonoBehaviour
    {
        private Camera _playerCam;
        private int _lookCount;
        
        private void Start()
        {
            _playerCam = Camera.main;
        }

        private void Update()
        {
            if (!Physics.Raycast(_playerCam.transform.position, _playerCam.transform.forward, out var hit)) return;
            if (hit.collider.gameObject != gameObject) return;
            
            _lookCount++;
            if (_lookCount % 2 == 0)
            {
                Debug.Log("Looked at the object twice.");
                ChangeEnvironment();
            }
        }
        
        private void ChangeEnvironment()
        {
            // Change the environment.
            // TODO: Tech document is vague about this.
        }
    }
}