using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[DisallowMultipleComponent]
public sealed class PlayerMovement2D : MonoBehaviour
{
    [Header("Hareket")]
    [SerializeField] float moveSpeed = 11f;
    [SerializeField] float acceleration = 220f;
    [SerializeField] float airAcceleration = 90f;
    [SerializeField] float groundFriction = 160f;

    [Header("Zıplama")]
    [SerializeField] float jumpForce = 14.5f;
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] float jumpBuffer = 0.08f;
    [SerializeField] float fallGravityMultiplier = 2.4f;

    [Header("Zemin")]
    [SerializeField] LayerMask groundMask = ~0;
    [SerializeField] float groundCheckDistance = 0.14f;
    [SerializeField] Vector2 groundCheckOffset;

    [Header("Animasyon")]
    [SerializeField, Min(0.02f)] float locomotionCrossFade = 0.08f;

    [Header("Referanslar")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    static readonly int IdleStateHash = Animator.StringToHash("Idle");
    static readonly int RunStateHash = Animator.StringToHash("Run");
    static readonly int JumpStateHash = Animator.StringToHash("Jump");
    static readonly int FallStateHash = Animator.StringToHash("Fall");

    [Header("Sağlık")]
    [SerializeField, Min(1)] int maxHealth = 100;
    [SerializeField] int startHealth = 100;
    [SerializeField] int healAmount = 25;
    [SerializeField, Min(0f)] float damageIFrames = 0.25f;
    [SerializeField, Min(0f)] float damageFlashTime = 0.12f;
    [SerializeField] Color damageFlashColor = new Color(1f, 0.2f, 0.2f, 1f);
    [SerializeField] float floatingTextDuration = 0.65f;

    Collider2D col;
    float coyoteCounter;
    float jumpBufferCounter;
    bool physicsGrounded;
    bool jumpConsumed;
    bool facingRight = true;
    bool dead;
    Coroutine deathRoutine;

    int currentHealth;
    float iFrameTimer;
    Color defaultSpriteColor;
    Coroutine flashRoutine;
    Vector3 spawnPosition;

    public GameObject gameOverPanel;


    public bool IsDead => dead;

    // Bileşenleri başlatır, Rigidbody ayarlarını yapar, canı ve spawn pozisyonunu ayarlar



    private void Start()
    {
       
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

    }

    void Awake()
    {
        rb = rb ? rb : GetComponent<Rigidbody2D>();
        animator = animator ? animator : GetComponent<Animator>();
        spriteRenderer = spriteRenderer ? spriteRenderer : GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        currentHealth = Mathf.Clamp(startHealth, 0, maxHealth);
        if (spriteRenderer != null)
            defaultSpriteColor = spriteRenderer.color;
        spawnPosition = transform.position;
    }

    // Her karede iFrame sayacını düşürür, H tuşuyla iyileşmeyi ve zıplama buffer'ını yönetir
    void Update()
    {
        if (iFrameTimer > 0f)
            iFrameTimer -= Time.deltaTime;
        var kb = Keyboard.current;

        if (dead && kb != null && kb.spaceKey.wasPressedThisFrame)
        {
            RestartGame();
            return;
        }

        if (dead)
            return;

        if (kb == null)
            return;

        if (kb.hKey.wasPressedThisFrame)
            Heal(healAmount);

        if (kb.spaceKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame)
            jumpBufferCounter = jumpBuffer;
        else
            jumpBufferCounter -= Time.deltaTime;
    }

    // Fizik adımlarını yönetir: zemin kontrolü, hareket, zıplama, yerçekimi ve yön
    void FixedUpdate()
    {

        if (dead)
            return;

        physicsGrounded = CheckGrounded();
        if (physicsGrounded)
        {
            coyoteCounter = coyoteTime;
            jumpConsumed = false;
        }
        else
            coyoteCounter -= Time.fixedDeltaTime;

        float h = ReadHorizontal();
        MoveHorizontal(h);
        ApplyJump();
        ApplyFallGravity();
        UpdateFacing(h);
    }

    // Her kare sonunda doğru animasyon state'ine geçiş yapar
    void LateUpdate()
    {
        if (dead || animator == null)
            return;

        if (AnimatorBlocksLocomotion())
            return;

        int target = ResolveLocomotionStateHash();
        if (!AnimatorIsInOrEnteringState(target))
            animator.CrossFade(target, locomotionCrossFade, 0);
    }

    // Hit veya ölüm animasyonu oynarken hareket animasyonlarını engeller
    bool AnimatorBlocksLocomotion()
    {
        if (animator.IsInTransition(0))
        {
            var next = animator.GetNextAnimatorStateInfo(0);
            if (IsHitOrDeadStateInfo(next))
                return true;
        }

        var cur = animator.GetCurrentAnimatorStateInfo(0);
        return IsHitOrDeadStateInfo(cur);
    }

    // Verilen animasyon state'inin Hit veya ölüm state'i olup olmadığını kontrol eder
    static bool IsHitOrDeadStateInfo(AnimatorStateInfo info)
    {
        return info.IsName("Hit") || info.IsName("DeadHit") || info.IsName("DeadGround");
    }

    // Animator'ın şu an verilen state'de olup olmadığını kontrol eder
    bool AnimatorIsInOrEnteringState(int stateHash)
    {
        var cur = animator.GetCurrentAnimatorStateInfo(0);
        if (cur.shortNameHash == stateHash)
            return true;

        if (animator.IsInTransition(0))
        {
            var next = animator.GetNextAnimatorStateInfo(0);
            if (next.shortNameHash == stateHash)
                return true;
        }

        return false;
    }

    // Hıza ve zemin durumuna göre hangi animasyonun oynanacağını belirler
    int ResolveLocomotionStateHash()
    {
        float vy = rb.linearVelocity.y;
        bool landContact = physicsGrounded && vy <= 0.15f;

        if (!landContact)
            return vy > 0.05f ? JumpStateHash : FallStateHash;

        return Mathf.Abs(rb.linearVelocity.x) > 0.05f ? RunStateHash : IdleStateHash;
    }

    // Klavyeden yatay hareket girdisini okur ve -1 ile 1 arasında döndürür
    float ReadHorizontal()
    {
        var kb = Keyboard.current;
        if (kb == null)
            return 0f;

        float v = 0f;
        if (kb.leftArrowKey.isPressed || kb.aKey.isPressed)
            v -= 1f;
        if (kb.rightArrowKey.isPressed || kb.dKey.isPressed)
            v += 1f;
        return Mathf.Clamp(v, -1f, 1f);
    }

    // Ayakların altından Raycast atarak zeminde olup olmadığını kontrol eder
    bool CheckGrounded()
    {
        if (col == null)
            return false;

        Bounds b = col.bounds;
        var origin = new Vector2(b.center.x + groundCheckOffset.x, b.min.y + groundCheckOffset.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundMask);
        return hit.collider != null;
    }

    // Yatay hareketi ivme ve sürtünmeyle uygular
    void MoveHorizontal(float h)
    {
        float accel = physicsGrounded ? acceleration : airAcceleration;
        float target = h * moveSpeed;
        float vx = rb.linearVelocity.x;
        float dt = Time.fixedDeltaTime;
        float newVx = Mathf.MoveTowards(vx, target, accel * dt);

        if (physicsGrounded && Mathf.Abs(h) < 0.01f)
            newVx = Mathf.MoveTowards(vx, 0f, groundFriction * dt);

        rb.linearVelocity = new Vector2(newVx, rb.linearVelocity.y);
    }

    // Zıplama buffer ve coyote time kontrolüyle zıplamayı uygular
    void ApplyJump()
    {
        bool wantJump = jumpBufferCounter > 0f;
        if (!wantJump)
            return;

        if (coyoteCounter <= 0f)
            return;
        if (jumpConsumed)
            return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        coyoteCounter = 0f;
        jumpBufferCounter = 0f;
        jumpConsumed = true;
    }

    // Düşerken yerçekimini artırarak daha doğal bir zıplama hissi sağlar
    void ApplyFallGravity()
    {
        if (physicsGrounded || rb.linearVelocity.y >= 0f)
            rb.gravityScale = 1f;
        else
            rb.gravityScale = fallGravityMultiplier;
    }

    // Hareket yönüne göre sprite'ı sağa veya sola çevirir
    void UpdateFacing(float h)
    {
        if (spriteRenderer == null || Mathf.Abs(h) < 0.01f)
            return;

        facingRight = h > 0f;
        spriteRenderer.flipX = !facingRight;
    }

    // Hasar alma animasyonunu tetikler
    public void TriggerHit()
    {
        if (dead || animator == null)
            return;
        animator.CrossFade("Hit", 0.05f, 0);
    }

    // Oyuncuyu öldürür ve ölüm coroutine'ini başlatır
    public void Die()
    {
        if (dead)
            return;

        dead = true;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Debug.Log("Player Died!");
        Time.timeScale = 0f;
    }


    // Ölüm animasyonlarını sırayla oynatır ve fizik simülasyonunu durdurur
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

        if (rb != null)
            rb.simulated = false;
    }

    // Oyuncuyu başlangıç pozisyonuna sıfırlar ve tüm değerleri yeniler
    public void Respawn()
    {
        if (deathRoutine != null)
        {
            StopCoroutine(deathRoutine);
            deathRoutine = null;
        }

        dead = false;
        iFrameTimer = 0f;
        currentHealth = Mathf.Clamp(startHealth, 1, maxHealth);
        transform.position = spawnPosition;
        jumpConsumed = false;
        coyoteCounter = 0f;
        jumpBufferCounter = 0f;

        if (spriteRenderer != null)
            spriteRenderer.color = defaultSpriteColor;

        if (col != null)
            col.enabled = true;

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        if (animator != null)
            animator.Play("Idle", 0, 0f);

        var bombDropper = GetComponent<BombDropper>();
        if (bombDropper != null)
            bombDropper.RefillBombs();
    }

    // Oyuncuya hasar verir, iFrame kontrolü yapar ve ölümü tetikler
    public void TakeDamage(int amount)
    {
        if (dead)
            return;
        if (amount <= 0)
            return;
        if (iFrameTimer > 0f)
            return;

        iFrameTimer = damageIFrames;
        currentHealth = Mathf.Max(0, currentHealth - amount);

        SpawnFloatingText("-" + amount, damageFlashColor, transform.position + Vector3.up * 0.6f, floatingTextDuration);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (spriteRenderer != null)
        {
            if (flashRoutine != null)
                StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(DamageFlashRoutine());
        }

        TriggerHit();
    }

    // Oyuncunun canını artırır ve üzerinde yeşil floating text gösterir
    public void Heal(int amount)
    {
        if (dead)
            return;
        if (amount <= 0)
            return;

        int before = currentHealth;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        int delta = currentHealth - before;
        if (delta <= 0)
            return;

        SpawnFloatingText("+" + delta, Color.green, transform.position + Vector3.up * 0.6f, floatingTextDuration);
    }

    // Maksimum can değerinin sıfırdan büyük olup olmadığını kontrol eder
    bool HasHealth()
    {
        return maxHealth > 0;
    }

    // Ekrana can barı ve ölüm/yeniden canlanma butonunu çizer
    void OnGUI()
    {
        if (!HasHealth())
            return;

        GUI.Label(new Rect(10, 10, 300, 20), $"HP: {currentHealth}/{maxHealth}");
        GUI.Label(new Rect(10, 30, 420, 20), $"Heal (H): +{healAmount}");
        //if (dead)
        //{
        //    GUI.Label(new Rect(10, 50, 420, 20), "Öldün");
        //    if (GUI.Button(new Rect(10, 75, 140, 30), "Yeniden Canlan"))
        //        Respawn();
        //}
    }

    // Hasar alınca sprite'ı kırmızıya çevirir ve sonra eski rengine döndürür
    IEnumerator DamageFlashRoutine()
    {
        if (spriteRenderer == null)
            yield break;

        float t = 0f;
        while (t < damageFlashTime)
        {
            t += Time.deltaTime;
            float lerp = Mathf.PingPong(t * 20f, 1f);
            spriteRenderer.color = Color.Lerp(defaultSpriteColor, damageFlashColor, lerp);
            yield return null;
        }

        spriteRenderer.color = defaultSpriteColor;
        flashRoutine = null;
    }

    // Oyuncunun üzerinde yükselen hasar/iyileşme yazısı oluşturur
    void SpawnFloatingText(string text, Color color, Vector3 pos, float duration)
    {
        var go = new GameObject("FloatingText");
        go.transform.position = pos;

        var tm = go.AddComponent<TextMesh>();
        tm.text = text;
        tm.color = color;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.fontSize = 64;
        tm.characterSize = 0.1f;

        StartCoroutine(FloatTextRoutine(tm, pos, duration));
    }

    // Floating text'i yukarı doğru hareket ettirir ve süre dolunca yok eder
    IEnumerator FloatTextRoutine(TextMesh tm, Vector3 startPos, float duration)
    {
        float t = 0f;
        while (t < duration && tm != null)
        {
            t += Time.deltaTime;
            float k = duration <= 0.0001f ? 1f : (t / duration);
            tm.transform.position = startPos + Vector3.up * (0.6f * k);
            yield return null;
        }

        if (tm != null)
            Destroy(tm.gameObject);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}