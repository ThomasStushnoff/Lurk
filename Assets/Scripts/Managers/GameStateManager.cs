using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameStateManager : Singleton<GameStateManager>
    {
        public static event Action<GameState> OnGameStateChange;
        public static event Action<bool> OnGamePaused;
        public static event Action<bool> OnGameLoading;
        public static event Action<bool> OnCursorStateChange;

        private bool _isMenuOpen;
        private bool _isPuzzleActive;
        
        public GameState CurrentGameState { get; private set; }
        public bool IsGamePaused => CurrentGameState == GameState.Paused;
        public bool IsGameLoading => CurrentGameState == GameState.Loading;
        public bool IsMenuOpen
        {
            get => _isMenuOpen;
            set
            {
                _isMenuOpen = value;
                EvaluateCursorState();

                SetGameState(value ? GameState.Paused : GameState.Playing);
            }
        }
        public bool IsPuzzleActive
        {
            get => _isPuzzleActive;
            set
            {
                _isPuzzleActive = value;
                EvaluateCursorState();
            }
        }
        
        /// <summary>
        /// Special singleton initializer method.
        /// </summary>
        public new static void Initialize()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Managers/GameStateManager");
            if (prefab == null) throw new Exception("Missing GameStateManager prefab!");

            var instance = Instantiate(prefab);
            if (instance == null) throw new Exception("Failed to instantiate GameStateManager prefab!");

            instance.name = "Managers.GameStateManager (Singleton)";
        }
        
        protected override void OnAwake()
        {
            // TODO: Modify later
            // var activeScene = SceneManager.GetActiveScene().name;
            CurrentGameState = GameState.Playing;
        }
        
        public void SetGameState(GameState state)
        {
            if (CurrentGameState == state) return;
            
            CurrentGameState = state;
            // Evaluate the cursor state if the game state has changed.
            EvaluateCursorState();
            // Trigger general game state change event.
            OnGameStateChange?.Invoke(state);

            switch (state)
            {
                case GameState.MainMenu:
                    break;
                case GameState.Paused:
                    Time.timeScale = 0.0f;
                    break;
                case GameState.Playing:
                    Time.timeScale = 1.0f;
                    break;
                case GameState.Loading:
                    OnGameLoading?.Invoke(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            
            if (state is not GameState.Loading)
                OnGameLoading?.Invoke(false);
        }
        
        private void EvaluateCursorState()
        {
            var shouldEnableCursor = IsGamePaused || _isMenuOpen || _isPuzzleActive || IsGameLoading;
            Cursor.lockState = shouldEnableCursor ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = shouldEnableCursor;
            OnCursorStateChange?.Invoke(shouldEnableCursor);
        }
    }
    
    public enum GameState
    {
        MainMenu,
        Paused,
        Playing,
        Loading,
    }
}