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

    bool exploded;

    void Awake()
    {
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
        if (exploded)
            return;
        exploded = true;

        // Kill in radius
        var hits = Physics2D.OverlapCircleAll(transform.position, killRadius, hitMask);
        for (int i = 0; i < hits.Length; i++)
        {
            var c = hits[i];
            if (c == null) continue;

            var player = c.GetComponentInParent<PlayerMovement2D>();
            if (player != null)
                player.Die();

            var enemy = c.GetComponentInParent<EnemyBigGuyAI2D>();
            if (enemy != null)
                enemy.Die();
        }

        // Hide bomb sprite
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        // Spawn explosion effect
        if (explosionPrefab != null)
        {
            var pos = transform.position;
            pos.z = 0f;
            var fx = Instantiate(explosionPrefab, pos, Quaternion.identity);
            Destroy(fx, explosionLifetime);
        }

        Destroy(gameObject, 0.05f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.1f, 0.35f);
        Gizmos.DrawSphere(transform.position, killRadius);
    }
}

