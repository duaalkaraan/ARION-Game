using UnityEngine;

// Sahnedeki toplanabilir malzeme objesi
public class Malzemeler : MonoBehaviour
{
    // Oyuncu malzemeye dokunduðunda otomatik çalýþýr
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Dokunan nesne oyuncu mu?
        if (other.CompareTag("Player"))
        {
            // GameManager varsa malzeme toplandýðýný bildir
            if (GameManager.instance != null)
                GameManager.instance.CollectItem();

            // Malzeme objesini sahneden sil (bir daha toplanamaz)
            Destroy(gameObject);
        }
    }
}