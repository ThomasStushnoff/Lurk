using Interfaces;
using Managers;
using UnityEngine;

namespace World
{
    public class Reflector : MonoBehaviour, IInteractable
    {
        
        [SerializeField] private float rotationSpeed = 50.0f;
        [SerializeField] private float panSpeed = 2.0f;
        
        private Transform _playerCamera;
        private bool _isInteracting;

        private void Start() => _playerCamera = Camera.main!.transform;

        public void Interact() => _isInteracting = !_isInteracting;

        private void Update()
        {
            if (!_isInteracting) return;
            
            InputManager.DisableMovementInput();
            InputManager.EnablePuzzleInput();
                
            Debug.Log("Interacting with object");
            if (InputManager.Interact.WasReleasedThisFrame())
            {
                _isInteracting = false;
                InputManager.EnableMovementInput();
                InputManager.DisablePuzzleInput();
                return;
            }
                
            RotateObject();
            PanObject();
        }
        
        /// <summary>
        /// Rotates the object based on the player's input.
        /// </summary>
        private void RotateObject()
        {
            Debug.Log("Rotating object");
            var rotate = InputManager.PuzzleRotate.ReadValue<Vector2>();
            transform.Rotate(rotate, rotationSpeed * Time.deltaTime);
        }
        
        /// <summary>
        /// Pans the object based on the player's input.
        /// </summary>
        private void PanObject()
        {
            var panDirection = InputManager.PuzzlePan.ReadValue<Vector2>();
            transform.position += _playerCamera.TransformDirection(panDirection) * (panSpeed * Time.deltaTime);
        }
    }
}