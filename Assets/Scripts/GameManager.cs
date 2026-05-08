using UnityEngine;
using UnityEngine.UI;
using TMPro;

// MonoBehaviour'dan türeyen oyun yöneticisi sınıfı
public class GameManager : MonoBehaviour
{
    // Singleton pattern - oyunun her yerinden erişim için tek örnek
    public static GameManager instance;

    // Toplanması gereken toplam malzeme sayısı (Inspector'dan ayarlanabilir)
    public int mToplam = 10;

    // Şu ana kadar toplanan malzeme sayısı (başlangıçta 0)
    private int collectedItems = 0;

    // Ekranda "toplanan/toplam" metnini gösterecek UI elementi
    public TextMeshProUGUI malzemeler;

    // Kapıyı kontrol eden script referansı
    public KapiCikisi kapiCikisi;

    // Kapının açık olup olmadığını tutan değişken
    public bool kapiAcik = false;

    // Oyun başladığında çalışır - instance'ı ayarlar
    void Awake()
    {
        instance = this;
    }

    // Bir malzeme toplandığında dışarıdan çağrılacak fonksiyon
    public void CollectItem()
    {
        // Toplanan malzeme sayısını 1 artır
        collectedItems++;

        // UI metni güncelle (örnek: "3/10")
        if (malzemeler != null)
            malzemeler.text = collectedItems + "/" + mToplam;

        // Tüm malzemeler toplandıysa kapıyı aç
        if (collectedItems >= mToplam)
        {
            // Kapı scripti bağlıysa kapıyı aç
            if (kapiCikisi != null)
                kapiCikisi.OpenDoor();

            // Konsola bilgi mesajı yaz
            Debug.Log("Tüm malzemeler toplandı! Kapı açıldı.");
        }
    }
}