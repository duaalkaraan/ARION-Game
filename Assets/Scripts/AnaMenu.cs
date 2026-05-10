using UnityEngine;
using UnityEngine.SceneManagement;

public class AnaMenu : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject onayPaneli;
    public GameObject ayarlarPaneli;

    // --- YENÝ EKLENEN KISIM: ANA MENÜYE DÖNÜÞ ---
    public void AnaMenuyeDon()
    {
        // Ana menü sahnenin adý hiyerarþide neyse onu yaz (Genelde "AnaMenu")
        SceneManager.LoadScene("AnaMenu");
    }
    // ------------------------------------------

    public void OyunuBaslat()
    {
        SceneManager.LoadScene("level_1");
    }

    public void OnayPaneliniAc()
    {
        onayPaneli.SetActive(true);
        if (ayarlarPaneli != null) ayarlarPaneli.SetActive(false);
    }

    public void Vazgec()
    {
        onayPaneli.SetActive(false);
    }

    public void TamamenCik()
    {
        Debug.Log("Oyun kapatýlýyor...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void AyarlariAc()
    {
        if (ayarlarPaneli != null) ayarlarPaneli.SetActive(true);
    }

    public void AyarlariKapat()
    {
        if (ayarlarPaneli != null) ayarlarPaneli.SetActive(false);
    }
}