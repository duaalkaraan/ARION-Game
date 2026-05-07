using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }

    [Header("Anahtar UI Slotları")]
    public Image[] keyUISlots;
    public Sprite[] keyPartSprites;
    public Sprite emptySprite;

    [Header("Tamamlanma Efekti")]
    public Image anahtarTamEkrani;
    public float parlameHizi = 1.5f;

    [Header("Zindan")]
    public GameObject zindanKapisi;
    public GameObject anahtarKutu;

    private bool[] collectedKeys = new bool[3];

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < keyUISlots.Length; i++)
        {
            if (emptySprite != null)
                keyUISlots[i].sprite = emptySprite;
            keyUISlots[i].color = new Color(0.4f, 0.4f, 0.4f, 1f);
        }

        if (anahtarTamEkrani != null)
        {
            anahtarTamEkrani.gameObject.SetActive(false);
           // anahtarTamEkrani.color = new Color(1f, 1f, 1f, 0f);
        }

        if (anahtarKutu != null)
            anahtarKutu.SetActive(false);
    }

    public void CollectKey(int id)
    {
        if (id < 0 || id >= collectedKeys.Length) return;
        if (collectedKeys[id]) return;

        collectedKeys[id] = true;
        StartCoroutine(SlotDoldurEfekti(id));
        CheckAllKeysCollected();
    }

    IEnumerator SlotDoldurEfekti(int id)
    {
        Image slot = keyUISlots[id];

        if (keyPartSprites != null && keyPartSprites.Length > id)
            slot.sprite = keyPartSprites[id];
        slot.color = Color.white;

        Vector3 normalBoyut = slot.transform.localScale;
        Vector3 buyukBoyut = normalBoyut * 1.4f;

        float sure = 0f;
        while (sure < 0.15f)
        {
            sure += Time.deltaTime;
            slot.transform.localScale = Vector3.Lerp(normalBoyut, buyukBoyut, sure / 0.15f);
            yield return null;
        }
        sure = 0f;
        while (sure < 0.15f)
        {
            sure += Time.deltaTime;
            slot.transform.localScale = Vector3.Lerp(buyukBoyut, normalBoyut, sure / 0.15f);
            yield return null;
        }
        slot.transform.localScale = normalBoyut;
    }

    void CheckAllKeysCollected()
    {
        foreach (bool key in collectedKeys)
            if (!key) return;

        StartCoroutine(AnahtarTamamlandiEfekti());
    }

    IEnumerator AnahtarTamamlandiEfekti()
    {
        yield return new WaitForSeconds(0.3f);

        // Kutu ve anahtar resmi aynı anda göster
        if (anahtarTamEkrani != null)
            anahtarTamEkrani.gameObject.SetActive(true);

        if (anahtarKutu != null)
            anahtarKutu.SetActive(true);

        yield return new WaitForSeconds(2f);

        if (anahtarTamEkrani != null)
            anahtarTamEkrani.gameObject.SetActive(false);

        if (anahtarKutu != null)
            anahtarKutu.SetActive(false);

        // Kapıyı aç
        if (zindanKapisi != null)

        Debug.Log("Zindan kapısı açıldı!");
    }
    public bool AreAllKeysCollected()
    {
        foreach (bool key in collectedKeys)
            if (!key) return false;
        return true;
    }
}