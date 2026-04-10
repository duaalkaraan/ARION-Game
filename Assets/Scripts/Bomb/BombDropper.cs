using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public sealed class BombDropper : MonoBehaviour
{
    [SerializeField] GameObject bombPrefab;
    [SerializeField, Min(0.1f)] float rechargeSeconds = 3f;
    [SerializeField] float dropCooldown = 0.1f;
    [SerializeField] Vector2 dropOffset = Vector2.zero;

    float charge01 = 1f;
    float nextDropTime;
    PlayerMovement2D player;
    public float NormalizedCharge => Mathf.Clamp01(charge01);
    public bool HasBombReady => charge01 >= 1f;

    // ✔ Oyuncu bileşenini bulur ve bomba prefabını otomatik atamayı dener
    void Awake()
    {
        player = GetComponent<PlayerMovement2D>();
        TryAutoAssignBombPrefab();
    }

    // ✔ Editörde değer değişince bomba prefabını otomatik bulmayı dener
    void OnValidate()
    {
        TryAutoAssignBombPrefab();
    }

    // ✔ Bombanın şarj olmasını, tuşa basılmasını ve bomba bırakmayı yönetir
    void Update()
    {
        if (bombPrefab == null)
            return;
        if (player != null && player.IsDead)
            return;

        // Bomba şarjını yenile
        if (charge01 < 1f)
            charge01 = Mathf.Min(1f, charge01 + (Time.deltaTime / rechargeSeconds));

        var kb = Keyboard.current;
        if (kb == null)
            return;

        // Bomb drop: E tuşu
        if (!kb.eKey.wasPressedThisFrame)
            return;

        if (!HasBombReady)
            return;

        if (Time.time < nextDropTime)
            return;
        nextDropTime = Time.time + dropCooldown;

        // Bombayı oluştur
        Vector3 pos = transform.position + (Vector3)dropOffset;
        pos.z = 0f;
        Instantiate(bombPrefab, pos, Quaternion.identity);
        charge01 = 0f;
    }

    // ✔ Editörde bomba prefabı atanmadıysa otomatik olarak bulmaya çalışır
    void TryAutoAssignBombPrefab()
    {
        if (bombPrefab != null)
            return;

#if UNITY_EDITOR
        bombPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Bomb/Bomb.prefab");
#endif
    }

    // ✔ Bombayı tamamen doldurur (örneğin checkpoint veya item sonrası)
    public void RefillBombs()
    {
        charge01 = 1f;
    }
}
