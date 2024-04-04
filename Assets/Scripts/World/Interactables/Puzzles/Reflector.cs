using Controllers;
using Entities;
using Managers;
using UnityEngine;

namespace World.Interactables.Puzzles
{
    // TODO:
    // Puzzle completion logic.
    public class Reflector : BasePuzzle
    {
        [Foldout("Puzzle Settings")]
        [SerializeField] private PuzzleType type;
        [SerializeField] private float rotationSpeed = 50.0f;
        [SerializeField] private float panSpeed = 2.0f;
        
        private Transform _playerCamera;
        private PlayerController _player;

        private void Start()
        {
            Type = type;
            _playerCamera = Camera.main!.transform;
            State = PuzzleState.Incomplete;
        }
        
        private void Update()
        {
            if (LockState is PuzzleLockState.Locked) return;
            
            Debug.Log("Interacting with object");
            if (InputManager.PuzzleCancel.WasPressedThisFrame())
            {
                EndInteract();
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
            var rotate = InputManager.PuzzleRotate.ReadValue<Vector2>();
            transform.Rotate(rotate, rotationSpeed * Time.deltaTime);
        }
        
        /// <summary>
        /// Pans the object based on the player's input.
        /// </summary>
        private void PanObject()
        {
            var panInput = InputManager.PuzzlePan.ReadValue<Vector2>();
            var panDirection = _playerCamera.TransformDirection(new Vector3(panInput.x, 0, panInput.y));
            transform.position += new Vector3(panDirection.x, 0, panDirection.z) * (panSpeed * Time.deltaTime);
        }
        
        public override void BeginInteract(BaseEntity entity)
        {
            if (entity is not PlayerController player) return;
            
            LockState = PuzzleLockState.Unlocked;
            _player = player;
            InputManager.DisableMovementInput();
            InputManager.DisableInteractInput();
            InputManager.EnablePuzzleInput();
            _player.FocusOnPuzzle(transform);
        }
        
        public override void EndInteract()
        {
            _player.StopFocusingOnPuzzle();
            InputManager.EnableMovementInput();
            InputManager.EnableInteractInput();
            InputManager.DisablePuzzleInput();
            _player = null;
            LockState = PuzzleLockState.Locked;
        }
    }
}