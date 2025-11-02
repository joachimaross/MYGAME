using UnityEngine;
using UnityEngine.Audio;

namespace MarketHustle.Audio
{
    /// <summary>
    /// Manages audio playback for the game: background music, sound effects, and UI sounds.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource sfxSource;

        [Header("Audio Clips")]
        public AudioClip backgroundMusic;
        public AudioClip buttonClick;
        public AudioClip moneyEarned;
        public AudioClip propertyPurchased;
        public AudioClip doorOpen;
        public AudioClip pickupItem;

        [Header("Settings")]
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float musicVolume = 0.5f;
        [Range(0f, 1f)] public float sfxVolume = 0.8f;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudioSources();
        }

        void InitializeAudioSources()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }

            UpdateVolumes();
        }

        void Start()
        {
            PlayBackgroundMusic();
        }

        void UpdateVolumes()
        {
            if (musicSource != null)
                musicSource.volume = masterVolume * musicVolume;

            if (sfxSource != null)
                sfxSource.volume = masterVolume * sfxVolume;
        }

        public void PlayBackgroundMusic()
        {
            if (musicSource != null && backgroundMusic != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.Play();
            }
        }

        public void PlaySFX(AudioClip clip)
        {
            if (sfxSource != null && clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }

        // Convenience methods for common sounds
        public void PlayButtonClick()
        {
            PlaySFX(buttonClick);
        }

        public void PlayMoneyEarned()
        {
            PlaySFX(moneyEarned);
        }

        public void PlayPropertyPurchased()
        {
            PlaySFX(propertyPurchased);
        }

        public void PlayDoorOpen()
        {
            PlaySFX(doorOpen);
        }

        public void PlayPickupItem()
        {
            PlaySFX(pickupItem);
        }

        // Volume controls
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        // Mute controls
        public void MuteMusic(bool mute)
        {
            if (musicSource != null)
                musicSource.mute = mute;
        }

        public void MuteSFX(bool mute)
        {
            if (sfxSource != null)
                sfxSource.mute = mute;
        }
    }
}