using UnityEngine;
using UnityEngine.Events;

namespace DuolingoMusic.Core
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("Score Settings")]
        [SerializeField] private int currentScore = 0;
        [SerializeField] private int highScore = 0;
        
        public static UnityEvent<int> OnScoreChanged = new UnityEvent<int>();
        public static UnityEvent<int> OnHighScoreChanged = new UnityEvent<int>();
        
        public int CurrentScore => currentScore;
        public int HighScore => highScore;
        
        public void Initialize()
        {
            LoadHighScore();
        }
        
        public void AddScore(int points)
        {
            currentScore += points;
            OnScoreChanged.Invoke(currentScore);
            
            if (currentScore > highScore)
            {
                highScore = currentScore;
                OnHighScoreChanged.Invoke(highScore);
                SaveHighScore();
            }
        }
        
        public void ResetScore()
        {
            currentScore = 0;
            OnScoreChanged.Invoke(currentScore);
        }
        
        private void LoadHighScore()
        {
            highScore = PlayerPrefs.GetInt("HighScore", 0);
        }
        
        private void SaveHighScore()
        {
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }
}