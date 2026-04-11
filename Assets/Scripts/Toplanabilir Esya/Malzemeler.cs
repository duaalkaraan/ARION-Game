using UnityEngine;

public class Malzemeler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance != null)
                GameManager.instance.CollectItem();
            Destroy(gameObject);
        }
    }
}