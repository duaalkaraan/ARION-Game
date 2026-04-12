using UnityEngine;

public class DenizAlani : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D diger)
    {
        YuzmeKontrol yuzme = diger.GetComponent<YuzmeKontrol>();
        if (yuzme != null)
            yuzme.SuyaGir();
    }

    private void OnTriggerExit2D(Collider2D diger)
    {
        YuzmeKontrol yuzme = diger.GetComponent<YuzmeKontrol>();
        if (yuzme != null)
            yuzme.SudanCik();
    }
}