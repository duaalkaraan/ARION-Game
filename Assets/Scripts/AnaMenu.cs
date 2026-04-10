using UnityEngine;
using UnityEngine.SceneManagement;

public class AnaMenu : MonoBehaviour
{
    // Maceraya Baþla butonu için
    public void OyunuBaslat()
    {
        // "OyunSahnesi" yazan yere yüklemek istediðin sahnenin adýný yaz
        SceneManager.LoadScene("SampleScene");
    }

    // Çýkýþ butonu için
    public void OyundanCik()
    {
        Debug.Log("Oyundan çýkýldý!");
        Application.Quit();
    }
}