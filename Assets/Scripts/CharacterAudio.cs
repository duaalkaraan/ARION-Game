using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterAudio : MonoBehaviour
{
    [SerializeField] AudioClip jumpClip;
    [SerializeField] AudioClip damageClip;
    [SerializeField, Range(0f, 1f)] float volume = 1f;

    AudioSource src;

    void Awake()
    {
        src = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
    }

    public void PlayJump() { if (jumpClip != null) src.PlayOneShot(jumpClip, volume); }
    public void PlayDamage() { if (damageClip != null) src.PlayOneShot(damageClip, volume); }
    public void PlayOneShot(AudioClip clip) { if (clip != null) src.PlayOneShot(clip, volume); }
}
