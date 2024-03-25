using Entities.Player;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class PageUIController : MonoBehaviour
    {
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
        }
    }
}