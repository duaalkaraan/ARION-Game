using UnityEngine;
using TMPro;

// Zindan kapısını yöneten sınıf - tüm anahtarlar olmadan geçilmez
public class ZindanKapisi : MonoBehaviour
{
    // Ekranda gösterilecek ipucu metni
    [SerializeField] TextMeshProUGUI ipucuYazisi;

    // ✅ Rafi'nin SpriteRenderer'ı - Inspector'dan bağla
    [SerializeField] SpriteRenderer rafiSprite;

    // ✅ Mutlu Rafi görseli - Inspector'dan Assets'teki mutluRAFİ'yi sürükle
    [SerializeField] Sprite mutluRafiSprite;

    // Oyuncu kapıya yakın mı?
    bool yakinMi = false;

    // Oyun başladığında ipucu kutusunu gizle
    void Start()
    {
        if (ipucuYazisi != null)
            ipucuYazisi.transform.parent.gameObject.SetActive(false);
    }

    // Her frame çalışır
    void Update()
    {
        if (!yakinMi) return;

        if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (KeyManager.Instance != null && KeyManager.Instance.AreAllKeysCollected())
            {
                // İpucu kutusunu gizle
                if (ipucuYazisi != null)
                    ipucuYazisi.transform.parent.gameObject.SetActive(false);

                // ✅ Rafi'yi mutlu yap
                if (rafiSprite != null && mutluRafiSprite != null)
                    rafiSprite.sprite = mutluRafiSprite;

                // Kapıyı sil
                Destroy(gameObject);
            }
            else
            {
                if (ipucuYazisi != null)
                {
                    ipucuYazisi.transform.parent.gameObject.SetActive(true);
                    ipucuYazisi.text = "Önce anahtarı tamamla!";
                }
            }
        }
    }

    // Oyuncu kapıya yaklaştığında çalışır
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            yakinMi = true;

            if (ipucuYazisi != null)
            {
                ipucuYazisi.transform.parent.gameObject.SetActive(true);
                ipucuYazisi.text = "[Space] Kapiyi Ac";
            }
        }
    }

    // Oyuncu kapıdan uzaklaştığında çalışır
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            yakinMi = false;

            if (ipucuYazisi != null)
                ipucuYazisi.transform.parent.gameObject.SetActive(false);
        }
    }
}