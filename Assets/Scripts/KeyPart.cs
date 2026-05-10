using UnityEngine;

// Sahnedeki anahtar parçasý objesi - oyuncu dokunca toplanýr
public class KeyPart : MonoBehaviour
{
    // Bu anahtarýn ID'si - 0, 1 veya 2 olabilir (Inspector'dan ayarla)
    public int keyID;

    // Oyuncu anahtar objesine dokunduðunda otomatik çalýþýr
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Dokunan nesne oyuncu mu?
        if (other.CompareTag("Player"))
        {
            // KeyManager'a bu anahtarýn toplandýðýný bildir
            KeyManager.Instance.CollectKey(keyID);

            // Anahtar objesini sahneden sil (bir daha toplanamaz)
            Destroy(gameObject);
        }
    }
}