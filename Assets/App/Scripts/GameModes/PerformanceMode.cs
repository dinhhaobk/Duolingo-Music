using UnityEngine;
using DuolingoMusic.Config;
using DuolingoMusic.Core;
using DuolingoMusic.Audio;

namespace DuolingoMusic.GameModes
{
    public class PerformanceMode : IGameMode
    {
        private PerformanceModeConfig config;
        private AudioManager audioManager;
        private ScoreManager scoreManager;
        
        private int consecutiveMisses = 0;
        private bool isVocalMuted = false;
        
        public PerformanceMode(PerformanceModeConfig config)
        {
            this.config = config;
        }
        
        public void Initialize()
        {
            audioManager = GameManager.Instance.audioManager;
            scoreManager = GameManager.Instance.scoreManager;
            
            Debug.Log("Performance Mode initialized");
        }
        
        public void Update()
        {
            // Performance mode specific updates
        }
        
        public void OnInput(InputType inputType)
        {
            if (inputType == InputType.Tap)
            {
                ProcessNote();
            }
        }
        
        private void ProcessNote()
        {
            // Simplified note processing - would need note timing system
            float accuracy = CalculateAccuracy();
            
            if (accuracy <= config.perfectHitWindow)
            {
                OnPerfectHit();
            }
            else if (accuracy <= config.goodHitWindow)
            {
                OnGoodHit();
            }
            else
            {
                OnMiss();
            }
        }
        
        private float CalculateAccuracy()
        {
            // This would calculate timing accuracy against expected note time
            return Random.Range(0f, 0.15f); // Placeholder
        }
        
        private void OnPerfectHit()
        {
            consecutiveMisses = 0;
            scoreManager.AddScore(150);
            audioManager.PlaySFX("perfect");
            
            if (isVocalMuted && config.enableUnmuteOnHit)
            {
                audioManager.UnmuteVocal();
                isVocalMuted = false;
            }
        }
        
        private void OnGoodHit()
        {
            consecutiveMisses = 0;
            scoreManager.AddScore(100);
            audioManager.PlaySFX("good");
            
            if (isVocalMuted && config.enableUnmuteOnHit)
            {
                audioManager.UnmuteVocal();
                isVocalMuted = false;
            }
        }
        
        private void OnMiss()
        {
            consecutiveMisses++;
            audioManager.PlaySFX("miss");
            
            if (consecutiveMisses >= config.maxConsecutiveMisses && config.enableMuteOnMiss)
            {
                audioManager.MuteVocal();
                isVocalMuted = true;
            }
        }
        
        public void Cleanup()
        {
            if (isVocalMuted)
            {
                audioManager.UnmuteVocal();
            }
        }
    }
}