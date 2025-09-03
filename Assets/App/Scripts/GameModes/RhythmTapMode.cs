using UnityEngine;
using DuolingoMusic.Config;
using DuolingoMusic.Core;
using DuolingoMusic.Audio;

namespace DuolingoMusic.GameModes
{
    public class RhythmTapMode : IGameMode
    {
        private RhythmTapConfig config;
        private AudioManager audioManager;
        private ScoreManager scoreManager;
        
        private float currentBPM;
        private int consecutiveHits = 0;
        private int consecutiveMisses = 0;
        private float lastBeatTime = 0f;
        private float beatInterval;
        
        public RhythmTapMode(RhythmTapConfig config)
        {
            this.config = config;
        }
        
        public void Initialize()
        {
            audioManager = GameManager.Instance.audioManager;
            scoreManager = GameManager.Instance.scoreManager;
            
            currentBPM = config.baseBPM;
            beatInterval = 60f / currentBPM;
            
            // Start music
            // audioManager.PlayMusic(selectedTrack, currentBPM);
            
            Debug.Log($"Rhythm Tap Mode initialized with BPM: {currentBPM}");
        }
        
        public void Update()
        {
            // Check for missed beats
            CheckForMissedBeats();
        }
        
        public void OnInput(InputType inputType)
        {
            if (inputType == InputType.Tap)
            {
                ProcessTap();
            }
        }
        
        private void ProcessTap()
        {
            float currentTime = audioManager.GetSongPosition();
            float timeSinceLastBeat = currentTime - lastBeatTime;
            
            // Check if tap is within hit window
            if (Mathf.Abs(timeSinceLastBeat % beatInterval) <= config.hitWindow)
            {
                OnHit();
            }
            else
            {
                OnMiss();
            }
        }
        
        private void OnHit()
        {
            consecutiveHits++;
            consecutiveMisses = 0;
            
            scoreManager.AddScore(100);
            audioManager.PlaySFX("hit");
            
            // Check for speed increase
            if (consecutiveHits >= config.consecutiveHitsForSpeedup)
            {
                IncreaseBPM();
                consecutiveHits = 0;
            }
        }
        
        private void OnMiss()
        {
            consecutiveMisses++;
            consecutiveHits = 0;
            
            audioManager.PlaySFX("miss");
            
            // Check for speed decrease
            if (consecutiveMisses >= config.consecutiveMissesForSlowdown)
            {
                DecreaseBPM();
                consecutiveMisses = 0;
            }
        }
        
        private void IncreaseBPM()
        {
            currentBPM += config.bpmIncrement;
            audioManager.SetBPM(currentBPM);
            beatInterval = 60f / currentBPM;
            
            Debug.Log($"BPM increased to: {currentBPM}");
        }
        
        private void DecreaseBPM()
        {
            currentBPM = Mathf.Max(60f, currentBPM - config.bpmDecrement);
            audioManager.SetBPM(currentBPM);
            beatInterval = 60f / currentBPM;
            
            Debug.Log($"BPM decreased to: {currentBPM}");
        }
        
        private void CheckForMissedBeats()
        {
            // Implementation for detecting missed beats
        }
        
        public void Cleanup()
        {
            // Clean up resources
        }
    }
}