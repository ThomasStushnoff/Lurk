using Entities.Player;
using Managers;
using UnityEngine;

namespace UI
{
    public class PageUIController : MonoBehaviour
    {
        public void OnEndInteract()
        {
            PlayerController.DisableCursor();
            Destroy(gameObject);
            InputManager.EnableMovementInput();
        }
    }
}