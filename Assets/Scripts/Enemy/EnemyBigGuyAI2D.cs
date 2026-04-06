using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[DisallowMultipleComponent]
public sealed class EnemyBigGuyAI2D : MonoBehaviour
{
    [Header("Hareket")]
    [SerializeField] float moveSpeed = 4.5f;
    [SerializeField] float attackRange = 1.4f;
    [SerializeField] float viewDistance = 6f;
    [SerializeField] float viewHalfAngle = 45f;
    [SerializeField] LayerMask obstacleMask;

    [Header("Saldırı")]
    [SerializeField] float attackCooldown = 0.7f;
    [SerializeField] int contactDamage = 10;

    [Header("Animasyon")]
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header("Hedef")]
    [SerializeField] Transform playerTargetOverride;

    Rigidbody2D rb;
    Collider2D col;
    PlayerMovement2D player;
    float nextAttackTime;
    string lastDesiredState;
    bool dead;
    Coroutine deathRoutine;

    // Basit devriye için
    float idlePatrolTimer;
    bool isPatrollingShortStep;

    // ✔ Enemy bileşenlerini hazırlayan başlangıç metodu
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = animator ? animator : GetComponent<Animator>();
        spriteRenderer = spriteRenderer ? spriteRenderer : GetComponent<SpriteRenderer>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        lastDesiredState = string.Empty;
        idlePatrolTimer = Random.Range(4f, 7f);
        isPatrollingShortStep = false;
    }

    // ✔ Enemy’nin ana yapay zekâ döngüsü (görme, takip, saldırı, devriye)
    void FixedUpdate()
    {
        if (dead)
            return;

        EnsurePlayer();
        if (player == null)
            return;

        if (player.IsDead)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (animator != null)
                TryCrossFade("Idle", 0.05f);
            return;
        }

        Vector2 myPos = transform.position;
        Vector2 playerPos = player.transform.position;
        Vector2 toPlayer = playerPos - myPos;
        float distance = toPlayer.magnitude;

        bool canSeePlayer = true;

        // Mesafe içinde değilse oyuncuyu görmüyor
        if (distance > viewDistance)
            canSeePlayer = false;

        // Enemy'nin baktığı yöne göre görüş açısı
        Vector2 forward = Vector2.right;
        if (spriteRenderer != null && spriteRenderer.flipX)
            forward = Vector2.left;

        if (toPlayer.sqrMagnitude > 0.0001f)
        {
            float angle = Vector2.Angle(forward, toPlayer.normalized);
            if (angle > viewHalfAngle)
                canSeePlayer = false;
        }

        // Arada duvar / zemin varsa oyuncuyu görmesin
        if (obstacleMask.value != 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(myPos, toPlayer.normalized, distance, obstacleMask);
            if (hit.collider != null)
                canSeePlayer = false;
        }

        if (!canSeePlayer)
        {
            HandleIdlePatrol();
            return;
        }

        float dx = playerPos.x - myPos.x;

        if (distance <= attackRange)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (spriteRenderer != null && Mathf.Abs(dx) > 0.001f)
                spriteRenderer.flipX = dx < 0f;

            if (animator != null)
                TryCrossFade("Attack", 0.05f);

            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackCooldown;
                player.TakeDamage(contactDamage);
            }

            return;
        }

        float dir = Mathf.Sign(dx);
        if (dir == 0f)
            dir = 1f;

        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);

        if (spriteRenderer != null)
            spriteRenderer.flipX = dir < 0f;

        if (animator != null)
            TryCrossFade("Run", 0.05f);
    }

    // ✔ Oyuncu referansını bulur veya override edilmiş hedefi kullanır
    void EnsurePlayer()
    {
        if (player != null)
            return;

        if (playerTargetOverride != null)
            player = playerTargetOverride.GetComponent<PlayerMovement2D>();

        if (player == null)
            player = FindFirstObjectByType<PlayerMovement2D>();
    }

    // ✔ Oyuncu görünmüyorsa kısa devriye davranışını yönetir
    void HandleIdlePatrol()
    {
        // Küçük idle-patrol: 5–6 saniyede bir yön değiştir, 3–4 adım yürü
        if (!isPatrollingShortStep)
        {
            idlePatrolTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (animator != null)
                TryCrossFade("Idle", 0.05f);

            if (idlePatrolTimer <= 0f)
            {
                // Yönü çevir
                if (spriteRenderer != null)
                    spriteRenderer.flipX = !spriteRenderer.flipX;

                // Kısa yürüyüşe başla
                isPatrollingShortStep = true;
                idlePatrolTimer = 0.7f; // yaklaşık 3–4 adım
            }
            return;
        }

        // Kısa yürüme fazı
        idlePatrolTimer -= Time.fixedDeltaTime;

        float dir = 1f;
        if (spriteRenderer != null && spriteRenderer.flipX)
            dir = -1f;

        rb.linearVelocity = new Vector2(dir * (moveSpeed * 0.6f), rb.linearVelocity.y);

        if (animator != null)
            TryCrossFade("Run", 0.08f);

        if (idlePatrolTimer <= 0f)
        {
            isPatrollingShortStep = false;
            idlePatrolTimer = Random.Range(4f, 7f); // tekrar bekleme süresi
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    // ✔ Animasyon geçişlerini kontrol eder, gereksiz tekrarları engeller
    void TryCrossFade(string desiredState, float duration)
    {
        if (string.Equals(lastDesiredState, desiredState))
            return;

        var cur = animator.GetCurrentAnimatorStateInfo(0);
        if (cur.IsName(desiredState) && !animator.IsInTransition(0))
        {
            lastDesiredState = desiredState;
            return;
        }

        lastDesiredState = desiredState;
        animator.CrossFade(desiredState, duration, 0);
    }

    // ✔ Enemy’yi öldürme işlemini başlatır
    public void Die()
    {
        if (dead)
            return;

        dead = true;
        if (deathRoutine == null)
            deathRoutine = StartCoroutine(DeathRoutine());
    }

    // ✔ Ölüm animasyonlarını oynatır ve enemy’yi sahneden kaldırır
    IEnumerator DeathRoutine()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
        if (col != null)
            col.enabled = false;

        if (animator != null)
            animator.Play("DeadHit", 0, 0f);

        yield return new WaitForSeconds(0.55f);

        if (animator != null)
            animator.Play("DeadGround", 0, 0f);

        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);
    }
}
