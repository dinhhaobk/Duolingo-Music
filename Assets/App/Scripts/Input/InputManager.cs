using UnityEngine;
using DuolingoMusic.GameModes;
using DuolingoMusic.Core;

namespace DuolingoMusic.Input
{
    public class InputManager : MonoBehaviour
    {
        private IGameMode currentGameMode;
        
        public void Initialize()
        {
            GameManager.OnGameStateChanged.AddListener(OnGameStateChanged);
        }
        
        void Update()
        {
            if (GameManager.Instance.currentState == GameState.Playing)
            {
                HandleInput();
            }
        }
        
        private void HandleInput()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space) || UnityEngine.Input.GetMouseButtonDown(0))
            {
                currentGameMode?.OnInput(InputType.Tap);
            }
        }
        
        private void OnGameStateChanged(GameState newState)
        {
            // Update current game mode reference when state changes
        }
        
        void OnDestroy()
        {
            GameManager.OnGameStateChanged.RemoveListener(OnGameStateChanged);
        }
    }
}