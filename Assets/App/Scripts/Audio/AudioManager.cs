using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DuolingoMusic.Config;
using DuolingoMusic.Core;

namespace DuolingoMusic.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource vocalSource;
        
        [Header("Audio Clips")]
        [SerializeField] private List<AudioClip> musicTracks;
        [SerializeField] private List<AudioClip> sfxClips;
        
        private AudioConfig config;
        private float currentBPM = 120f;
        
        public float CurrentBPM => currentBPM;
        public bool IsPlaying => musicSource.isPlaying;
        
        public void Initialize()
        {
            config = GameManager.Instance.gameConfig.audioConfig;
            ApplyAudioSettings();
        }
        
        private void ApplyAudioSettings()
        {
            musicSource.volume = config.musicVolume * config.masterVolume;
            sfxSource.volume = config.sfxVolume * config.masterVolume;
            vocalSource.volume = config.musicVolume * config.masterVolume;
        }
        
        public void PlayMusic(AudioClip clip, float bpm = 120f)
        {
            currentBPM = bpm;
            musicSource.clip = clip;
            musicSource.Play();
        }
        
        public void SetBPM(float newBPM)
        {
            currentBPM = newBPM;
            musicSource.pitch = newBPM / 120f; // Assuming 120 BPM as base
        }
        
        public void MuteVocal()
        {
            vocalSource.mute = true;
        }
        
        public void UnmuteVocal()
        {
            vocalSource.mute = false;
        }
        
        public void PlaySFX(string clipName)
        {
            AudioClip clip = sfxClips.Find(c => c.name == clipName);
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }
        
        public float GetSongPosition()
        {
            return musicSource.time;
        }
    }
}