using UnityEngine;
#if UNITY_EDITOR
using UnityEditor; // Sadece Unity Editor'da çalýþýr, oyunda deðil
#endif

[DisallowMultipleComponent] // Ayný objeye 2 kez eklenemez
public sealed class BombBarDisplay : MonoBehaviour // sealed = baþka class miras alamaz
{
    [SerializeField] BombDropper dropper;           // Bomba býrakma scripti (þarj verisini buradan alýr)
    [SerializeField] SpriteRenderer barRenderer;    // Þarj çubuðunun görseli
    [SerializeField] Vector3 worldOffset = new Vector3(0f, 1.25f, 0f); // Çubuðun objenin üzerindeki konumu
    [SerializeField] Sprite[] chargeSprites;        // Þarj seviyelerine göre sprite dizisi (11 adet)

    // Oyun baþlamadan önce çalýþýr
    void Awake()
    {
        // Dropper atanmamýþsa ayný objeden almayý dene
        dropper = dropper ? dropper : GetComponent<BombDropper>();

        // SpriteRenderer yoksa otomatik oluþtur
        EnsureRenderer();

        // Baþlangýçta çubuðu tam dolu göster
        RefreshSprite(1f);
    }

    // Her frame en son çalýþýr (kamera ve pozisyon güncellemelerinden sonra)
    void LateUpdate()
    {
        if (barRenderer == null || dropper == null)
            return;

        // Çubuðu her frame objenin biraz üstünde tut
        barRenderer.transform.position = transform.position + worldOffset;

        // Dropper'dan þarj oranýný al ve görseli güncelle (0.0 - 1.0 arasý)
        RefreshSprite(dropper.NormalizedCharge);
    }

    // Þarj çubuðu için SpriteRenderer oluþturur (yoksa)
    void EnsureRenderer()
    {
        // Zaten varsa iþlem yapma
        if (barRenderer != null)
            return;

        // "BombBar" adlý child objeyi ara
        var child = transform.Find("BombBar");

        if (child == null)
        {
            // Yoksa yeni bir child obje oluþtur
            var go = new GameObject("BombBar");
            go.transform.SetParent(transform);
            go.transform.localPosition = worldOffset;
            go.transform.localScale = Vector3.one;
            child = go.transform;
        }

        // SpriteRenderer bileþenini al, yoksa ekle
        barRenderer = child.GetComponent<SpriteRenderer>();
        if (barRenderer == null)
            barRenderer = child.gameObject.AddComponent<SpriteRenderer>();

        // Diðer spritelerin üstünde görünsün
        barRenderer.sortingOrder = 300;
    }

    // Þarj oranýna göre doðru sprite'ý seçer ve gösterir
    void RefreshSprite(float normalized)
    {
        if (barRenderer == null || chargeSprites == null || chargeSprites.Length == 0)
            return;

        int maxIndex = chargeSprites.Length - 1;

        // 0.0-1.0 oranýný sprite dizisi index'ine çevir
        // Örnek: 11 sprite varsa 0.5 þarj ? 5. sprite
        int idx = Mathf.Clamp(Mathf.RoundToInt(normalized * maxIndex), 0, maxIndex);

        barRenderer.sprite = chargeSprites[idx];
    }

    // Inspector'da bir deðer deðiþtiðinde Editor'da çalýþýr
    void OnValidate()
    {
        dropper = dropper ? dropper : GetComponent<BombDropper>();
        EnsureRenderer();
        TryAutoAssignSprites(); // Sprite'larý otomatik atamayý dene
    }

    // Sprite dizisi boþsa Assets klasöründen otomatik yükler (sadece Editor'da)
    void TryAutoAssignSprites()
    {
        // Zaten sprite varsa iþlem yapma
        if (chargeSprites != null && chargeSprites.Length > 0)
            return;

#if UNITY_EDITOR
        // 1.png'den 11.png'ye kadar olan sprite'larý Assets'ten yükle
        var sprites = new Sprite[11];
        for (int i = 1; i <= 11; i++)
        {
            string path = $"Assets/Sprites/7-Objects/3-Bomb Bar/1-Charging Bar/{i}.png";
            sprites[i - 1] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        chargeSprites = sprites;
#endif
    }
}