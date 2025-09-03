using DuolingoMusic.Audio;
using DuolingoMusic.Config;
using DuolingoMusic.GameModes;
using DuolingoMusic.Input;
using UnityEngine;
using UnityEngine.Events;

namespace DuolingoMusic.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Configuration")]
        [SerializeField] public GameConfig gameConfig;
        
        [Header("Managers")]
        [SerializeField] public AudioManager audioManager;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] public ScoreManager scoreManager;
        
        private IGameMode currentGameMode;
        public GameState currentState { get; private set; } = GameState.Menu;
        
        // Events
        public static UnityEvent<GameState> OnGameStateChanged = new();
        
        public static GameManager Instance { get; private set; }
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            ChangeGameState(GameState.Menu);
        }
        
        void Update()
        {
            currentGameMode?.Update();
        }
        
        private void InitializeGame()
        {
            // Initialize all managers
            audioManager.Initialize();
            inputManager.Initialize();
            uiManager.Initialize();
            scoreManager.Initialize();
        }
        
        public void StartLesson(LessonType lessonType)
        {
            switch (lessonType)
            {
                case LessonType.SpeedItUp:
                    currentGameMode = new RhythmTapMode(gameConfig.rhythmTapConfig);
                    break;
                case LessonType.PerformTheSong:
                    currentGameMode = new PerformanceMode(gameConfig.performanceModeConfig);
                    break;
            }
            
            currentGameMode?.Initialize();
            ChangeGameState(GameState.Playing);
        }
        
        public void ChangeGameState(GameState newState)
        {
            currentState = newState;
            OnGameStateChanged.Invoke(newState);
        }
        
        public void EndLesson()
        {
            currentGameMode?.Cleanup();
            ChangeGameState(GameState.Results);
        }
    }
    
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        Results
    }
    
    public enum LessonType
    {
        SpeedItUp,
        PerformTheSong
    }
}