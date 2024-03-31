using Controllers;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        public PlayerController localPlayer;
        
        protected override void OnAwake()
        {
            PrefabManager.Initialize();
            AudioManager.Initialize();
            InputManager.Initialize();
        }
    }
}
