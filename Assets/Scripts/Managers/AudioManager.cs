using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 0.7f;
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

    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeAudio();
        UpdateButtonGraphics();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void InitializeAudio()
    {
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.clip = backgroundMusic;
        _musicSource.loop = true;
        _musicSource.volume = musicEnabled ? musicVolume : 0f;

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;
        _sfxSource.loop = false;

        PlayBackgroundMusic();
    }

    private void UpdateButtonGraphics()
    {
        if (musicButtonImage != null)
            musicButtonImage.sprite = musicEnabled ? musicOnSprite : musicOffSprite;

        if (sfxButtonImage != null)
            sfxButtonImage.sprite = sfxEnabled ? sfxOnSprite : sfxOffSprite;
    }

    public void PlayBackgroundMusic()
    {
        if (_musicSource != null && !_musicSource.isPlaying && musicEnabled)
            _musicSource.Play();
    }

    public void PlayEatSound()
    {
        if (!sfxEnabled || eatSound == null || _sfxSource == null)
            return;

        _sfxSource.PlayOneShot(eatSound, sfxVolume);
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        if (_musicSource != null)
        {
            _musicSource.volume = musicEnabled ? musicVolume : 0f;
            if (musicEnabled && !_musicSource.isPlaying)
                _musicSource.Play();
            else if (!musicEnabled)
                _musicSource.Stop();
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
        if (_musicSource != null && musicEnabled)
            _musicSource.volume = musicVolume;
    }

    public void UpdateSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    public void OnMusicButtonPressed() => ToggleMusic();

    public void OnSFXButtonPressed() => ToggleSFX();
}
