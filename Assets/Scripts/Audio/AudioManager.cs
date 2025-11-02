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
        public AudioClip cityMusic;
        public AudioClip storeMusic;
        public AudioClip homeMusic;
        public AudioClip buttonClick;
        public AudioClip moneyEarned;
        public AudioClip chaChing;
        public AudioClip propertyPurchased;
        public AudioClip doorOpen;
        public AudioClip pickupItem;
        public AudioClip levelUpSound;
        public AudioClip eventNotification;
        public AudioClip reputationIncrease;
        public AudioClip customerSatisfied;
        public AudioClip customerDissatisfied;

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

        public void PlayChaChing()
        {
            PlaySFX(chaChing);
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

        public void PlayLevelUp()
        {
            PlaySFX(levelUpSound);
        }

        public void PlayEventNotification()
        {
            PlaySFX(eventNotification);
        }

        public void PlayReputationIncrease()
        {
            PlaySFX(reputationIncrease);
        }

        public void PlayCustomerSatisfied()
        {
            PlaySFX(customerSatisfied);
        }

        public void PlayCustomerDissatisfied()
        {
            PlaySFX(customerDissatisfied);
        }

        // Dynamic music switching based on location
        public void SwitchToCityMusic()
        {
            if (musicSource != null && cityMusic != null)
            {
                musicSource.clip = cityMusic;
                musicSource.Play();
            }
        }

        public void SwitchToStoreMusic()
        {
            if (musicSource != null && storeMusic != null)
            {
                musicSource.clip = storeMusic;
                musicSource.Play();
            }
        }

        public void SwitchToHomeMusic()
        {
            if (musicSource != null && homeMusic != null)
            {
                musicSource.clip = homeMusic;
                musicSource.Play();
            }
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