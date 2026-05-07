using UnityEngine;

public class DenizAlani : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D diger)
    {
        if (diger.CompareTag("Player"))
        {
            Debug.Log("bir nesne deniz alanýna girdi: " + diger.tag);
            YuzmeKontrol yuzme = diger.GetComponent<YuzmeKontrol>();
            if (yuzme != null)
                yuzme.SuyaGir();
        }
    }

    private void OnTriggerExit2D(Collider2D diger)
    {
        if (diger.CompareTag("Player"))
        {
            YuzmeKontrol yuzme = diger.GetComponent<YuzmeKontrol>();
            if (yuzme != null)
                yuzme.SudanCik();
        }
    }
}