using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public sealed class BombBarDisplay : MonoBehaviour
{
    [SerializeField] BombDropper dropper;
    [SerializeField] SpriteRenderer barRenderer;
    [SerializeField] Vector3 worldOffset = new Vector3(0f, 1.25f, 0f);
    [SerializeField] Sprite[] chargeSprites;

    void Awake()
    {
        dropper = dropper ? dropper : GetComponent<BombDropper>();
        EnsureRenderer();
        RefreshSprite(1f);
    }

    void LateUpdate()
    {
        if (barRenderer == null || dropper == null)
            return;

        barRenderer.transform.position = transform.position + worldOffset;
        RefreshSprite(dropper.NormalizedCharge);
    }

    void EnsureRenderer()
    {
        if (barRenderer != null)
            return;

        var child = transform.Find("BombBar");
        if (child == null)
        {
            var go = new GameObject("BombBar");
            go.transform.SetParent(transform);
            go.transform.localPosition = worldOffset;
            go.transform.localScale = Vector3.one;
            child = go.transform;
        }

        barRenderer = child.GetComponent<SpriteRenderer>();
        if (barRenderer == null)
            barRenderer = child.gameObject.AddComponent<SpriteRenderer>();
        barRenderer.sortingOrder = 300;
    }

    void RefreshSprite(float normalized)
    {
        if (barRenderer == null || chargeSprites == null || chargeSprites.Length == 0)
            return;

        int maxIndex = chargeSprites.Length - 1;
        int idx = Mathf.Clamp(Mathf.RoundToInt(normalized * maxIndex), 0, maxIndex);
        barRenderer.sprite = chargeSprites[idx];
    }

    void OnValidate()
    {
        dropper = dropper ? dropper : GetComponent<BombDropper>();
        EnsureRenderer();
        TryAutoAssignSprites();
    }

    void TryAutoAssignSprites()
    {
        if (chargeSprites != null && chargeSprites.Length > 0)
            return;

#if UNITY_EDITOR
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

