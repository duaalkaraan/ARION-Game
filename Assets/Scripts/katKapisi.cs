using UnityEngine;
using TMPro;

// Kat kapısını yöneten sınıf - düşman öldürme ve anahtar şartı arar
public class katKapisi : MonoBehaviour
{
    [Header("Şartlar")]
    [SerializeField] int gerekliDusmanSayisi = 2;  // Kapı için öldürülmesi gereken düşman sayısı
    [SerializeField] int gerekliAnahtarID;          // Toplanması gereken anahtarın ID'si

    [Header("Teleport")]
    [SerializeField] Transform hedefKonum;          // Oyuncunun ışınlanacağı konum

    [Header("UI")]
    [SerializeField] GameObject ipucuKutu;           // İpucu metninin arka plan kutusu
    [SerializeField] TextMeshProUGUI ipucuYazisi;   // Ekranda gösterilecek ipucu metni


    [Header("Sprite")]
    [SerializeField] Sprite kapaliSprite;            // Kapı kapalıyken görsel
    [SerializeField] Sprite acikSprite;              // Kapı açıkken görsel

    bool kapıAcik = false;              // Kapı açık mı?
    bool yakinMi = false;               // Oyuncu kapıya yakın mı?
    int oldurulenDusmanSayisi = 0;      // Şimdiye kadar öldürülen düşman sayısı

    SpriteRenderer sr;                  // Kapının görsel bileşeni

    // Oyun başladığında çalışır
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // İpucu kutusunu başlangıçta gizle
        if (ipucuYazisi != null)
            ipucuYazisi.transform.parent.gameObject.SetActive(false);
    }

    // Düşman öldürüldüğünde dışarıdan çağrılır
    public void DusmanOlduruldu()
    {
        oldurulenDusmanSayisi++;   // Sayacı artır
        SartlariKontrolEt();       // Şartlar sağlandı mı kontrol et
    }

    // Kapının açılma şartlarını kontrol eder
    public void SartlariKontrolEt()
    {
        // Kapı zaten açıksa tekrar kontrol etme
        if (kapıAcik) return;

        // Yeterli düşman öldürüldü mü?
        bool dusmanlarOldu = oldurulenDusmanSayisi >= gerekliDusmanSayisi;

        // Gerekli anahtar toplandı mı? (KeyManager üzerinden kontrol)
        bool anahtarAlindi = KeyManager.Instance != null &&
                             KeyManager.Instance.IsKeyCollected(gerekliAnahtarID);

        // Her iki şart da sağlandıysa kapıyı aç
        if (dusmanlarOldu && anahtarAlindi)
        {
            kapıAcik = true;

            // Açık sprite varsa onu kullan, yoksa yeşile boya
            if (acikSprite != null)
                sr.sprite = acikSprite;
            else
                sr.color = new Color(0f, 1f, 0f, 0.8f);

            
        }
    }

    // Her frame çalışır
    void Update()
    {
        // Oyuncu yakında değilse hiçbir şey yapma
        if (!yakinMi) return;

        SartlariKontrolEt();

        // Space tuşuna basıldıysa
        if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (kapıAcik)
            {
                // Kapı açıksa oyuncuyu ışınla
                TeleportOyuncu();
            }
            else
            {
                // Kapı kapalıysa eksik şartları hesapla
                bool dusmanlarOldu = oldurulenDusmanSayisi >= gerekliDusmanSayisi;
                bool anahtarAlindi = KeyManager.Instance != null &&
                                     KeyManager.Instance.IsKeyCollected(gerekliAnahtarID);

                // Eksik şartları mesaj olarak göster
                string mesaj = "";
                if (!dusmanlarOldu)
                    mesaj += "Tüm düşmanları öldür! ";
                if (!anahtarAlindi)
                    mesaj += "Anahtar parçasını topla!";

                if (ipucuYazisi != null)
                    ipucuYazisi.text = mesaj;
            }
        }
    }

    // Oyuncuyu hedef konuma ışınlar
    void TeleportOyuncu()
    {
        // Hedef konum atanmamışsa uyar ve dur
        if (hedefKonum == null)
        {
            Debug.LogWarning("Hedef konum atanmadı! Inspector'dan hedefKonum'u ayarla.");
            return;
        }

        // "Player" tag'li nesneyi bul
        GameObject oyuncu = GameObject.FindGameObjectWithTag("Player");
        if (oyuncu != null)
        {
            // Işınlamadan önce oyuncunun hızını sıfırla (kaymaması için)
            Rigidbody2D rb = oyuncu.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            // Oyuncuyu hedef konuma taşı
            oyuncu.transform.position = hedefKonum.position;
        }

        // Kapıyı sıfırla - bir sonraki kat için hazırla
        kapıAcik = false;
        yakinMi = false;               // Trigger çalışmayabilir, elle sıfırla
        oldurulenDusmanSayisi = 0;     // Düşman sayacını sıfırla
        sr.color = Color.white;        // Rengi normale döndür

        // Kapalı sprite'a geri dön
        if (acikSprite != null && kapaliSprite != null)
            sr.sprite = kapaliSprite;

        //// İpucu kutusunu gizle
        //if (ipucuKutu != null)
        //    ipucuKutu.SetActive(false);
    }

    // Oyuncu kapıya girdiğinde çalışır
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            yakinMi = true;

            if (ipucuYazisi != null)
            {
                ipucuYazisi.transform.parent.gameObject.SetActive(true);

                if (kapıAcik)
                    ipucuYazisi.text = "Kapi acik! [Space] Diger kata gec";
                else
                {
                    bool dusmanlarOldu = oldurulenDusmanSayisi >= gerekliDusmanSayisi;
                    bool anahtarAlindi = KeyManager.Instance != null &&
                                         KeyManager.Instance.IsKeyCollected(gerekliAnahtarID);

                    string mesaj = "";
                    if (!dusmanlarOldu)
                        mesaj += "Tum dusmanlari oldur! ";
                    if (!anahtarAlindi)
                        mesaj += "Anahtar parcasini topla!";

                    ipucuYazisi.text = mesaj;
                }
            }
        }
    }

    // Oyuncu kapı bölgesinden çıktığında çalışır
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            yakinMi = false;

            // İpucu kutusunu gizle
            if (ipucuYazisi != null)
                ipucuYazisi.transform.parent.gameObject.SetActive(false);
        }
    }
}