using UnityEngine;
using UnityEngine.InputSystem;

public class LadderClimb : MonoBehaviour
{
    [SerializeField] float climbSpeed = 5f;

    Rigidbody2D rb;
    Collider2D playerCol;
    bool onLadder;
    float lockedY;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCol = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        if (!onLadder) return;

        rb.gravityScale = 0f;

        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.wKey.isPressed || kb.upArrowKey.isPressed)
        {
            lockedY = rb.position.y;
            rb.linearVelocity = new Vector2(0f, climbSpeed);
            // Týrmanýrken zemin geçiþini aç
            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("Ground"), true);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = new Vector2(rb.position.x, lockedY);
            // Durduðunda zemin çarpýþmasýný geri aç
            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("Ground"), false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ladder")) return;
        onLadder = true;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        lockedY = rb.position.y;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Ladder")) return;
        onLadder = false;
        rb.gravityScale = 1f;
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Ground"), false);
    }
}