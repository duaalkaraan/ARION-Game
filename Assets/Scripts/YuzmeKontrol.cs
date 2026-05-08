using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// Oyuncunun yüzme kontrolünü yöneten sınıf
public class YuzmeKontrol : MonoBehaviour
{
    // Inspector'da "Yüzme" başlığı altında görünür
    [Header("Yüzme")]
    [SerializeField] float yuzmeHizi = 4f;

    // Inspector'da "Oksijen" başlığı altında görünür
    [Header("Oksijen")]
    [SerializeField] float maksOksijen = 5f;   // Maksimum oksijen miktarı
    [SerializeField] Image oksijentBarUI;       // Oksijen çubuğu UI görseli

    bool suda = false;              // Oyuncu suda mı?
    float mevcutOksijen;            // Şu anki oksijen miktarı
    Rigidbody2D rb;                 // Fizik bileşeni
    PlayerMovement2D oyuncu;        // Oyuncu hareket scripti

    // Oyun başladığında çalışır
    void Awake()
    {
        // Gerekli bileşenleri al
        rb = GetComponent<Rigidbody2D>();
        oyuncu = GetComponent<PlayerMovement2D>();

        // Oksijen barını başlangıçta dolu ayarla
        mevcutOksijen = maksOksijen;

        // Oksijen UI'ını gizle (suda değiliz)
        if (oksijentBarUI != null)
            oksijentBarUI.gameObject.SetActive(false);
    }

    // Her frame çalışır
    void Update()
    {
        // Suda değilse hiçbir şey yapma
        if (!suda) return;

        // Oksijeni zamanla azalt
        mevcutOksijen -= Time.deltaTime;

        // Oksijen UI çubuğunu güncelle (0 ile 1 arası oran)
        if (oksijentBarUI != null)
            oksijentBarUI.fillAmount = mevcutOksijen / maksOksijen;

        // Oksijen bittiyse oyuncuyu öldür
        if (mevcutOksijen <= 0f)
        {
            oyuncu.Die();
            return;
        }

        // Klavye girdisini al
        var kb = Keyboard.current;
        if (kb == null) return;

        // Yatay ve dikey hareket değerleri
        float yatay = 0f;
        float dikey = 0f;

        // Sol/Sağ ok tuşları veya A/D tuşlarıyla yatay hareket
        if (kb.leftArrowKey.isPressed || kb.aKey.isPressed) yatay = -1f;
        if (kb.rightArrowKey.isPressed || kb.dKey.isPressed) yatay = 1f;

        // Yukarı/Aşağı ok tuşları veya W/S tuşlarıyla dikey hareket
        if (kb.upArrowKey.isPressed || kb.wKey.isPressed) dikey = 1f;
        if (kb.downArrowKey.isPressed || kb.sKey.isPressed) dikey = -1f;

        // Eğer platform üzerindeyse hareket etme (su yüzeyinde dur)
        // Aksi hâlde tüm yönlerde serbestçe yüz
        if (yatay == 0f && dikey == 0f)
            rb.linearVelocity = new Vector2(0f, 0f);   // Dur
        else
            rb.linearVelocity = new Vector2(yatay * yuzmeHizi, dikey * yuzmeHizi); // Yüz
    }

    // Suya girince çağrılır (dışarıdan tetiklenir)
    public void SuyaGir()
    {
        suda = true;                        // Suda modunu aç
        rb.gravityScale = 1f;               // Yerçekimini sıfırla (suda yüzer)
        mevcutOksijen = maksOksijen;        // Oksijeni doldur

        // Oksijen UI'ını göster
        if (oksijentBarUI != null)
            oksijentBarUI.gameObject.SetActive(true);
    }

    // Suyu terk edince çağrılır (dışarıdan tetiklenir)
    public void SudanCik()
    {
        suda = false;                       // Suda modunu kapat
        rb.gravityScale = 1f;              // Yerçekimini normale döndür

        // Oksijen UI'ını gizle
        if (oksijentBarUI != null)
            oksijentBarUI.gameObject.SetActive(false);
    }
}