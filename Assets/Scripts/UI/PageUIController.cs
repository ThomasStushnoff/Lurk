using Entities.Player;
using JetBrains.Annotations;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using World.Environmental;

namespace UI
{
    public class PageUIController : MonoBehaviour
    {
        [CanBeNull] public LightBulb lightBulb;
        
        private void Start()
        {
            // Might change later.
            InputManager.InteractOther.started += OnClick;
        }

        private void OnDestroy()
        {
            // Might change later.
            InputManager.InteractOther.started -= OnClick;
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            if (!PlayerController.IsCursorEnabled()) return;
            
            PlayerController.DisableCursor();
            Destroy(gameObject);
            InputManager.EnableMovementInput();
            InputManager.EnableInteractInput();
            
            if (lightBulb != null)
                lightBulb.onRoomChange?.Invoke();
        }
    }
}