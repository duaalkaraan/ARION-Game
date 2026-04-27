using UnityEngine;
using UnityEngine.SceneManagement;

public class AnaMenu : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject onayPaneli;    // "Rafi esir" yazan çýkýþ onay paneli
    public GameObject ayarlarPaneli; // Eðer yaptýysan ayarlar paneli

    // 1. MACERAYA BAÞLA BUTONU ÝÇÝN
    public void OyunuBaslat()
    {
        // "SampleScene" yazan yere kendi oyun sahnenin adýný týrnak içinde yaz!
        SceneManager.LoadScene("Level1");
    }

    // 2. VEDA ET BUTONU ÝÇÝN (Paneli açar)
    public void OnayPaneliniAc()
    {
        onayPaneli.SetActive(true);
        if (ayarlarPaneli != null) ayarlarPaneli.SetActive(false); // Ayarlar açýksa kapatýr
    }

    // 3. HAYIR / VAZGEÇ BUTONU ÝÇÝN (Paneli kapatýr)
    public void Vazgec()
    {
        onayPaneli.SetActive(false);
    }

    // 4. EVET / ÇIKIÞ BUTONU ÝÇÝN (Oyunu kapatýr)
    public void TamamenCik()
    {
        Debug.Log("Oyun kapatýlýyor... (Bu yazý Console'da çýkýyorsa kod çalýþýyor demektir)");

        // Gerçek oyun dosyasýnda (Build) kapatýr
        Application.Quit();

        // Unity Editor içindeyken Play modunu durdurur (Test etmek için)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // 5. AYARLAR BUTONU ÝÇÝN (Opsiyonel)
    public void AyarlariAc()
    {
        ayarlarPaneli.SetActive(true);
    }

    public void AyarlariKapat()
    {
        ayarlarPaneli.SetActive(false);
    }
}