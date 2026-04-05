using UnityEngine;

public class KapiCikisi : MonoBehaviour
{
    private bool isOpen = false;

    public void OpenDoor()
    {
        isOpen = true;
        Debug.Log("Kapı Açıldı! 🎉");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen && other.CompareTag("Player"))
        {
            Debug.Log("Level Tamamlandı! 🏴‍☠️");
        }
    }
}