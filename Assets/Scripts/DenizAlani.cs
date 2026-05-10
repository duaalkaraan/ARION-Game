using UnityEngine;

// Deniz/su alanýný temsil eden trigger bölgesi
public class DenizAlani : MonoBehaviour
{
    // Bir nesne su alanýna girdiðinde otomatik çalýþýr
    private void OnTriggerEnter2D(Collider2D diger)
    {
        // Giren nesne "Player" tag'ine sahip mi kontrol et
        if (diger.CompareTag("Player"))
        {
            // Konsola hangi nesnenin girdiðini yaz
            Debug.Log("bir nesne deniz alanýna girdi: " + diger.tag);

            // Oyuncunun YuzmeKontrol scriptini al
            YuzmeKontrol yuzme = diger.GetComponent<YuzmeKontrol>();

            // Script varsa SuyaGir fonksiyonunu çaðýr
            if (yuzme != null)
                yuzme.SuyaGir();
        }
    }

    // Bir nesne su alanýndan çýktýðýnda otomatik çalýþýr
    private void OnTriggerExit2D(Collider2D diger)
    {
        // Çýkan nesne "Player" tag'ine sahip mi kontrol et
        if (diger.CompareTag("Player"))
        {
            // Oyuncunun YuzmeKontrol scriptini al
            YuzmeKontrol yuzme = diger.GetComponent<YuzmeKontrol>();

            // Script varsa SudanCik fonksiyonunu çaðýr
            if (yuzme != null)
                yuzme.SudanCik();
        }
    }
}