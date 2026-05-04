using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class YuzmeKontrol : MonoBehaviour
{
    [Header("Yüzme")]
    [SerializeField] float yuzmeHizi = 4f;

    [Header("Oksijen")]
    [SerializeField] float maksOksijen = 5f;
    [SerializeField] Image oksijenBarUI;

    bool suda = false;
    float mevcutOksijen;
    Rigidbody2D rb;
    PlayerMovement2D oyuncu;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        oyuncu = GetComponent<PlayerMovement2D>();
        mevcutOksijen = maksOksijen;
        if (oksijenBarUI != null)
            oksijenBarUI.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!suda) return;

        mevcutOksijen -= Time.deltaTime;
        if (oksijenBarUI != null)
            oksijenBarUI.fillAmount = mevcutOksijen / maksOksijen;

        if (mevcutOksijen <= 0f)
        {
            oyuncu.Die();
            return;
        }

        var kb = Keyboard.current;
        if (kb == null) return;

        float yatay = 0f;
        float dikey = 0f;

        if (kb.leftArrowKey.isPressed || kb.aKey.isPressed) yatay = -1f;
        if (kb.rightArrowKey.isPressed || kb.dKey.isPressed) yatay = 1f;
        if (kb.upArrowKey.isPressed || kb.wKey.isPressed) dikey = 1f;
        if (kb.downArrowKey.isPressed || kb.sKey.isPressed) dikey = -1f;

        // Tuşa basılmıyorsa dur — platform üzerinde dursun
        if (yatay == 0f && dikey == 0f)
            rb.linearVelocity = new Vector2(0f, 0f);
        else
            rb.linearVelocity = new Vector2(yatay * yuzmeHizi, dikey * yuzmeHizi);
    }

    public void SuyaGir()
    {
        //Debug.LogAssertion("selam");
        suda = true;
        //rb.gravityScale = 1f;
        //rb.angularDamping = 0f;
        rb.linearVelocityY = -1f;
        mevcutOksijen = maksOksijen;
        if (oksijenBarUI != null)
            oksijenBarUI.gameObject.SetActive(true);
    }

    public void SudanCik()
    {
        suda = false;
        rb.gravityScale = 1f;
        if (oksijenBarUI != null)
            oksijenBarUI.gameObject.SetActive(false);
    }
}