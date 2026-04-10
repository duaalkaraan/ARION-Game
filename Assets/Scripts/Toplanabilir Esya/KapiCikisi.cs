using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class KapiCikisi : MonoBehaviour
{
    private bool isAcik = false;

    [SerializeField] string sonrakiSahneAdi = "Level2";
    [SerializeField] float gecisGecikmesi = 1f;
    [SerializeField] Animator kapiAnimator; // animasyon yoksa boş bırak, sorun olmaz

    public void OpenDoor()
    {
        if (isAcik) return;
        isAcik = true;

        // Animator varsa tetikle, yoksa geç
        if (kapiAnimator != null)
            kapiAnimator.SetTrigger("Ac");

        Debug.Log("Kapı Açıldı!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if (isAcik && other.CompareTag("Player"))
        //{
        //    Debug.Log("Level geçiliyor...");
        //    StartCoroutine(SonrakiLeveleGec());
        //}
        Debug.Log("Deneem");
        StartCoroutine(SonrakiLeveleGec());
    }

    private IEnumerator SonrakiLeveleGec()
    {
        yield return new WaitForSeconds(gecisGecikmesi);
        SceneManager.LoadScene(sonrakiSahneAdi);
    }
}