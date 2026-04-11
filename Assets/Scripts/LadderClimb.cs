using UnityEngine;
using UnityEngine.InputSystem;

public class LadderClimb : MonoBehaviour
{
    [SerializeField] float climbSpeed = 5f;

    Rigidbody2D rb;
    bool onLadder;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!onLadder)
        {
            rb.gravityScale = 1f;
            return;
        }

        var kb = Keyboard.current;
        if (kb == null) return;

        rb.gravityScale = 0f;

        float v = 0f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) v = 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) v = -1f;

        if (v != 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, v * climbSpeed);
        else
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        onLadder = true;
        rb.gravityScale = 0f;
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Ground"), true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        onLadder = false;
        rb.gravityScale = 1f;
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Ground"), false);
    }
}