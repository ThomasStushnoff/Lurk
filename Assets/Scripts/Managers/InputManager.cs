using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class InputManager : Singleton<InputManager>
    {
        private Actions _actions;
        
        // Player actions.
        public static InputAction Move => Instance._actions.Player.Move;
        public static InputAction Vault => Instance._actions.Player.Vault;
        public static InputAction Crouch => Instance._actions.Player.Crouch;
        public static InputAction Sneak => Instance._actions.Player.Sneak;
        public static InputAction Interact => Instance._actions.Player.Interact;
        public static InputAction InteractOther => Instance._actions.Player.InteractOther; // TODO: Figure out if we need this.
        public static InputAction Inspect => Instance._actions.Player.Inspect;
        public static InputAction RotateLeft => Instance._actions.Player.RotateLeft;
        public static InputAction RotateRight => Instance._actions.Player.RotateRight;
        public static InputAction Look => Instance._actions.Player.Look;
        public static InputAction FreeCursor => Instance._actions.Player.FreeCursor;
        public static InputAction Scroll => Instance._actions.Player.Scroll;
        public static InputAction Camera => Instance._actions.Player.Camera;
        public static InputAction NightVision => Instance._actions.Player.NightVision;
        public static InputAction Menu => Instance._actions.Player.Menu;
        
        // Puzzle actions.
        public static InputAction PuzzlePan => Instance._actions.Puzzle.Pan;
        public static InputAction PuzzleRotate => Instance._actions.Puzzle.Rotate;
        public static InputAction PuzzleCancel => Instance._actions.Puzzle.Cancel;
        
        /// <summary>
        /// Special singleton initializer method.
        /// </summary>
        public new static void Initialize()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Managers/InputManager");
            if (prefab == null) throw new Exception("Missing InputManager prefab!");
            
            var instance = Instantiate(prefab);
            if (instance == null) throw new Exception("Failed to instantiate InputManager prefab!");
            
            instance.name = "Managers.InputManager (Singleton)";
        }
        
        protected override void OnAwake()
        {
            _actions = new Actions();

            GameStateManager.OnCursorStateChange += UpdateCursorState;
            
            FreeCursor.performed += _ => EnableCursor();
            FreeCursor.canceled += _ => DisableCursor();
        }
        
        /// <summary>
        /// Enables all input actions when the scene is loaded.
        /// </summary>
        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode) => _actions?.Enable();

        /// <summary>
        /// Disables all input actions when the scene is unloaded 
        /// </summary>
        protected override void OnSceneUnloaded(Scene scene) => _actions?.Disable();

        /// <summary>
        /// Enables all player movement input.
        /// </summary>
        public static void EnableMovementInput()
        {
            Move.Enable();
            Vault.Enable();
            Crouch.Enable();
            Sneak.Enable();
        }
        
        /// <summary>
        /// Disables all player movement input.
        /// </summary>
        public static void DisableMovementInput()
        {
            Move.Disable();
            Vault.Disable();
            Crouch.Disable();
            Sneak.Disable();
        }

        /// <summary>
        /// Disables all interact input.
        /// </summary>
        public static void DisableInteractInput()
        {
            Interact.Disable();
            Inspect.Disable();
            Scroll.Disable();
            Camera.Disable();
            NightVision.Disable();
        }
        
        /// <summary>
        /// Enables all interact input.
        /// </summary>
        public static void EnableInteractInput()
        {
            Interact.Enable();
            Inspect.Enable();
            Scroll.Enable();
            Camera.Enable();
            NightVision.Enable();
        }
        
        /// <summary>
        /// Disables all puzzle input.
        /// </summary>
        public static void DisablePuzzleInput()
        {
            PuzzlePan.Disable();
            PuzzleRotate.Disable();
            PuzzleCancel.Disable();
        }
        
        /// <summary>
        /// Enables all puzzle input.
        /// </summary>
        public static void EnablePuzzleInput()
        {
            PuzzlePan.Enable();
            PuzzleRotate.Enable();
            PuzzleCancel.Enable();
        }
        
        /// <summary>
        /// Checks if all movement input is enabled.
        /// </summary>
        public static bool IsMovementEnabled => Move.enabled && Vault.enabled && Crouch.enabled && Sneak.enabled;

        private void UpdateCursorState(bool enable)
        {
            if (enable) EnableCursor();
            else DisableCursor();
        }
        
        /// <summary>
        /// Enables the cursor. 
        /// </summary>
        public static void EnableCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        /// <summary>
        /// Disables the cursor.
        /// </summary>
        public static void DisableCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        /// <summary>
        /// Checks if the cursor is enabled.
        /// </summary>
        public static bool IsCursorEnabled() => Cursor.lockState == CursorLockMode.None && Cursor.visible;
    }
}