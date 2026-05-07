using UnityEngine;

public class KeyPart : MonoBehaviour
{
    public int keyID; // 0, 1 veya 2

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            KeyManager.Instance.CollectKey(keyID);
            Destroy(gameObject);
        }
    }
}
