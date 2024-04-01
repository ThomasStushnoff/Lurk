using Controllers;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        public PlayerController localPlayer;
        
        protected override void OnAwake()
        {
            GameStateManager.Initialize();
            PrefabManager.Initialize();
            AudioManager.Initialize();
            InputManager.Initialize();
        }
    }
}
