using UnityEngine;
using UnityEngine.InputSystem;

public class LadderClimb : MonoBehaviour
{
    // Týrmanma hýzý, Inspector'dan ayarlanabilir
    [SerializeField] float climbSpeed = 5f;

    // Karakterin fizik bileþeni
    Rigidbody2D rb;

    // Karakterin collider bileþeni
    Collider2D playerCol;

    // Karakterin merdivende olup olmadýðýný tutar
    bool onLadder;

    // Karakterin durduðu Y pozisyonunu saklar, kaymayý önler
    float lockedY;

    // Oyun baþlayýnca bileþenleri bulur ve atar
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCol = GetComponent<Collider2D>();
    }

    // Her fizik adýmýnda çalýþýr, týrmanma ve sabitleme iþlemlerini yönetir
    void FixedUpdate()
    {
        // Merdivende deðilse hiçbir þey yapma
        if (!onLadder) return;

        // Merdivende ise yerçekimini kapat
        rb.gravityScale = 0f;

        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.wKey.isPressed || kb.upArrowKey.isPressed)
        {
            // Yukarý týrmanýrken Y pozisyonunu kaydet
            lockedY = rb.position.y;

            // Karakteri yukarý hareket ettir
            rb.linearVelocity = new Vector2(0f, climbSpeed);

            // Týrmanýrken üst zemine geçebilmek için çarpýþmayý kapat
            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("Ground"), true);
        }
        else
        {
            // Tuþa basýlmýyorsa hýzý sýfýrla
            rb.linearVelocity = Vector2.zero;

            // Karakteri kaydettiði Y pozisyonuna sabitle, düþmesin
            rb.position = new Vector2(rb.position.x, lockedY);

            // Durduðunda zemin çarpýþmasýný geri aç
            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("Ground"), false);
        }
    }

    // Karakter merdivene girince çalýþýr
    void OnTriggerEnter2D(Collider2D other)
    {
        // Sadece Ladder tag'li objelere tepki ver
        if (!other.CompareTag("Ladder")) return;

        // Merdiven modunu aç
        onLadder = true;

        // Yerçekimini kapat
        rb.gravityScale = 0f;

        // Hýzý sýfýrla, giriþ noktasýnda sabit dur
        rb.linearVelocity = Vector2.zero;

        // Giriþ noktasýný kaydet
        lockedY = rb.position.y;
    }

    // Karakter merdivenden çýkýnca çalýþýr
    void OnTriggerExit2D(Collider2D other)
    {
        // Sadece Ladder tag'li objelere tepki ver
        if (!other.CompareTag("Ladder")) return;

        // Merdiven modunu kapat
        onLadder = false;

        // Yerçekimini geri aç
        rb.gravityScale = 1f;

        // Zemin çarpýþmasýný geri aç
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Ground"), false);
    }
}