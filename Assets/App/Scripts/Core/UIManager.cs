using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DuolingoMusic.Config;

namespace DuolingoMusic.Core
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject resultsPanel;
        
        [Header("Menu UI")]
        [SerializeField] private Button speedItUpButton;
        [SerializeField] private Button performSongButton;
        [SerializeField] private TextMeshProUGUI highScoreText;
        
        [Header("Gameplay UI")]
        [SerializeField] private TextMeshProUGUI currentScoreText;
        [SerializeField] private TextMeshProUGUI bpmText;
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TextMeshProUGUI gameModeText;
        
        [Header("Pause UI")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;
        
        [Header("Results UI")]
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI accuracyText;
        [SerializeField] private TextMeshProUGUI newHighScoreText;
        [SerializeField] private Button playAgainButton;
        [SerializeField] private Button backToMenuButton;
        
        [Header("Performance Indicators")]
        [SerializeField] private GameObject hitIndicatorPrefab;
        [SerializeField] private Transform hitIndicatorParent;
        [SerializeField] private float hitIndicatorLifetime = 1f;
        
        [Header("Visual Feedback")]
        [SerializeField] private Image screenFlash;
        [SerializeField] private Color perfectHitColor = Color.green;
        [SerializeField] private Color goodHitColor = Color.yellow;
        [SerializeField] private Color missColor = Color.red;
        
        private GameState currentState;
        private Coroutine progressUpdateCoroutine;
        private int currentCombo = 0;
        
        public void Initialize()
        {
            SetupButtonListeners();
            SubscribeToEvents();
            ShowPanel(GameState.Menu);
            UpdateHighScoreDisplay();
        }
        
        private void SetupButtonListeners()
        {
            // Menu buttons
            speedItUpButton?.onClick.AddListener(() => StartLesson(LessonType.SpeedItUp));
            performSongButton?.onClick.AddListener(() => StartLesson(LessonType.PerformTheSong));
            
            // Gameplay buttons
            pauseButton?.onClick.AddListener(PauseGame);
            
            // Pause buttons
            resumeButton?.onClick.AddListener(ResumeGame);
            restartButton?.onClick.AddListener(RestartGame);
            mainMenuButton?.onClick.AddListener(ReturnToMenu);
            
            // Results buttons
            playAgainButton?.onClick.AddListener(RestartGame);
            backToMenuButton?.onClick.AddListener(ReturnToMenu);
        }
        
        private void SubscribeToEvents()
        {
            GameManager.OnGameStateChanged.AddListener(OnGameStateChanged);
            ScoreManager.OnScoreChanged.AddListener(UpdateScoreDisplay);
            ScoreManager.OnHighScoreChanged.AddListener(OnNewHighScore);
        }
        
        private void OnGameStateChanged(GameState newState)
        {
            currentState = newState;
            ShowPanel(newState);
            
            switch (newState)
            {
                case GameState.Playing:
                    StartProgressUpdate();
                    break;
                case GameState.Paused:
                    StopProgressUpdate();
                    break;
                case GameState.Results:
                    StopProgressUpdate();
                    ShowResults();
                    break;
                case GameState.Menu:
                    ResetUI();
                    break;
            }
        }
        
        private void ShowPanel(GameState state)
        {
            // Hide all panels first
            menuPanel?.SetActive(false);
            gameplayPanel?.SetActive(false);
            pausePanel?.SetActive(false);
            resultsPanel?.SetActive(false);
            
            // Show appropriate panel
            switch (state)
            {
                case GameState.Menu:
                    menuPanel?.SetActive(true);
                    break;
                case GameState.Playing:
                    gameplayPanel?.SetActive(true);
                    break;
                case GameState.Paused:
                    gameplayPanel?.SetActive(true);
                    pausePanel?.SetActive(true);
                    break;
                case GameState.Results:
                    resultsPanel?.SetActive(true);
                    break;
            }
        }
        
        private void StartLesson(LessonType lessonType)
        {
            GameManager.Instance.StartLesson(lessonType);
            
            // Update game mode text
            string modeText = lessonType == LessonType.SpeedItUp ? "Speed It Up - Rhythm Tap" : "Perform The Song";
            gameModeText?.SetText(modeText);
            
            ResetGameplayUI();
        }
        
        private void PauseGame()
        {
            Time.timeScale = 0f;
            GameManager.Instance.ChangeGameState(GameState.Paused);
        }
        
        private void ResumeGame()
        {
            Time.timeScale = 1f;
            GameManager.Instance.ChangeGameState(GameState.Playing);
        }
        
        private void RestartGame()
        {
            Time.timeScale = 1f;
            GameManager.Instance.scoreManager.ResetScore();
            
            // Get current lesson type and restart
            LessonType currentLesson = GetCurrentLessonType();
            GameManager.Instance.StartLesson(currentLesson);
        }
        
        private void ReturnToMenu()
        {
            Time.timeScale = 1f;
            GameManager.Instance.ChangeGameState(GameState.Menu);
        }
        
        private LessonType GetCurrentLessonType()
        {
            // This would need to be stored or determined from current game mode
            return LessonType.SpeedItUp; // Default fallback
        }
        
        private void UpdateScoreDisplay(int newScore)
        {
            currentScoreText?.SetText($"Score: {newScore:N0}");
        }
        
        private void UpdateHighScoreDisplay()
        {
            if (highScoreText != null && GameManager.Instance?.scoreManager != null)
            {
                highScoreText.SetText($"High Score: {GameManager.Instance.scoreManager.HighScore:N0}");
            }
        }
        
        private void OnNewHighScore(int newHighScore)
        {
            UpdateHighScoreDisplay();
            
            // Show celebration effect
            if (newHighScoreText != null)
            {
                newHighScoreText.gameObject.SetActive(true);
                StartCoroutine(FadeOutNewHighScore());
            }
        }
        
        private IEnumerator FadeOutNewHighScore()
        {
            yield return new WaitForSeconds(3f);
            
            if (newHighScoreText != null)
            {
                float alpha = 1f;
                Color originalColor = newHighScoreText.color;
                
                while (alpha > 0f)
                {
                    alpha -= Time.deltaTime;
                    newHighScoreText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                    yield return null;
                }
                
                newHighScoreText.gameObject.SetActive(false);
                newHighScoreText.color = originalColor;
            }
        }
        
        public void UpdateBPM(float bpm)
        {
            bpmText?.SetText($"BPM: {bpm:F0}");
        }
        
        public void UpdateCombo(int combo)
        {
            currentCombo = combo;
            comboText?.SetText(combo > 0 ? $"Combo: {combo}" : "");
        }
        
        public void ShowHitIndicator(string hitType, Vector3 position)
        {
            if (hitIndicatorPrefab != null && hitIndicatorParent != null)
            {
                GameObject indicator = Instantiate(hitIndicatorPrefab, hitIndicatorParent);
                indicator.transform.position = position;
                
                TextMeshProUGUI indicatorText = indicator.GetComponent<TextMeshProUGUI>();
                if (indicatorText != null)
                {
                    indicatorText.text = hitType;
                    indicatorText.color = GetHitColor(hitType);
                }
                
                StartCoroutine(DestroyIndicatorAfterDelay(indicator));
            }
        }
        
        private Color GetHitColor(string hitType)
        {
            return hitType.ToLower() switch
            {
                "perfect" => perfectHitColor,
                "good" => goodHitColor,
                "miss" => missColor,
                _ => Color.white
            };
        }
        
        private IEnumerator DestroyIndicatorAfterDelay(GameObject indicator)
        {
            yield return new WaitForSeconds(hitIndicatorLifetime);
            
            if (indicator != null)
            {
                Destroy(indicator);
            }
        }
        
        public void FlashScreen(string hitType)
        {
            if (screenFlash != null)
            {
                StartCoroutine(ScreenFlashEffect(GetHitColor(hitType)));
            }
        }
        
        private IEnumerator ScreenFlashEffect(Color flashColor)
        {
            screenFlash.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0.3f);
            screenFlash.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(0.1f);
            
            screenFlash.gameObject.SetActive(false);
        }
        
        private void StartProgressUpdate()
        {
            if (progressUpdateCoroutine != null)
            {
                StopCoroutine(progressUpdateCoroutine);
            }
            
            progressUpdateCoroutine = StartCoroutine(UpdateProgressBar());
        }
        
        private void StopProgressUpdate()
        {
            if (progressUpdateCoroutine != null)
            {
                StopCoroutine(progressUpdateCoroutine);
                progressUpdateCoroutine = null;
            }
        }
        
        private IEnumerator UpdateProgressBar()
        {
            while (currentState == GameState.Playing)
            {
                if (progressSlider != null && GameManager.Instance?.audioManager != null)
                {
                    float songPosition = GameManager.Instance.audioManager.GetSongPosition();
                    // This would need song duration from AudioManager
                    float songDuration = 180f; // Placeholder - would get from AudioManager
                    progressSlider.value = songPosition / songDuration;
                }
                
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        private void ShowResults()
        {
            if (GameManager.Instance?.scoreManager != null)
            {
                int finalScore = GameManager.Instance.scoreManager.CurrentScore;
                finalScoreText?.SetText($"Final Score: {finalScore:N0}");
                
                // Calculate accuracy (would need more detailed tracking)
                float accuracy = CalculateAccuracy();
                accuracyText?.SetText($"Accuracy: {accuracy:P1}");
            }
        }
        
        private float CalculateAccuracy()
        {
            // This would need to track hits vs total notes
            return Random.Range(0.7f, 1f); // Placeholder
        }
        
        private void ResetGameplayUI()
        {
            currentCombo = 0;
            UpdateCombo(0);
            UpdateScoreDisplay(0);
            
            if (progressSlider != null)
            {
                progressSlider.value = 0f;
            }
        }
        
        private void ResetUI()
        {
            ResetGameplayUI();
            UpdateHighScoreDisplay();
            
            if (newHighScoreText != null)
            {
                newHighScoreText.gameObject.SetActive(false);
            }
        }
        
        void OnDestroy()
        {
            // Cleanup
            if (progressUpdateCoroutine != null)
            {
                StopCoroutine(progressUpdateCoroutine);
            }
            
            // Unsubscribe from events
            GameManager.OnGameStateChanged.RemoveListener(OnGameStateChanged);
            ScoreManager.OnScoreChanged.RemoveListener(UpdateScoreDisplay);
            ScoreManager.OnHighScoreChanged.RemoveListener(OnNewHighScore);
        }
        
        // Public methods for game modes to call
        public void OnNoteHit(string hitType, Vector3 worldPosition = default)
        {
            UpdateCombo(currentCombo + 1);
            ShowHitIndicator(hitType, worldPosition);
            FlashScreen(hitType);
        }
        
        public void OnNoteMiss()
        {
            UpdateCombo(0);
            ShowHitIndicator("Miss", Vector3.zero);
            FlashScreen("Miss");
        }
        
        public void UpdateGameModeSpecificUI(string key, object value)
        {
            // For mode-specific UI updates
            switch (key.ToLower())
            {
                case "bpm":
                    if (value is float bpm)
                        UpdateBPM(bpm);
                    break;
            }
        }
    }
}