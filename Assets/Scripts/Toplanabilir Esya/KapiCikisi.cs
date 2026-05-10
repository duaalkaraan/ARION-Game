using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// Kapı geçişini ve sahne yüklemeyi yöneten sınıf
public class KapiCikisi : MonoBehaviour
{
    // Kapının açık olup olmadığını takip eder
    private bool isAcik = false;

    // Inspector'dan ayarlanabilir değişkenler
    [SerializeField] string sonrakiSahneAdi = "Level2";  // Geçilecek sahnenin adı
    [SerializeField] float gecisGecikmesi = 1f;           // Geçişten önce bekleme süresi
    [SerializeField] Animator kapiAnimator;               // Kapı animasyonu

    // GameManager tarafından çağrılır - kapıyı açar
    public void OpenDoor()
    {
        // Kapı zaten açıksa tekrar açma
        if (GameManager.instance.kapiAcik) return;

        // GameManager'da kapıyı açık olarak işaretle
        GameManager.instance.kapiAcik = true;

        Debug.Log("Kapı Açıldı!");
        // İstersen buraya: kapiAnimator.SetTrigger("Ac"); ekleyebilirsin
    }

    // Oyuncu kapıya dokunduğunda otomatik çalışır
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Hangi nesnenin dokunduğunu ve kapı durumunu konsola yaz
        Debug.Log("Dokunan: " + other.name + " kapiAcik: " + GameManager.instance.kapiAcik);

        // Kapı açıksa VE dokunan nesne oyuncuysa sahneyi geç
        if (GameManager.instance.kapiAcik && other.CompareTag("Player"))
        {
            Debug.Log("Level geçiliyor...");

            // Gecikmeli sahne geçişini başlat
            StartCoroutine(SonrakiLeveleGec());
        }
    }

    // Gecikmeli sahne geçişi (Coroutine)
    private IEnumerator SonrakiLeveleGec()
    {
        // Belirtilen süre kadar bekle (örn: 1 saniye)
        yield return new WaitForSeconds(gecisGecikmesi);

        // Sonraki sahneyi yükle
        SceneManager.LoadScene(sonrakiSahneAdi);
    }
}