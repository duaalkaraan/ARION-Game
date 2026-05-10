using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip explosionClip; // assign explosion sound in inspector
    public AudioClip interactClip;  // sound to play when player presses E
    public AudioClip[] soundEffects;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 1f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private AudioSource musicSource; // used for background music
    private AudioSource sfxSource;   // used for one-shot SFX

    void Awake()
    {
        // Primary AudioSource (already required) will be used as music source
        musicSource = GetComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;

        // Create a separate AudioSource for sound effects so music won't be interrupted
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;
    }

    void Start()
    {
        if (backgroundMusic != null)
        {
            PlayBackground();
        }
    }

    void Update()
    {
        // Play interact sound when player presses E
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayInteract();
        }
    }

    // Background music control
    public void PlayBackground()
    {
        if (backgroundMusic == null || musicSource == null) return;
        musicSource.clip = backgroundMusic;
        musicSource.volume = musicVolume;
        if (!musicSource.isPlaying) musicSource.Play();
    }

    public void StopBackground()
    {
        if (musicSource != null && musicSource.isPlaying) musicSource.Stop();
    }

    // Play explosion sound (call this from your bomb script when bomb explodes)
    public void PlayExplosion()
    {
        if (explosionClip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(explosionClip, sfxVolume);
    }

    // Play the interact sound (called on E key press)
    public void PlayInteract()
    {
        if (interactClip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(interactClip, sfxVolume);
    }

    // Play a sound effect from the array by index
    public void PlaySFX(int index)
    {
        if (soundEffects == null || index < 0 || index >= soundEffects.Length || sfxSource == null) return;
        sfxSource.PlayOneShot(soundEffects[index], sfxVolume);
    }

    // Play any AudioClip once (useful for ad-hoc SFX)
    public void PlaySFXOneShot(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale) * sfxVolume);
    }

    // Adjust music volume (0..1)
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null) musicSource.volume = musicVolume;
    }

    // Adjust SFX volume (0..1)
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }

    // Toggle mute on/off for both sources
    public void ToggleMute()
    {
        if (musicSource != null) musicSource.mute = !musicSource.mute;
        if (sfxSource != null) sfxSource.mute = !sfxSource.mute;
    }
}
