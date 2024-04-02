using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Menus
{
    public class MainMenu : MonoBehaviour
    {
        public OptionsMenu optionsMenu;
        public CreditsMenu creditsMenu;

        private void Awake()
        {
            GameStateManager.Instance.SetGameState(GameState.MainMenu);
        }

        public void OnPlayButtonClicked()
        {
            // TODO:
            // Loading Screen transition.
            // Call LoadScene.
            
            // SceneManager.LoadScene("");
        }

        public void OnOptionsButtonClicked() => optionsMenu.OpenMenu();
        
        public void OnCreditsButtonClicked() => creditsMenu.OpenMenu();
        
        public void OnQuitButtonClicked()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_STANDALONE
            Application.Quit();
            #endif
        }
    }
}