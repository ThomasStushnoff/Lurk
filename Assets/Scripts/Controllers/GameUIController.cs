using JetBrains.Annotations;
using Managers;
using UI.Menus;
using UnityEngine;

namespace Controllers
{
    public class GameUIController : MonoBehaviour
    {
        [SerializeField, NotNull] PauseMenu pauseMenu;
        
        private void Update()
        {
            if (InputManager.Menu.WasReleasedThisFrame() && !pauseMenu.IsOpen)
                pauseMenu.OpenMenu();
        }
    }
}