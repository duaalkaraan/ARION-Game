using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }

    [Header("Anahtar UI Slotları")]
    public Image[] keyUISlots;        // 3 slot
    public Sprite[] keyPartSprites;   // 3 parça sprite (renkli)
    public Sprite emptySprite;        // boş/gri sprite

    [Header("Tamamlanma Efekti")]
    public Image anahtarTamEkrani;    // tüm anahtar resmi (büyük)
    public float parlameHizi = 1.5f;

    [Header("Zindan")]
    public GameObject zindanKapisi;

    private bool[] collectedKeys = new bool[3];

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Başlangıçta tüm slotlar gri
        for (int i = 0; i < keyUISlots.Length; i++)
        {
            if (emptySprite != null)
                keyUISlots[i].sprite = emptySprite;
            keyUISlots[i].color = new Color(0.4f, 0.4f, 0.4f, 1f); // gri
        }

        // Tamamlanma ekranı gizli
        if (anahtarTamEkrani != null)
        {
            anahtarTamEkrani.gameObject.SetActive(false);
            anahtarTamEkrani.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    public void CollectKey(int id)
    {
        if (id < 0 || id >= collectedKeys.Length) return;
        if (collectedKeys[id]) return; // zaten toplandı

        collectedKeys[id] = true;
        StartCoroutine(SlotDoldurEfekti(id));
        CheckAllKeysCollected();
    }

    // Slot dolunca küçük zıplama + renk efekti
    IEnumerator SlotDoldurEfekti(int id)
    {
        Image slot = keyUISlots[id];

        // Sprite'ı renkli yap
        if (keyPartSprites != null && keyPartSprites.Length > id)
            slot.sprite = keyPartSprites[id];
        slot.color = Color.white;

        // Zıplama efekti
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

        // Hepsi toplandı!
        StartCoroutine(AnahtarTamamlandiEfekti());
    }

    IEnumerator AnahtarTamamlandiEfekti()
    {
        yield return new WaitForSeconds(0.3f);

        // Büyük anahtar resmi belir
        if (anahtarTamEkrani != null)
        {
            anahtarTamEkrani.gameObject.SetActive(true);

            // Fade in
            float sure = 0f;
            while (sure < 0.5f)
            {
                sure += Time.deltaTime;
                anahtarTamEkrani.color = new Color(1f, 1f, 1f, sure / 0.5f);
                yield return null;
            }

            // Parlama efekti — 3 kez
            for (int i = 0; i < 3; i++)
            {
                sure = 0f;
                while (sure < 0.2f)
                {
                    sure += Time.deltaTime;
                    float parlaklik = Mathf.Lerp(1f, 2f, sure / 0.2f);
                    anahtarTamEkrani.color = new Color(parlaklik, parlaklik, parlaklik, 1f);
                    yield return null;
                }
                sure = 0f;
                while (sure < 0.2f)
                {
                    sure += Time.deltaTime;
                    float parlaklik = Mathf.Lerp(2f, 1f, sure / 0.2f);
                    anahtarTamEkrani.color = new Color(parlaklik, parlaklik, parlaklik, 1f);
                    yield return null;
                }
            }

            yield return new WaitForSeconds(1f);

            // Fade out
            sure = 0f;
            while (sure < 0.4f)
            {
                sure += Time.deltaTime;
                anahtarTamEkrani.color = new Color(1f, 1f, 1f, 1f - (sure / 0.4f));
                yield return null;
            }
            anahtarTamEkrani.gameObject.SetActive(false);
        }

        // Kapıyı aç
        if (zindanKapisi != null)
            Destroy(zindanKapisi);

        Debug.Log("Zindan kapısı açıldı!");
    }
}