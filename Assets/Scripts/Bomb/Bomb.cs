using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class Bomb : MonoBehaviour
{
    [Header("Zamanlama")]
    [SerializeField] float fuseSeconds = 3f;
    [SerializeField] float explosionLifetime = 0.8f;

    [Header("Hasar Alanı")]
    [SerializeField] float killRadius = 2.2f;
    [SerializeField] LayerMask hitMask = ~0;

    [Header("Görsel")]
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject explosionPrefab;

    [Header("Ses")]
    [SerializeField] AudioClip explosionSound;
    AudioSource audioSource;

    bool exploded;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        animator = animator ? animator : GetComponent<Animator>();
        spriteRenderer = spriteRenderer ? spriteRenderer : GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        exploded = false;
        StartCoroutine(FuseRoutine());
    }

    IEnumerator FuseRoutine()
    {
        if (animator != null)
            animator.Play("BombOn", 0, 0f);
        yield return new WaitForSeconds(fuseSeconds);
        Explode();
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (audioSource != null && explosionSound != null)
            audioSource.PlayOneShot(explosionSound);

        var hits = Physics2D.OverlapCircleAll(transform.position, killRadius, hitMask);
        for (int i = 0; i < hits.Length; i++)
        {
            var c = hits[i];
            if (c == null) continue;

            var player = c.GetComponentInParent<PlayerMovement2D>();
            if (player != null) player.Die();

            var enemy = c.GetComponentInParent<EnemyBigGuyAI2D>();
            if (enemy != null) enemy.Die();
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (explosionPrefab != null)
        {
            var pos = transform.position;
            pos.z = 0f;
            var fx = Instantiate(explosionPrefab, pos, Quaternion.identity);
            // Try to play AudioSource on the spawned effect (preferred)
            var fxAudio = fx.GetComponent<AudioSource>();
            if (fxAudio != null)
            {
                fxAudio.Play();
            }
            else
            {
                // Fallback: find global audio manager and play explosion clip
                var audioManager = FindObjectOfType<AudioManager>();
                if (audioManager != null)
                    audioManager.PlayExplosion();
            }

            Destroy(fx, explosionLifetime);
        }
        else
        {
            // No prefab: try global audio manager
            var audioManager = FindObjectOfType<AudioManager>();
            if (audioManager != null)
                audioManager.PlayExplosion();
        }

        Destroy(gameObject, explosionSound != null ? explosionSound.length : 0.05f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.1f, 0.35f);
        Gizmos.DrawSphere(transform.position, killRadius);
    }
}