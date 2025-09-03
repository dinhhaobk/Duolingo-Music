using UnityEngine;

namespace DuolingoMusic.Config
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Duolingo Music/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Performance Settings")]
        public int targetFPS = 60;
        public float maxLatency = 50f; // milliseconds
        
        [Header("Game Modes")]
        public RhythmTapConfig rhythmTapConfig;
        public PerformanceModeConfig performanceModeConfig;
        
        [Header("Audio Settings")]
        public AudioConfig audioConfig;
    }
    
    [System.Serializable]
    public class RhythmTapConfig
    {
        public float baseBPM = 120f;
        public float bpmIncrement = 10f;
        public float bpmDecrement = 10f;
        public int consecutiveHitsForSpeedup = 4;
        public int consecutiveMissesForSlowdown = 3;
        public float hitWindow = 0.1f; // seconds
    }
    
    [System.Serializable]
    public class PerformanceModeConfig
    {
        public float perfectHitWindow = 0.05f;
        public float goodHitWindow = 0.1f;
        public int maxConsecutiveMisses = 3;
        public bool enableMuteOnMiss = true;
        public bool enableUnmuteOnHit = true;
    }
    
    [System.Serializable]
    public class AudioConfig
    {
        public float masterVolume = 1f;
        public float musicVolume = 0.8f;
        public float sfxVolume = 0.7f;
    }
}