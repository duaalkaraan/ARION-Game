using System.Collections;
using UnityEngine;

[DisallowMultipleComponent] // Aynı objeye 2 kez eklenemez
public sealed class Bomb : MonoBehaviour // sealed = miras alınamaz
{
    [Header("Zamanlama")]
    [SerializeField] float fuseSeconds = 3f;        // Fitil süresi - kaç saniye sonra patlasın
    [SerializeField] float explosionLifetime = 0.8f; // Patlama efektinin ekranda kalma süresi

    [Header("Hasar Alanı")]
    [SerializeField] float killRadius = 2.2f;        // Patlama yarıçapı (birim)
    [SerializeField] LayerMask hitMask = ~0;         // Hangi layerlara hasar versin (~0 = hepsi)

    [Header("Görsel")]
    [SerializeField] Animator animator;              // Fitil animasyonu
    [SerializeField] SpriteRenderer spriteRenderer;  // Bomba görseli
    [SerializeField] GameObject explosionPrefab;     // Patlama efekti prefab'ı

    bool exploded; // Bomba patladı mı? (çift patlama önlemi)

    // Oyun başlamadan önce bileşenleri al
    void Awake()
    {
        animator = animator ? animator : GetComponent<Animator>();
        spriteRenderer = spriteRenderer ? spriteRenderer : GetComponent<SpriteRenderer>();
    }

    // Bomba aktif olduğunda çalışır (Instantiate veya SetActive sonrası)
    void OnEnable()
    {
        exploded = false;           // Patlama durumunu sıfırla
        StartCoroutine(FuseRoutine()); // Fitil geri sayımını başlat
    }

    // Fitil geri sayımı - fuseSeconds kadar bekleyip patlatır
    IEnumerator FuseRoutine()
    {
        // Fitil animasyonunu başlat (BombOn state'i)
        if (animator != null)
            animator.Play("BombOn", 0, 0f);

        // Fitil süresi kadar bekle
        yield return new WaitForSeconds(fuseSeconds);

        // Süre doldu - patlat!
        Explode();
    }

    // Bombanın patlama mantığı
    void Explode()
    {
        // Zaten patladıysa tekrar çalışma
        if (exploded) return;
        exploded = true;

        // Patlama yarıçapındaki tüm collider'ları bul
        var hits = Physics2D.OverlapCircleAll(transform.position, killRadius, hitMask);

        for (int i = 0; i < hits.Length; i++)
        {
            var c = hits[i];
            if (c == null) continue;

            // Oyuncu yakalandıysa öldür
            var player = c.GetComponentInParent<PlayerMovement2D>();
            if (player != null)
                player.Die();

            // Büyük düşman yakalandıysa öldür
            var enemy = c.GetComponentInParent<EnemyBigGuyAI2D>();
            if (enemy != null)
                enemy.Die();
        }

        // Bomba görselini gizle (patlama efekti görünsün diye)
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        // Patlama efekti oluştur ve belirli süre sonra sil
        if (explosionPrefab != null)
        {
            var pos = transform.position;
            pos.z = 0f; // 2D için Z'yi sıfırla
            var fx = Instantiate(explosionPrefab, pos, Quaternion.identity);
            Destroy(fx, explosionLifetime); // Efekti 0.8 saniye sonra sil
        }

        // Bomba objesini 0.05 saniye sonra sil (efektin başlamasına izin ver)
        Destroy(gameObject, 0.05f);
    }

    // Editor'da seçiliyken patlama alanını görselleştirir (oyunda görünmez)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.1f, 0.35f); // Yarı saydam turuncu
        Gizmos.DrawSphere(transform.position, killRadius); // Yarıçapı göster
    }
}