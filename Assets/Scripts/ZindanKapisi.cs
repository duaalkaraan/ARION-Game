using UnityEngine;
using TMPro;

// Zindan kapısını yöneten sınıf - tüm anahtarlar olmadan geçilmez
public class ZindanKapisi : MonoBehaviour
{
    // Ekranda gösterilecek ipucu metni
    [SerializeField] TextMeshProUGUI ipucuYazisi;

    // Oyuncu kapıya yakın mı?
    bool yakinMi = false;

    // Oyun başladığında ipucu kutusunu gizle
    void Start()
    {
        if (ipucuYazisi != null)
            ipucuYazisi.transform.parent.gameObject.SetActive(false); // İpucu kutusunun parent'ını gizle
    }

    // Her frame çalışır
    void Update()
    {
        // Oyuncu yakında değilse hiçbir şey yapma
        if (!yakinMi) return;

        // Space tuşuna basıldıysa
        if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // Tüm anahtarlar toplandıysa kapıyı aç
            if (KeyManager.Instance != null && KeyManager.Instance.AreAllKeysCollected())
            {
                // İpucu kutusunu gizle
                if (ipucuYazisi != null)
                    ipucuYazisi.transform.parent.gameObject.SetActive(false);

                // Kapı objesini sahneden sil (kapı açıldı)
                Destroy(gameObject);
            }
            else
            {
                // Anahtarlar eksikse uyarı mesajı göster
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

            // İpucu kutusunu göster ve mesajı ayarla
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

            // İpucu kutusunu gizle
            if (ipucuYazisi != null)
                ipucuYazisi.transform.parent.gameObject.SetActive(false);
        }
    }
}