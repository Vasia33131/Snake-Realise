using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Settings")]
    [Range(0, 1)] public float musicVolume = 0.5f;
    [Range(0, 1)] public float sfxVolume = 0.7f;
    public bool musicEnabled = true;
    public bool sfxEnabled = true;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip eatSound;

    [Header("UI References")]
    public Image musicButtonImage;
    public Image sfxButtonImage;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public Sprite sfxOnSprite;
    public Sprite sfxOffSprite;

    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
            UpdateButtonGraphics();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeAudio()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.volume = musicEnabled ? musicVolume : 0f;
        PlayBackgroundMusic();
    }

    private void UpdateButtonGraphics()
    {
        if (musicButtonImage != null)
        {
            musicButtonImage.sprite = musicEnabled ? musicOnSprite : musicOffSprite;
        }

        if (sfxButtonImage != null)
        {
            sfxButtonImage.sprite = sfxEnabled ? sfxOnSprite : sfxOffSprite;
        }
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource != null && !musicSource.isPlaying && musicEnabled)
        {
            musicSource.Play();
        }
    }

    public void PlayEatSound()
    {
        if (sfxEnabled)
        {
            AudioSource.PlayClipAtPoint(eatSound, Camera.main.transform.position, sfxVolume);
        }
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        if (musicSource != null)
        {
            musicSource.volume = musicEnabled ? musicVolume : 0f;
            if (musicEnabled && !musicSource.isPlaying)
            {
                musicSource.Play();
            }
            else if (!musicEnabled)
            {
                musicSource.Stop();
            }
        }
        UpdateButtonGraphics();
    }

    public void ToggleSFX()
    {
        sfxEnabled = !sfxEnabled;
        UpdateButtonGraphics();
    }

    public void UpdateMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null && musicEnabled)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void UpdateSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    // Методы для UI кнопок
    public void OnMusicButtonPressed()
    {
        ToggleMusic();
    }

    public void OnSFXButtonPressed()
    {
        ToggleSFX();
    }
}