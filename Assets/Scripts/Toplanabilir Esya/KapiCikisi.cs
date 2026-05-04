using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class KapiCikisi : MonoBehaviour
{
    private bool isAcik = false;

    [SerializeField] string sonrakiSahneAdi = "Level2";
    [SerializeField] float gecisGecikmesi = 1f;
    [SerializeField] Animator kapiAnimator;

    public void OpenDoor()
    {
        if (GameManager.instance.kapiAcik) return;
        GameManager.instance.kapiAcik = true;
        Debug.Log("Kapı Açıldı!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Dokunan: " + other.name + " kapiAcik: " + GameManager.instance.kapiAcik);

        if (GameManager.instance.kapiAcik && other.CompareTag("Player"))
        {
            Debug.Log("Level geçiliyor...");
            StartCoroutine(SonrakiLeveleGec());
        }
    }

    private IEnumerator SonrakiLeveleGec()
    {
        yield return new WaitForSeconds(gecisGecikmesi);
        SceneManager.LoadScene(sonrakiSahneAdi);
    }
}