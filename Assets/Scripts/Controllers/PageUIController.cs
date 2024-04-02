using JetBrains.Annotations;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using World.Environmental;

namespace Controllers
{
    public class PageUIController : MonoBehaviour
    {
        [CanBeNull] public LightBulb lightBulb;
        
        private void Start()
        {
            InputManager.InteractOther.started += OnClick;
        }
        
        private void OnDestroy()
        {
            InputManager.InteractOther.started -= OnClick;
        }
        
        private void Update()
        {
            if (GameStateManager.Instance.IsGamePaused) return;
            
            // Ensure cursor is enabled when the page is open.
            if (!GameStateManager.Instance.IsPuzzleActive)
                GameStateManager.Instance.IsPuzzleActive = true;
        }
        
        private void OnClick(InputAction.CallbackContext context)
        {
            if (!GameStateManager.Instance.IsPuzzleActive && GameStateManager.Instance.IsGamePaused) return;

            GameStateManager.Instance.IsPuzzleActive = false;
            Destroy(gameObject);
            InputManager.EnableMovementInput();
            InputManager.EnableInteractInput();
            
            if (lightBulb != null)
                lightBulb.onRoomChange?.Invoke();
        }
    }
}