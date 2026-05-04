using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int mToplam = 10;
    private int collectedItems = 0;
    public TextMeshProUGUI malzemeler;
    public KapiCikisi kapiCikisi;
    public bool kapiAcik = false;

    void Awake()
    {
        instance = this;
    }

    public void CollectItem()
    {
        collectedItems++;
        if (malzemeler != null)
            malzemeler.text = collectedItems + "/" + mToplam;

        if (collectedItems >= mToplam)
        {
            if (kapiCikisi != null)
                kapiCikisi.OpenDoor();
            Debug.Log("Tüm malzemeler toplandı! Kapı açıldı.");
        }
    }
}