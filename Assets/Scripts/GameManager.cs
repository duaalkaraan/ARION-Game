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
        }
    }
}