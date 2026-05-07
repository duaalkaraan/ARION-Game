using UnityEngine;
using TMPro;

public class katKapisi : MonoBehaviour
{
    [Header("Şartlar")]
    [SerializeField] int gerekliDusmanSayisi = 2;
    [SerializeField] int gerekliAnahtarID;

    [Header("Teleport")]
    [SerializeField] Transform hedefKonum;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI ipucuYazisi;
    [SerializeField] GameObject ipucuKutu;

    [Header("Sprite")]
    [SerializeField] Sprite kapaliSprite;
    [SerializeField] Sprite acikSprite;

    bool kapıAcik = false;
    bool yakinMi = false;
    int oldurulenDusmanSayisi = 0;

    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (ipucuKutu != null)
            ipucuKutu.SetActive(false);
    }

    public void DusmanOlduruldu()
    {
        oldurulenDusmanSayisi++;
        SartlariKontrolEt();
    }

    void SartlariKontrolEt()
    {
        if (kapıAcik) return; // zaten açıksa tekrar kontrol etme

        bool dusmanlarOldu = oldurulenDusmanSayisi >= gerekliDusmanSayisi;
        bool anahtarAlindi = KeyManager.Instance != null &&
                             KeyManager.Instance.IsKeyCollected(gerekliAnahtarID);

        if (dusmanlarOldu && anahtarAlindi)
        {
            kapıAcik = true;

            if (acikSprite != null)
                sr.sprite = acikSprite;
            else
                sr.color = new Color(0f, 1f, 0f, 0.8f);

            if (yakinMi && ipucuYazisi != null)
                ipucuYazisi.text = "Kapı açık! [Space] Geç";

            Debug.Log("Kapı açık! Geçebilirsin.");
        }
    }

    void Update()
    {
        if (!yakinMi) return;

        SartlariKontrolEt();

        if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (kapıAcik)
            {
                TeleportOyuncu();
            }
            else
            {
                bool dusmanlarOldu = oldurulenDusmanSayisi >= gerekliDusmanSayisi;
                bool anahtarAlindi = KeyManager.Instance != null &&
                                     KeyManager.Instance.IsKeyCollected(gerekliAnahtarID);

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

    void TeleportOyuncu()
    {
        if (hedefKonum == null)
        {
            Debug.LogWarning("Hedef konum atanmadı! Inspector'dan hedefKonum'u ayarla.");
            return;
        }

        GameObject oyuncu = GameObject.FindGameObjectWithTag("Player");
        if (oyuncu != null)
        {
            Rigidbody2D rb = oyuncu.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            oyuncu.transform.position = hedefKonum.position;
        }

        // Kapıyı sıfırla
        kapıAcik = false;
        yakinMi = false;          // trigger exit tetiklenmeyebilir, elle sıfırla
        oldurulenDusmanSayisi = 0;
        sr.color = Color.white;

        if (acikSprite != null && kapaliSprite != null)
            sr.sprite = kapaliSprite;

        if (ipucuKutu != null)
            ipucuKutu.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            yakinMi = true;
            if (ipucuKutu != null)
                ipucuKutu.SetActive(true);
            if (ipucuYazisi != null)
                ipucuYazisi.text = kapıAcik ? "Kapı açık! [Space] Geç" : "[Space] Kapıyı Aç";
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            yakinMi = false;
            if (ipucuKutu != null)
                ipucuKutu.SetActive(false);
        }
    }
}