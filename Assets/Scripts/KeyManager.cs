using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Anahtar parçalarını yöneten Singleton sınıf
public class KeyManager : MonoBehaviour
{
    // Singleton - her yerden KeyManager.Instance ile erişim
    public static KeyManager Instance { get; private set; }

    [Header("Anahtar UI Slotları")]
    public Image[] keyUISlots;       // Ekrandaki 3 anahtar slot görseli
    public Sprite[] keyPartSprites;  // Her anahtarın dolu sprite'ı
    public Sprite emptySprite;       // Boş slot görseli

    [Header("Tamamlanma Efekti")]
    public Image anahtarTamEkrani;   // Tüm anahtarlar toplandığında gösterilecek ekran
    public float parlameHizi = 1.5f; // Efekt hızı

    [Header("Zindan")]
    public GameObject zindanKapisi;  // Açılacak zindan kapısı
    public GameObject anahtarKutu;   // Tamamlanma kutucuğu UI

    // Her anahtarın toplanıp toplanmadığını tutan dizi (3 anahtar)
    private bool[] collectedKeys = new bool[3];

    // Oyun başlamadan önce çalışır - Singleton kurulumu
    void Awake()
    {
        // Sahnede zaten bir KeyManager varsa bu objeyi yok et
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Oyun başladığında çalışır - UI'ı sıfırla
    void Start()
    {
        // Tüm slotları boş görsel ve gri renk ile başlat
        for (int i = 0; i < keyUISlots.Length; i++)
        {
            if (emptySprite != null)
                keyUISlots[i].sprite = emptySprite;
            keyUISlots[i].color = new Color(0.4f, 0.4f, 0.4f, 1f); // Gri
        }

        // Tamamlanma ekranını gizle
        if (anahtarTamEkrani != null)
            anahtarTamEkrani.gameObject.SetActive(false);

        // Anahtar kutusunu gizle
        if (anahtarKutu != null)
            anahtarKutu.SetActive(false);
    }

    // Anahtar toplandığında dışarıdan çağrılır
    public void CollectKey(int id)
    {
        // Geçersiz ID ise işlem yapma
        if (id < 0 || id >= collectedKeys.Length) return;

        // Zaten toplandıysa tekrar toplama
        if (collectedKeys[id]) return;

        // Anahtarı toplandı olarak işaretle
        collectedKeys[id] = true;

        // Slot dolma animasyonunu başlat
        StartCoroutine(SlotDoldurEfekti(id));

        // Tüm anahtarlar toplandı mı kontrol et
        CheckAllKeysCollected();
    }

    // Anahtar toplandığında slot büyüyüp küçülür (zıplama efekti)
    IEnumerator SlotDoldurEfekti(int id)
    {
        Image slot = keyUISlots[id];

        // Slota anahtar görselini ata ve beyaz yap
        if (keyPartSprites != null && keyPartSprites.Length > id)
            slot.sprite = keyPartSprites[id];
        slot.color = Color.white;

        Vector3 normalBoyut = slot.transform.localScale;
        Vector3 buyukBoyut = normalBoyut * 1.4f; // %140 büyüt

        // 0.15 saniyede büyüt
        float sure = 0f;
        while (sure < 0.15f)
        {
            sure += Time.deltaTime;
            slot.transform.localScale = Vector3.Lerp(normalBoyut, buyukBoyut, sure / 0.15f);
            yield return null;
        }

        // 0.15 saniyede normale döndür
        sure = 0f;
        while (sure < 0.15f)
        {
            sure += Time.deltaTime;
            slot.transform.localScale = Vector3.Lerp(buyukBoyut, normalBoyut, sure / 0.15f);
            yield return null;
        }

        // Tam normale sabitle
        slot.transform.localScale = normalBoyut;
    }

    // Tüm anahtarlar toplandı mı kontrol eder
    void CheckAllKeysCollected()
    {
        // Herhangi biri toplanmadıysa çık
        foreach (bool key in collectedKeys)
            if (!key) return;

        // Hepsi toplandıysa tamamlanma efektini başlat
        StartCoroutine(AnahtarTamamlandiEfekti());
    }

    // Tüm anahtarlar toplandığında çalışan efekt dizisi
    IEnumerator AnahtarTamamlandiEfekti()
    {
        yield return new WaitForSeconds(0.3f); // Kısa bekleme

        // Tamamlanma ekranını ve kutuyu göster
        if (anahtarTamEkrani != null)
            anahtarTamEkrani.gameObject.SetActive(true);
        if (anahtarKutu != null)
            anahtarKutu.SetActive(true);

        yield return new WaitForSeconds(2f); // 2 saniye göster

        // Ekranı ve kutuyu gizle
        if (anahtarTamEkrani != null)
            anahtarTamEkrani.gameObject.SetActive(false);
        if (anahtarKutu != null)
            anahtarKutu.SetActive(false);

        // Zindan kapısını aç
        if (zindanKapisi != null)
            Debug.Log("Zindan kapısı açıldı!");
        // İstersen buraya: zindanKapisi.SetActive(false); ekleyebilirsin
    }

    // Tüm anahtarlar toplandı mı? (dışarıdan sorgulanabilir)
    public bool AreAllKeysCollected()
    {
        foreach (bool key in collectedKeys)
            if (!key) return false;
        return true;
    }

    // Belirli bir anahtar toplandı mı? (ID'ye göre sorgulama)
    public bool IsKeyCollected(int id)
    {
        if (id < 0 || id >= collectedKeys.Length) return false;
        return collectedKeys[id];
    }
}