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
        public static InputAction InteractPuzzle => Instance._actions.Player.InteractPuzzle; // TODO: Figure out if we need this.
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
        
        protected override void OnAwake() => _actions = new Actions();
        
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
            Interact.Enable();
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
            Interact.Disable();
        }

        public static void EnablePuzzleInput()
        {
            PuzzlePan.Enable();
            PuzzleRotate.Enable();
            PuzzleCancel.Enable();
        }
        
        public static void DisablePuzzleInput()
        {
            PuzzlePan.Disable();
            PuzzleRotate.Disable();
            PuzzleCancel.Disable();
        }
        
        public static bool IsMovementEnabled => Move.enabled && Vault.enabled && Crouch.enabled && Sneak.enabled;
    }
}