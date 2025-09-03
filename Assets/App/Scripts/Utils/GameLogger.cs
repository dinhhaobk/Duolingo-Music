using UnityEngine;
using System.Collections.Generic;

namespace DuolingoMusic.Utils
{
    public class GameLogger : MonoBehaviour
    {
        private List<LogEntry> logEntries = new List<LogEntry>();
        
        public static GameLogger Instance { get; private set; }
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void LogNoteHit(float accuracy, string result)
        {
            var entry = new LogEntry
            {
                type = "note_hit",
                timestamp = Time.time,
                data = new Dictionary<string, object>
                {
                    {"accuracy", accuracy},
                    {"result", result}
                }
            };
            
            logEntries.Add(entry);
            Debug.Log(JsonUtility.ToJson(entry));
        }
        
        public void LogNoteMiss(float expectedTime, float actualTime)
        {
            var entry = new LogEntry
            {
                type = "note_miss",
                timestamp = Time.time,
                data = new Dictionary<string, object>
                {
                    {"expected_time", expectedTime},
                    {"actual_time", actualTime}
                }
            };
            
            logEntries.Add(entry);
            Debug.Log(JsonUtility.ToJson(entry));
        }
        
        public void LogSongEnd(int finalScore, float accuracy)
        {
            var entry = new LogEntry
            {
                type = "song_end",
                timestamp = Time.time,
                data = new Dictionary<string, object>
                {
                    {"final_score", finalScore},
                    {"accuracy", accuracy}
                }
            };
            
            logEntries.Add(entry);
            Debug.Log(JsonUtility.ToJson(entry));
        }
        
        [System.Serializable]
        public class LogEntry
        {
            public string type;
            public float timestamp;
            public Dictionary<string, object> data;
        }
    }
}